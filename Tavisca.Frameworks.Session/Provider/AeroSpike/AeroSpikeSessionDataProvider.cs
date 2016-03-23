using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aerospike.Client;
using Tavisca.Frameworks.Session.Exceptions;
using Tavisca.Frameworks.Session.Resources;

namespace Tavisca.Frameworks.Session.Provider.AeroSpike
{
    public class AeroSpikeSessionDataProvider : SessionDataProviderBase
    {
        private readonly string _applicationKey;
        protected string Host { get; set; }
        protected int Port { get; set; }

        public AeroSpikeSessionDataProvider(string connStringNameOrValue, string applicationKey)
            : base(connStringNameOrValue)
        {
            _applicationKey = applicationKey;
            ParseConnString(connStringNameOrValue);
        }

        public override void Add(string category, string key, object value, TimeSpan expireIn)
        {
            if (value == null)
                return;

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentNullException("category");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            var client = GetAeroSpikeClient();

            var data = GetFormatter().Format(value);

            var bin = new Bin(GetBinKey(), data);

            client.Put(GetWritePolicy(), GetKey(category, key), bin);
        }

        public override T Get<T>(string category, string key)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentNullException("category");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            var client = GetAeroSpikeClient();

            var record = client.Get(GetQueryPolicy(), GetKey(category, key));

            if (record == null)
                return default(T);

            var data = (byte[])record.GetValue(GetBinKey());

            return GetFormatter().FromFormatted<T>(data);
        }

        public override bool Remove(string category, string key)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentNullException("category");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            var client = GetAeroSpikeClient();

            return client.Delete(GetWritePolicy(), GetKey(category, key));
        }

        protected virtual ClientPolicy GetClientPolicy()
        {
            var policy = new ClientPolicy
                {
                    failIfNotConnected = true,
                    maxThreads = Configuration.MaxAsyncThreads.HasValue ? Configuration.MaxAsyncThreads.Value : 5,
                    maxSocketIdle = 100000,
                    timeout = 30000
                };
            policy.user = string.Empty;
            policy.password = string.Empty;

            return policy;
        }

        private static WritePolicy _writePolicy;
        protected virtual WritePolicy GetWritePolicy()
        {
            return _writePolicy ?? (_writePolicy = new WritePolicy()
                {
                    expiration = Convert.ToInt32(Configuration.ExpiresIn.TotalMilliseconds),
                    maxRetries = 2,
                    priority = Priority.DEFAULT,
                    sleepBetweenRetries = 50
                });
        }

        private static QueryPolicy _queryPolicy;
        protected virtual QueryPolicy GetQueryPolicy()
        {
            return _queryPolicy ?? (_queryPolicy = new QueryPolicy()
            {
                maxRetries = 2,
                priority = Priority.DEFAULT,
                sleepBetweenRetries = 50,
                recordQueueSize = 1
            });
        }

        protected virtual void ParseConnString(string connectionString)
        {
            var split = connectionString.Split(':');

            if (split.Length != 2)
                throw new SessionConfigurationException(string.Format(SessionResources.AeroSpike_IncorrectConnString,
                    connectionString));

            int port;

            if (!int.TryParse(split[1], out port))
                throw new SessionConfigurationException(string.Format(SessionResources.AeroSpike_InvalidPort,
                    split[1]));

            Host = split[0];
            Port = port;
        }

        protected virtual Key GetKey(string category, string key)
        {
            return new Key(_applicationKey, category, key);
        }

        private static readonly ConcurrentDictionary<string, AerospikeClient> AerospikeClients = 
            new ConcurrentDictionary<string, AerospikeClient>();

        protected AerospikeClient GetAeroSpikeClient()
        {
            return AerospikeClients.GetOrAdd(GetHostKey(),
                                              s => new AerospikeClient(GetClientPolicy(), Host, Port));
        }

        protected virtual string GetHostKey()
        {
            return ConnStringNameOrValue;
        }

        protected string GetBinKey()
        {
            return "bin1";
        }
    }
}
