using System;
using System.Collections.Concurrent;
using Aerospike.Client;
using Tavisca.Frameworks.Session.Exceptions;
using Tavisca.Frameworks.Session.Resources;
using Tavisca.Frameworks.Session.DependencyInjection;

namespace Tavisca.Frameworks.Session.Provider.AeroSpike
{
    public class AeroSpikeSessionDataProvider : SessionDataProviderBase
    {
        private readonly string _applicationKey;
        protected string Host { get; set; }
        protected int Port { get; set; }

        private object _clientLock = new object();
        private AsyncClient _client;
        private AsyncClient Client
        {
            get
            {
                if (_client != null)
                    return _client;
                lock(_clientLock)
                {
                    if (_client == null)
                        _client = GetNewClient();
                }
                return _client;
            }
        }
        static AeroSpikeSessionDataProvider()
        {
            ServiceLocator.Default.RegisterCustomInstance(typeof(IAerospikeInstanceFactory),
                DefaultAerospikeInstanceFactory.Instance);
        }

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

            var data = GetFormatter().Format(value);

            var bin = new Bin(GetBinKey(), data);

            Client.Put(GetWritePolicy(), GetKey(category, key), bin);
        }

        public override T Get<T>(string category, string key)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentNullException("category");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            var record = Client.Get(GetQueryPolicy(), GetKey(category, key));

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

            return Client.Delete(GetWritePolicy(), GetKey(category, key));
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

        private AsyncClient GetNewClient()
        {
            var instanceFactory = ServiceLocator.Default.GetService<IAerospikeInstanceFactory>();
            return instanceFactory.GetInstance(Host, Port);
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
