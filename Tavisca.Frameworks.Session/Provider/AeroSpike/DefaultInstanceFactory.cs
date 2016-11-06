using Aerospike.Client;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Tavisca.Frameworks.Session.Provider.AeroSpike
{
    public class DefaultAerospikeInstanceFactory : IAerospikeInstanceFactory
    {
        private DefaultAerospikeInstanceFactory()
        {

        }
        private static object _clientCreationLock = new object();
        private static IDictionary<string, AsyncClient> _instanceCache = new ConcurrentDictionary<string, AsyncClient>();
        public AsyncClient GetInstance(string host, int port)
        {
            var clientKey = string.Concat(host, port);
            AsyncClient client;
            if (_instanceCache.TryGetValue(clientKey, out client))
                return client;

            lock(_clientCreationLock)
            {
                if (_instanceCache.TryGetValue(clientKey, out client))
                    return client;
                client = new AsyncClient(host, port);
                _instanceCache[clientKey] = client;

                return client;
            }
        }

        public static DefaultAerospikeInstanceFactory Instance { get; } = new DefaultAerospikeInstanceFactory();
    }
}
