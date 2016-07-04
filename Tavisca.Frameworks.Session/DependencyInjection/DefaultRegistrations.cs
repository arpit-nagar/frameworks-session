using System;
using System.Collections.Generic;
using Tavisca.Frameworks.Session.Provider.AeroSpike;

namespace Tavisca.Frameworks.Session.DependencyInjection
{
    internal class DefaultRegistrations
    {
        public static IEnumerable<KeyValuePair<Type, object>> GetDefaultSet()
        {
            yield return new KeyValuePair<Type, object>(typeof(IAerospikeInstanceFactory),
                DefaultAerospikeInstanceFactory.Instance);
        }
    }
}
