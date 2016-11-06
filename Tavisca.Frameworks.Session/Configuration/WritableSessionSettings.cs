using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavisca.Frameworks.Session.Formatters;
using Tavisca.Frameworks.Session.Provider;

namespace Tavisca.Frameworks.Session.Configuration
{
    public class WritableSessionSettings : ISessionConfiguration
    {
        public string ApplicationKey { get; set; }
        public ProviderTypeOptions Provider { get; set; }
        public string CustomProviderType { get; set; }
        public string ConnStringNameOrValue { get; set; }
        public int ExpirationTimeInSeconds { get; set; }
        public TimeSpan ExpiresIn { get; private set; }
        public FormatterTypeOptions Formatter { get; set; }
        public string CustomFormatterType { get; set; }
        public int? MaxAsyncThreads { get; set; }

        public WritableSessionSettings(string applicationKey, ProviderTypeOptions provider, 
            string customProviderType, string connStringNameOrValue, int expirationTimeInSeconds,
            TimeSpan expiresIn, FormatterTypeOptions formatter, string customFormatterType,
            int? maxAsyncThreads)
        {
            ApplicationKey = applicationKey;
            Provider = provider;
            CustomProviderType = customProviderType;
            ConnStringNameOrValue = connStringNameOrValue;
            ExpirationTimeInSeconds = expirationTimeInSeconds;
            ExpiresIn = expiresIn;
            Formatter = formatter;
            CustomFormatterType = customFormatterType;
            MaxAsyncThreads = maxAsyncThreads;
        }

        public ISessionDataProvider GetProvider()
        {
            return StorageProviderProvider.GetProvider(this);
        }

        public bool IsWritable { get { return true; } }
        
        public ISessionConfiguration GetWritableCopy()
        {
            return this;
        }
    }
}
