using System;
using Tavisca.Frameworks.Session.Configuration;
using Tavisca.Frameworks.Session.Formatters;

namespace Tavisca.Frameworks.Session.Provider
{
    public abstract class SessionDataProviderBase : ISessionDataProvider
    {
        protected string ConnStringNameOrValue { get; private set; }

        protected virtual ISessionDataFormatter GetFormatter()
        {
            return FormatterFactory.GetFormatter(Configuration);
        }

        protected SessionDataProviderBase(string connStringNameOrValue)
        {
            ConnStringNameOrValue = connStringNameOrValue;
        }

        public ISessionConfiguration Configuration { get; set; }

        public abstract void Add(string category, string key, object value, TimeSpan expireIn);
        public abstract T Get<T>(string category, string key);
        public abstract bool Remove(string category, string key);
    }
}