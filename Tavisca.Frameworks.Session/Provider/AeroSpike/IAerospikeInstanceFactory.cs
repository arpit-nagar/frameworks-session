using Aerospike.Client;

namespace Tavisca.Frameworks.Session.Provider.AeroSpike
{
    public interface IAerospikeInstanceFactory
    {
        AsyncClient GetInstance(string host, int port);
    }
}
