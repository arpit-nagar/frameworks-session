using System;
using Tavisca.Frameworks.Parallel;
using Tavisca.Frameworks.Session.Configuration;
using Tavisca.Frameworks.Session.Exceptions;
using Tavisca.Frameworks.Session.Provider;
using Tavisca.Frameworks.Session.Resources;

namespace Tavisca.Frameworks.Session
{
    /// <summary>
    /// The API for storing and retrieving data based on a category and key (Config driven).
    /// </summary>
    public sealed class SessionStore
    {
        #region Private Members

        private readonly ISessionDataProvider _provider;
        private readonly ISessionConfiguration _configuration;

        #endregion

        #region Constructors

        public SessionStore()
        {
            _configuration = SessionConfigurationManager.GetConfiguration();
            _provider = _configuration.GetProvider();
        }

        public SessionStore(ISessionConfiguration configuration)
        {
            _configuration = configuration;
            _provider = configuration.GetProvider();
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Gets the session value from the back end store.
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve</typeparam>
        /// <param name="category">The category under which the object is stored.</param>
        /// <param name="sessionId">The key against which the object is stored in the category.</param>
        public T Get<T>(string category, string sessionId)
        {
            try
            {
                return _provider.Get<T>(category, sessionId);
            }
            catch (SessionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SessionException(SessionResources.Error_Generic, ex);
            }
        }

        /// <summary>
        /// Adds Session Synchronously. Use it when you want application to add Value Synchronously.
        /// </summary>
        /// <param name="category">The category under which the object is stored.</param>
        /// <param name="sessionId">The key against which the object is stored in the category.</param>
        /// <param name="value">Value that needs to be added to DataStore</param>
        /// <param name="expiresOn">Expiration TimeSpan</param>
        public void Add(string category, string sessionId, object value, TimeSpan expiresOn)
        {
            try
            {
                _provider.Add(category, sessionId, value, expiresOn);
            }
            catch (SessionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SessionException(SessionResources.Error_Generic, ex);
            }
        }

        /// <summary>
        /// Adds Session Synchronously. Use it when you want application to add Value Synchronously.
        /// </summary>
        /// <param name="category">The category under which the object is stored.</param>
        /// <param name="sessionId">The key against which the object is stored in the category.</param>
        /// <param name="value">Value that needs to be added to DataStore</param>
        public void Add(string category, string sessionId, object value)
        {
            try
            {
                Add(category, sessionId, value, _configuration.ExpiresIn);
            }
            catch (SessionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SessionException(SessionResources.Error_Generic, ex);
            }
        }

        /// <summary>
        /// Adds Session Asynchronously. Use it when you want application to add Value asynchronously.
        /// </summary>
        /// <param name="category">The category under which the object is stored.</param>
        /// <param name="sessionId">The key against which the object is stored in the category.</param>
        /// <param name="value">Value that needs to be added to DataStore</param>
        /// <param name="expiresOn">Expiration TimeSpan</param>
        public void AddAsync(string category, string sessionId, object value, TimeSpan expiresOn)
        {
            try
            {
                var factory = GetAmbientTaskFactory();

                factory.StartAmbient(() => Add(category, sessionId, value, expiresOn));
            }
            catch (SessionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SessionException(SessionResources.Error_Generic, ex);
            }
        }

        /// <summary>
        /// Adds Session Asynchronously. Use it when you want application to add Value asynchronously.
        /// </summary>
        /// <param name="category">The category under which the object is stored.</param>
        /// <param name="sessionId">The key against which the object is stored in the category.</param>
        /// <param name="value">Value that needs to be added to DataStore</param>
        public void AddAsync(string category, string sessionId, object value)
        {
            try
            {
                AddAsync(category, sessionId, value, _configuration.ExpiresIn);
            }
            catch (SessionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SessionException(SessionResources.Error_Generic, ex);
            }
        }

        /// <summary>
        /// Removes the value from the back end store.
        /// </summary>
        /// <param name="category">The category under which the object is stored.</param>
        /// <param name="sessionId">The key against which the object is stored in the category.</param>
        public void Remove(string category, string sessionId)
        {
            try
            {
                _provider.Remove(category, sessionId);
            }
            catch (SessionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SessionException(SessionResources.Error_Generic, ex);
            }
        }

        /// <summary>
        /// Removes the value from the back end store asynchronously
        /// </summary>
        /// <param name="category">The category under which the object is stored.</param>
        /// <param name="sessionId">The key against which the object is stored in the category.</param>
        public void RemoveAsync(string category, string sessionId)
        {
            try
            {
                var factory = GetAmbientTaskFactory();

                factory.StartAmbient(() => Remove(category, sessionId));
            }
            catch (SessionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SessionException(SessionResources.Error_Generic, ex);
            }
        }

        #endregion

        #region Private Members

        private AmbientTaskFactory GetAmbientTaskFactory()
        {
            return TaskFactoryFactory.GetTaskFactory(
                SchedulerTypeOptions.LimitedConcurrencyScheduler,
                GetLocalizedTaskSchedulerOptions());
        }

        private TaskFactoryFactory.LocalizedSchedulerOptions GetLocalizedTaskSchedulerOptions()
        {
            const string key = "Tav.Fr.Ses";

            var configuration = _configuration;

            if (configuration.MaxAsyncThreads.HasValue && configuration.MaxAsyncThreads.Value > 0)
            {
                return new TaskFactoryFactory.LocalizedSchedulerOptions(key, configuration.MaxAsyncThreads.Value);
            }

            return new TaskFactoryFactory.LocalizedSchedulerOptions(key);
        }

        #endregion
    }
}
