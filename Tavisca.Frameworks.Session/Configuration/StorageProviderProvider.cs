using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavisca.Frameworks.Session.Exceptions;
using Tavisca.Frameworks.Session.Formatters;
using Tavisca.Frameworks.Session.Infrastructure;
using Tavisca.Frameworks.Session.Provider;
using Tavisca.Frameworks.Session.Provider.AeroSpike;
using Tavisca.Frameworks.Session.Provider.DynamoDB;
using Tavisca.Frameworks.Session.Provider.Redis;
using Tavisca.Frameworks.Session.Provider.SQL;
using Tavisca.Frameworks.Session.Resources;

namespace Tavisca.Frameworks.Session.Configuration
{
    internal static class StorageProviderProvider
    {
        public static ISessionDataProvider GetProvider(ISessionConfiguration configuration)
        {
            ISessionDataProvider retVal = null;
            switch (configuration.Provider)
            {
                case ProviderTypeOptions.Sql:
                    retVal = new SqlSessionDataProvider(configuration.ConnStringNameOrValue);
                    break;
                case ProviderTypeOptions.Redis:
                    retVal = new RedisSessionDataProvider(configuration.ConnStringNameOrValue, configuration.ApplicationKey);
                    break;
                case ProviderTypeOptions.AeroSpike:
                    retVal = new AeroSpikeSessionDataProvider(configuration.ConnStringNameOrValue, configuration.ApplicationKey);
                    break;
                case ProviderTypeOptions.DynamoDB:
                    retVal = new DynamoDBSessionDataProvider(configuration.ConnStringNameOrValue, configuration.ApplicationKey);
                    break;
                case ProviderTypeOptions.Custom:
                    if (string.IsNullOrWhiteSpace(configuration.CustomProviderType))
                        throw new SessionConfigurationException(SessionResources.CustomProvider_Missing);

                    var provider = configuration.CustomProviderType.Construct();

                    retVal = provider as ISessionDataProvider;

                    if (retVal == null)
                        throw new SessionConfigurationException(string.Format(SessionResources.CustomProvider_InvalidType, typeof(ISessionDataProvider).FullName));
                    break;
            }

            if (retVal != null)
                retVal.Configuration = configuration;

            return retVal;
        }
    }
}
