using System;
using System.Configuration;
using System.Linq;
using Tavisca.Frameworks.Session.Exceptions;
using Tavisca.Frameworks.Session.Resources;

namespace Tavisca.Frameworks.Session.Provider.SQL
{
    public sealed class SqlSessionDataProvider : SessionDataProviderBase
    {
        #region Fields

        private string _connectionStringName;

        #endregion

        #region Helper Methods

        private void SetConnectionName(string nameOrConnectionString)
        {
            var length = nameOrConnectionString.IndexOf('=');
            if (length < 0)
            {
                _connectionStringName = nameOrConnectionString;
            }
            else if (nameOrConnectionString.IndexOf('=', length + 1) >= 0)
            {
                _connectionStringName = null;
            }
            else if (nameOrConnectionString.Substring(0, length).Trim().Equals("name", StringComparison.OrdinalIgnoreCase))
            {
                _connectionStringName = nameOrConnectionString.Substring(length + 1).Trim();
            }
            else
            {
                _connectionStringName = null;
            }
        }

        private DataStoreDataContext GetSqlContext()
        {
            if (_connectionStringName != null)
            {
                return new DataStoreDataContext(
                    ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString);
            }

            throw new SessionConfigurationException(string.Format(SessionResources.Invalid_ConnString, SessionResources.ProviderName_Sql));
        }

        #endregion

        #region Constructors

        public SqlSessionDataProvider(string connStringNameOrValue) : base(connStringNameOrValue)
        {
            SetConnectionName(connStringNameOrValue);
        }

        #endregion

        #region IDataRepoProvider Methods

        public override void Add(string category, string key, object value, TimeSpan expireIn)
        {
            var currDateUtc = DateTime.UtcNow;
            if (value == null)
                return;

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentNullException("category");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            Guid sessionId;

            if (!Guid.TryParse(key, out sessionId))
                throw new ArgumentException(string.Format(SessionResources.Key_Not_Guid, SessionResources.ProviderName_Sql), "key");
            var data = GetFormatter().Format(value);

            using (var context = GetSqlContext())
            {
                context.spAddEntry(category, sessionId, new System.Data.Linq.Binary(data), currDateUtc, currDateUtc.Add(expireIn));
            }
        }

        public override T Get<T>(string category, string key)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentNullException("category");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            Guid sessionId;

            if (!Guid.TryParse(key, out sessionId))
                throw new ArgumentException(string.Format(SessionResources.Key_Not_Guid, SessionResources.ProviderName_Sql), "key");

            spGetEntryResult data;
            using (var context = GetSqlContext())
            {
                data = context.spGetEntry(sessionId, category).SingleOrDefault();
            }

            if (data != null)
            {
                if (data.ExpiresOnUTC > DateTime.UtcNow)
                    return data.ObjectValue == null ? default(T) :
                        GetFormatter().FromFormatted<T>(data.ObjectValue.ToArray());

                Remove(category, key);
            }
            return default(T);
        }

        public override bool Remove(string category, string key)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentNullException("category");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            Guid sessionId;

            if (!Guid.TryParse(key, out sessionId))
                throw new ArgumentException(string.Format(SessionResources.Key_Not_Guid, SessionResources.ProviderName_Sql), "key");

            using (var context = GetSqlContext())
            {
                context.spRemoveEntry(sessionId, category);
            }

            return true;
        }

        #endregion
    }
}
