using System;
using System.Configuration;
using Tavisca.Frameworks.Session.Exceptions;
using Tavisca.Frameworks.Session.Resources;

namespace Tavisca.Frameworks.Session.Configuration
{
    public static class SessionConfigurationManager
    {
        #region Configure

        private static ISessionConfiguration _apiSessionConfiguration;

        static SessionConfigurationManager()
        {
            SetConfiguration((ISessionConfiguration)ConfigurationManager.GetSection("DataStore"));
        }

        /// <summary>
        /// Sets the configuration for the api session, 
        /// all currently created objects for <see cref="SessionStore"/> will continue to use the old configuration.
        /// </summary>
        /// <param name="apiSessionConfiguration">The configuration to set.</param>
        public static void SetConfiguration(ISessionConfiguration apiSessionConfiguration)
        {
            _apiSessionConfiguration = apiSessionConfiguration;
        }

        #endregion

        #region Usage

        /// <summary>
        /// Gets the currently active configuration for the library.
        /// </summary>
        public static ISessionConfiguration GetConfiguration()
        {
            if (_apiSessionConfiguration == null)
                throw new SessionConfigurationException(SessionResources.Configuration_MissingError);

            return _apiSessionConfiguration;
        }

        #endregion
    }
}
