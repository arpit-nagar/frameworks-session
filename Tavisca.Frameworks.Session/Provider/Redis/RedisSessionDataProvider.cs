using System;
using System.Linq;
using ServiceStack.Redis;
using Tavisca.Frameworks.Helper.Net;
using Tavisca.Frameworks.Helper.Redis;
using Tavisca.Frameworks.Session.Exceptions;
using Tavisca.Frameworks.Session.Resources;

namespace Tavisca.Frameworks.Session.Provider.Redis
{
    public class RedisSessionDataProvider : SessionDataProviderBase
    {
        #region Constructors

        public RedisSessionDataProvider(string connStringNameOrValue, string applicationKey)
            : base(connStringNameOrValue)
        {
            _applicationKey = applicationKey;

            ParseConnString(connStringNameOrValue);
        }

        #endregion

        #region IApiSessionProvider Methods

        public override void Add(string category, string key, object value, TimeSpan expireIn)
        {
            if (value == null)
                return;

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentNullException("category");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");          

            var session = new DataEntry
                {
                    ObjectName = category,
                    Id = key,
                    ObjectValue = GetFormatter().Format(value),
                    ExpiresOn = DateTime.UtcNow.Add(expireIn),
                    AddedOn = DateTime.UtcNow,
                };

            try
            {
                using (var client = RedisClientManager.GetClient(_host, _port))
                {
                    var typedClient = client.As<DataEntry>();

                    typedClient.SetEntry(GetKey(category, key), session, expireIn);
                }
            }
            catch (RedisException)
            {
                var resolver = new DnsResolver();

                resolver.RemoveCache(_host);

                throw;
            }
        }

        public override T Get<T>(string category, string key)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentNullException("category");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");           

            try
            {
                DataEntry retValue;
                using (var client = RedisClientManager.GetClient(_host, _port))
                {
                    var typedClient = client.As<DataEntry>();

                    retValue = typedClient.GetValue(GetKey(category, key));
                }

                return retValue == null ? default(T) : GetFormatter().FromFormatted<T>(retValue.ObjectValue);
            }
            catch (RedisException)
            {
                var resolver = new DnsResolver();

                resolver.RemoveCache(_host);

                throw;
            }
        }

        public override bool Remove(string category, string key)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentNullException("category");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");          

            try
            {
                using (var client = RedisClientManager.GetClient(_host, _port))
                {
                    var typedClient = client.As<DataEntry>();

                    return typedClient.RemoveEntry(GetKey(category, key));
                }
            }
            catch (RedisException)
            {
                throw;
            }
        }

        #endregion

        #region Private Members

        private readonly string _applicationKey;

        private string GetKey(string category, string key)
        {
            return _applicationKey + "-" + category + "-" + key;
        }

        private string _host;
        private int _port = 6379;

        private void ParseConnString(string connectionString)
        {
            var split = connectionString.Split(':');

            int port;
            if (split.Length == 2 && int.TryParse(split[1], out port))
                _port = port;

            _host = split[0];
        }

        #endregion
    }
}
