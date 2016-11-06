using System;
using System.Configuration;
using Tavisca.Frameworks.Session.Formatters;
using Tavisca.Frameworks.Session.Infrastructure;
using Tavisca.Frameworks.Session.Provider;
using Tavisca.Frameworks.Session.Provider.Redis;
using Tavisca.Frameworks.Session.Provider.SQL;

namespace Tavisca.Frameworks.Session.Configuration
{
    public class SessionSection : ConfigurationSection, ISessionConfiguration
    {

        [ConfigurationProperty("applicationKey", IsRequired = true)]
        public string ApplicationKey
        {
            get { return (string)this["applicationKey"]; }
            set { this["applicationKey"] = value; }
        }

        [ConfigurationProperty("provider", IsRequired = false, DefaultValue = ProviderTypeOptions.Sql)]
        public ProviderTypeOptions Provider
        {
            get { return (ProviderTypeOptions)this["provider"]; }
            set { this["provider"] = value; }
        }

        [ConfigurationProperty("customProviderType", IsRequired = false)]
        public string CustomProviderType
        {
            get { return (string)this["customProviderType"]; }
            set { this["customProviderType"] = value; }
        }

        [ConfigurationProperty("connString", IsRequired = true)]
        public string ConnStringNameOrValue
        {
            get { return (string)this["connString"]; }
            set { this["connString"] = value; }
        }

        [ConfigurationProperty("expireIn", IsRequired = false, DefaultValue = 300)]
        public int ExpirationTimeInSeconds
        {
            get { return (int)this["expireIn"]; }
            set { this["expireIn"] = value; }
        }

        private TimeSpan? _expiresIn;

        public TimeSpan ExpiresIn
        {
            get
            {
                if (_expiresIn == null)
                    _expiresIn = new TimeSpan(0,0,this.ExpirationTimeInSeconds);

                return _expiresIn.Value;
            }
        }

        [ConfigurationProperty("formatter", IsRequired = false, DefaultValue = FormatterTypeOptions.BinaryCompressed)]
        public FormatterTypeOptions Formatter
        {
            get { return (FormatterTypeOptions)this["formatter"]; }
            set { this["formatter"] = value; }
        }

        [ConfigurationProperty("customFormatterType", IsRequired = false)]
        public string CustomFormatterType
        {
            get { return (string)this["customFormatterType"]; }
            set { this["customFormatterType"] = value; }
        }

        [ConfigurationProperty("maxAsyncThreads", IsRequired = false)]
        public int? MaxAsyncThreads
        {
            get
            {
                return (int?)this["maxAsyncThreads"];
            }
            set
            {
                this["maxAsyncThreads"] = value;
            }
        }

        public ISessionDataProvider GetProvider()
        {
            return StorageProviderProvider.GetProvider(this);
        }

        public bool IsWritable { get { return false; } }

        public ISessionConfiguration GetWritableCopy()
        {
            return new WritableSessionSettings(ApplicationKey, Provider, CustomProviderType, 
                ConnStringNameOrValue, ExpirationTimeInSeconds, ExpiresIn, Formatter,
                CustomFormatterType, MaxAsyncThreads);
        }
    }
}
