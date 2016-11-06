using System;
using Tavisca.Frameworks.Session.Formatters;
using Tavisca.Frameworks.Session.Provider;

namespace Tavisca.Frameworks.Session.Configuration
{
    public interface ISessionConfiguration
    {
        string ApplicationKey { get; set; }

        ProviderTypeOptions Provider { get; set; }

        string CustomProviderType { get; set; }

        string ConnStringNameOrValue { get; set; }

        int ExpirationTimeInSeconds { get; set; }

        TimeSpan ExpiresIn { get; }

        FormatterTypeOptions Formatter { get; set; }

        string CustomFormatterType { get; set; }

        int? MaxAsyncThreads { get; set; }

        ISessionDataProvider GetProvider();

        bool IsWritable { get; }

        ISessionConfiguration GetWritableCopy();
    }
}
