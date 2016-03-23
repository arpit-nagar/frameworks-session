using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Tavisca.Frameworks.Session.Exceptions;
using Tavisca.Frameworks.Session.Infrastructure;
using Tavisca.Frameworks.Session.Resources;

namespace Tavisca.Frameworks.Session.Provider.DynamoDB
{
    public sealed class DynamoDBSessionDataProvider : SessionDataProviderBase
    {
        private readonly string _applicationKey;

        public DynamoDBSessionDataProvider(string connStringNameOrValue, string applicationKey)
            : base(connStringNameOrValue)
        {
            _applicationKey = applicationKey;
            LoadConnectionStringValues(connStringNameOrValue);
        }

        #region SessionDataProviderBase Members

        public override void Add(string category, string key, object value, TimeSpan expireIn)
        {
            var id = GetKey(category, key);

            var data = GetFormatter().Format(value);

            var secondary = new Random().Next(1, DynamoDBMeta.MaxSecondaryIndexVal + 1).ToString(CultureInfo.InvariantCulture);

            var expiry = GetEpoch(expireIn).ToString(CultureInfo.InvariantCulture);

            var items = GetItemsToWrite(id, secondary, data, expiry);

            AddItems(items);
        }

        public override T Get<T>(string category, string key)
        {
            var id = GetKey(category, key);

            GetItemResponse response;
            using (var client = CreateClient())
            {
                response = client.GetItem(_tableName, new Dictionary<string, AttributeValue>()
                    {
                        {DynamoDBMeta.IdColumn, new AttributeValue(id)}
                    });
            }

            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                throw CreateInvalidResponseException(
                    response.ResponseMetadata == null ? null : response.ResponseMetadata.Metadata,
                    response.HttpStatusCode);
            }

            if (response.Item.Count == 0)
                return default(T);

            var data = response.Item[DynamoDBMeta.DataColumn].B.ToArray();

            int itemCount;
            if (!int.TryParse(response.Item[DynamoDBMeta.ItemCountColumn].N, out itemCount))
                throw new SessionException(SessionResources.UnexpectedDynamoData);

            if (itemCount == 1)
                return GetFormatter().FromFormatted<T>(data);

            var restofData = GetRestOfTheData(id, itemCount);

            var netData = new byte[data.Length + restofData.Length];

            var length = data.Length;

            Array.Copy(data, netData, length);
            
            for (var i = 0; i < restofData.Length; i++)
            {
                netData[length + i] = restofData[i];
            }

            return GetFormatter().FromFormatted<T>(netData);
        }

        public override bool Remove(string category, string key)
        {
            var id = GetKey(category, key);

            DeleteItemResponse response;
            using (var client = CreateClient())
            {
                response = client.DeleteItem(_tableName, new Dictionary<string, AttributeValue>()
                    {
                        {DynamoDBMeta.IdColumn, new AttributeValue(id)}
                    });
            }

            return response.HttpStatusCode == HttpStatusCode.OK;
        }

        #endregion

        #region Private Methods

        private readonly static DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static long GetEpoch(TimeSpan value)
        {
            var time = DateTime.UtcNow.Add(value);

            return Convert.ToInt64(Math.Floor((time - Epoch).TotalSeconds));
        }

        private string GetKey(string category, string key)
        {
            return _applicationKey + "-" + category + "-" + key;
        }

        private AmazonDynamoDBClient CreateClient()
        {
            var config = new AmazonDynamoDBConfig
            {
                ServiceURL = _url,
                MaxErrorRetry = 2
            };

            return new AmazonDynamoDBClient(new BasicAWSCredentials(_accessKey, _secretKey), config);
        }

        private string _url;
        private string _accessKey;
        private string _secretKey;
        private string _tableName;

        private const string ConnStringItemSeperator = "|||";
        private const string ConnStringFormat = "url|||tableName|||AccessKey|||SecretKey";

        private void LoadConnectionStringValues(string connString)
        {
            if (string.IsNullOrWhiteSpace(connString))
                throw new SessionConfigurationException(SessionResources.DynamoDB_EmptyConnString);

            connString = DecryptConnString(connString);

            var splitVals = connString.Split(new[] { ConnStringItemSeperator }, StringSplitOptions.RemoveEmptyEntries);

            if (splitVals.Length != 4)
                throw CreateIncorrectFormatConnStringException("Incorrect number of parameters after splitting by seperator detected.");

            Uri uri;

            if (!Uri.TryCreate(splitVals[0], UriKind.Absolute, out uri))
                throw CreateIncorrectFormatConnStringException("Invalid uri in the string");

            _url = uri.ToString();

            if (string.IsNullOrWhiteSpace(splitVals[1]))
                throw CreateIncorrectFormatConnStringException("Empty table name");

            _tableName = splitVals[1];

            if (string.IsNullOrWhiteSpace(splitVals[2]))
                throw CreateIncorrectFormatConnStringException("Empty access key");

            _accessKey = splitVals[2];

            if (string.IsNullOrWhiteSpace(splitVals[3]))
                throw CreateIncorrectFormatConnStringException("Empty secret key");

            _secretKey = splitVals[3];
        }

        private static string DecryptConnString(string connString)
        {
            try
            {
                return connString.TripleDESDecrypt();
            }
            catch (SessionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SessionConfigurationException(SessionResources.ErrorDecryptingConnString, ex);
            }
        }

        private static SessionException CreateIncorrectFormatConnStringException(string hint)
        {
            var ex = new SessionConfigurationException(
                string.Format(
                SessionResources.DynamoDB_IncorrectConnString,
                ConnStringFormat
                ));

            if (!string.IsNullOrWhiteSpace(hint))
                ex.Data.Add("Hint", hint);

            return ex;
        }

        private static SessionException CreateInvalidResponseException(ICollection<KeyValuePair<string, string>> metaData, HttpStatusCode statusCode)
        {
            var ex = new SessionException(
                string.Format(
                SessionResources.DynamoDB_ErrorInResponse, Enum.GetName(typeof(HttpStatusCode), statusCode)));

            if (metaData != null && metaData.Count > 0)
            {
                foreach (var pair in metaData)
                {
                    ex.Data.Add(pair.Key, pair.Value);
                }
            }

            return ex;
        }

        private byte[] GetRestOfTheData(string id, int itemsCount)
        {
            if (itemsCount == 2)
            {
                GetItemResponse response;
                using (var client = CreateClient())
                {
                    response = client.GetItem(_tableName, new Dictionary<string, AttributeValue>()
                        {
                            {DynamoDBMeta.IdColumn, new AttributeValue(id + "1")}
                        });
                }

                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw CreateInvalidResponseException(
                        response.ResponseMetadata == null ? null : response.ResponseMetadata.Metadata,
                        response.HttpStatusCode);
                }

                return response.Item[DynamoDBMeta.DataColumn].B.ToArray();
            }

            var map = new Dictionary<int, byte[]>();

            BatchGetItemResponse batchGetItemResponse = null;
            
            using (var client = CreateClient())
            {
                do
                {
                    var req = batchGetItemResponse != null ? new BatchGetItemRequest(batchGetItemResponse.UnprocessedKeys) : new BatchGetItemRequest(new Dictionary<string, KeysAndAttributes>()
                        {
                            {
                                _tableName, new KeysAndAttributes()
                                    {
                                        AttributesToGet = new List<string>() {DynamoDBMeta.DataColumn, DynamoDBMeta.IdColumn},
                                        Keys =
                                            Enumerable.Range(1, itemsCount).Select(
                                                x => new Dictionary<string, AttributeValue>()
                                                    {
                                                        {
                                                            DynamoDBMeta.IdColumn,
                                                            new AttributeValue(id +
                                                                               x.ToString(CultureInfo.InvariantCulture))
                                                        }
                                                    }).ToList()
                                    }
                            }
                        });

                    batchGetItemResponse = client.BatchGetItem(req);

                    foreach (var response in batchGetItemResponse.Responses[_tableName])
                    {
                        map[Int32.Parse(response[DynamoDBMeta.IdColumn].S.Last().ToString(CultureInfo.InvariantCulture))] = 
                            response[DynamoDBMeta.DataColumn].B.ToArray();
                    }

                } while (batchGetItemResponse.UnprocessedKeys != null && batchGetItemResponse.UnprocessedKeys.Count > 0);
            }

            var retVal = new byte[map.Sum(x => x.Value.Length)];

            var startIndex = 0;

            for (var i = 1; i < itemsCount; i++)
            {
                var data = map[i];

                for (var j = 0; j < data.Length; j++)
                {
                    retVal[j + startIndex] = data[j];
                }
                startIndex += data.Length;
            }

            return retVal;
        }

        private void AddItems(List<Dictionary<string, AttributeValue>> items)
        {
            if (items.Count == 0)
                return;

            if (items.Count == 1)
            {
                PutItemResponse response;
                using (var client = CreateClient())
                {
                    response = client.PutItem(_tableName, items[0]);
                }

                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw CreateInvalidResponseException(
                        response.ResponseMetadata == null ? null : response.ResponseMetadata.Metadata,
                        response.HttpStatusCode);
                }

                return;
            }

            BatchWriteItemResponse batchWriteItemResponse = null;
            do
            {
                var writeRequest = batchWriteItemResponse == null
                                        ? new Dictionary<string, List<WriteRequest>>(){
                                            {_tableName, items.Select(item => new WriteRequest(new PutRequest(item))).ToList()}
                                        }
                                        : batchWriteItemResponse.UnprocessedItems;

                using (var client = CreateClient())
                {
                    batchWriteItemResponse = client.BatchWriteItem(writeRequest);
                }
            } while (batchWriteItemResponse.UnprocessedItems != null && batchWriteItemResponse.UnprocessedItems.Count > 0);

            if (batchWriteItemResponse.HttpStatusCode != HttpStatusCode.OK)
            {
                throw CreateInvalidResponseException(
                    batchWriteItemResponse.ResponseMetadata == null ? null : batchWriteItemResponse.ResponseMetadata.Metadata,
                    batchWriteItemResponse.HttpStatusCode);
            }
        }

        private static List<Dictionary<string, AttributeValue>> GetItemsToWrite(string id, string secondary, byte[] data, string expiry)
        {
            var batches = CreateBatches(data).ToArray();

            if (batches.Length > 25)
                throw new SessionException(SessionResources.DataTooLarge);

            var firstItem = new Dictionary<string, AttributeValue>()
                {
                    {DynamoDBMeta.IdColumn, new AttributeValue(id)},
                    {DynamoDBMeta.ExpiryStaticColumn, new AttributeValue(){N = secondary}},
                    {DynamoDBMeta.DataColumn, new AttributeValue(){B = new MemoryStream(batches[0])}},
                    {
                        DynamoDBMeta.ExpiryEpochColumn,
                        new AttributeValue(){N = expiry}
                    },
                    {
                        DynamoDBMeta.ItemCountColumn, 
                        new AttributeValue(){N = batches.Length.ToString(CultureInfo.InvariantCulture)}
                    }
                };

            var list = new List<Dictionary<string, AttributeValue>>();

            list.Add(firstItem);

            for (var i = 1; i < batches.Length; i++)
            {
                list.Add(
                        new Dictionary<string, AttributeValue>()
                            {
                                {DynamoDBMeta.IdColumn, new AttributeValue(id + i.ToString(CultureInfo.InvariantCulture))},
                                {DynamoDBMeta.ExpiryStaticColumn, new AttributeValue(){N = secondary}},
                                {DynamoDBMeta.DataColumn, new AttributeValue(){B = new MemoryStream(batches[i])}},
                                {
                                    DynamoDBMeta.ExpiryEpochColumn,
                                    new AttributeValue(){N = expiry}
                                },
                                {
                                    DynamoDBMeta.ItemCountColumn, 
                                    new AttributeValue(){N = "0"}
                                }
                            }
                    );
            }

            return list;
        }

        private static IEnumerable<byte[]> CreateBatches(byte[] data)
        {
            const int maxDataSize = 358400;
            const double maxDataSizeInDouble = 358400d;

            var batchSize = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(data.Length) / maxDataSizeInDouble)); //350kb

            if (batchSize == 1)
            {
                yield return data;
                yield break;
            }

            var lastIndex = 0;

            for (var i = 0; i < batchSize; i++)
            {
                var dataLeft = data.Length - (i * maxDataSize);

                var arrSize = dataLeft > maxDataSize ? maxDataSize : dataLeft;

                var arr = new byte[arrSize];
                
                for (var j = 0; j < arrSize; j++)
                {
                    arr[j] = data[j + lastIndex];
                }

                lastIndex += arrSize;

                yield return arr;
            }
        }

        #endregion
    }
}
