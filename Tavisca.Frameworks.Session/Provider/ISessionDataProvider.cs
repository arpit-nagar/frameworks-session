using System;
using Tavisca.Frameworks.Session.Configuration;

namespace Tavisca.Frameworks.Session.Provider
{
    public interface ISessionDataProvider
    {
        ISessionConfiguration Configuration { get; set; }
        void Add(string category, string key, object value, TimeSpan expireIn);
        T Get<T>(string category, string key);
        bool Remove(string category, string key);
    }
}
