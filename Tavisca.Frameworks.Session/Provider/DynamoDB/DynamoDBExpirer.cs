using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Tavisca.Frameworks.Session.Exceptions;
using Tavisca.Frameworks.Session.Infrastructure;
using Tavisca.Frameworks.Session.Resources;

namespace Tavisca.Frameworks.Session.Provider.DynamoDB
{
    public sealed class DynamoDBExpirer
    {
        public DynamoDBExpirer(string connString)
        {
            LoadConnectionStringValues(connString);
        }

        public void RemoveExpiredItems()
        {
            //trying twice, delete in batches does not delete some items sometimes, twice ensures cleanup.
            for (var i = 0; i < 2; i++) 
            {
                InternalRemoveExpiredItems();
            }
        }

        public void InternalRemoveExpiredItems()
        {
            Action<ICollection<string>> action = objectsToExpireBatch => {
                BatchWriteItemResponse response;
                using (var client = CreateClient())
                {
                    response = client.BatchWriteItem(new BatchWriteItemRequest()
                        {
                            RequestItems = new Dictionary<string, List<WriteRequest>>()
                                {
                                    {
                                        _tableName, GetWriteRequests(objectsToExpireBatch)
                                    }
                                }
                        });
                }

                if (response.HttpStatusCode != HttpStatusCode.OK)
                    Console.WriteLine("Dynamodb returned a not ok response, code was {0}, and unprocessed items were {1}", 
                        response.HttpStatusCode, response.UnprocessedItems);
            };

            foreach (var batch in GetObjectsToExpireInBatches())
            {
                System.Threading.Thread.Sleep(GetSleepTime());
                action(batch);
            }
            //System.Threading.Tasks.Parallel.ForEach(GetObjectsToExpireInBatches(), new ParallelOptions()
            //                                                                           {
            //                                                                               MaxDegreeOfParallelism = 2
            //                                                                           }, action);
        }

        private int GetSleepTime()
        {
            if (DateTime.UtcNow.Hour > 3 && DateTime.UtcNow.Hour < 12)
            {
                return 350;
            }
            return 900;
        }

        #region Private Methods

        private static List<WriteRequest> GetWriteRequests(IEnumerable<string> ids)
        {
            var retVal = new List<WriteRequest>();

            foreach (var id in ids)
            {
                retVal.Add(
                    new WriteRequest(new DeleteRequest()
                        {
                            Key = new Dictionary<string, AttributeValue>()
                                {
                                    {DynamoDBMeta.IdColumn, new AttributeValue(id)}
                                }
                        })
                    );
            }

            return retVal;
        }

        private IEnumerable<ICollection<string>> GetObjectsToExpireInBatches()
        {
            foreach (var staticSecondaryIndexVal in GetStaticSecondaryIndexVals())
            {
                QueryResponse response;
                using (var client = CreateClient())
                {
                    response = client.Query(new QueryRequest(_tableName)
                    {
                        IndexName = "expiryIndex",
                        KeyConditions = new Dictionary<string, Condition>()
                            {
                                {
                                    DynamoDBMeta.ExpiryStaticColumn, 
                                    new Condition()
                                        {
                                            ComparisonOperator = ComparisonOperator.EQ, 
                                            AttributeValueList = new List<AttributeValue>() { staticSecondaryIndexVal }
                                        }
                                },
                                {
                                    DynamoDBMeta.ExpiryEpochColumn,
                                    new Condition()
                                    {
                                        ComparisonOperator = ComparisonOperator.LT,
                                        AttributeValueList = new List<AttributeValue>()
                                            {
                                                new AttributeValue(){N = GetEpoch(TimeSpan.Zero).ToString(CultureInfo.InvariantCulture)}
                                            }
                                    }
                                }
                            }
                    });
                }

                Console.WriteLine("Expiry index batch#{0} starting.", staticSecondaryIndexVal.N);

                if (response.HttpStatusCode != HttpStatusCode.OK || response.Items.Count == 0)
                    continue;

                var items = response.Items.Select(x => x[DynamoDBMeta.IdColumn].S).ToList();

                if (items.Count <= 10)
                {
                    yield return items;
                }
                else
                {
                    var batches = ToBatches(items, 10);

                    foreach (var batch in batches)
                    {
                        yield return batch;
                    }
                }
                Console.WriteLine("Expiry index batch#{0} completed.", staticSecondaryIndexVal.N);
            }
        }

        public static List<List<T>> ToBatches<T>(ICollection<T> collection, int batchSize)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (batchSize < 1)
                throw new ArgumentException("batchSize cannot be less than 1", "batchSize");

            var batchCount = Convert.ToInt32(Math.Ceiling(
                (Convert.ToDouble(collection.Count) / Convert.ToDouble(batchSize))
                ));

            var batches = new List<List<T>>(batchCount);

            for (var i = 0; i < batchCount; i++)
            {
                var batch = collection.Skip(i * batchSize).Take(batchSize).ToList();

                batches.Add(new List<T>(batch));
            }

            return batches;
        }

        private readonly static DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static long GetEpoch(TimeSpan value)
        {
            var time = DateTime.UtcNow.Add(value);

            return Convert.ToInt64(Math.Floor((time - Epoch).TotalSeconds));
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

        private static List<AttributeValue> _staticSecondaryIndexVals;
        private static IEnumerable<AttributeValue> GetStaticSecondaryIndexVals()
        {
            return _staticSecondaryIndexVals ??
                (_staticSecondaryIndexVals =
                Enumerable.Range(1, DynamoDBMeta.MaxSecondaryIndexVal).Select(x => new AttributeValue() { N = x.ToString(CultureInfo.InvariantCulture) }).ToList());
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

        #endregion
    }
}
