using System;
using System.Collections.Concurrent;

namespace Tavisca.Frameworks.Session.DependencyInjection
{
    public class ServiceLocator
    {
        private ServiceLocator()
        {

        }

        private readonly ConcurrentDictionary<Type, object> Registry 
            = new ConcurrentDictionary<Type, object>();

        public void RegisterCustomInstance(Type serviceType, object instance)
        {
            if (!serviceType.IsAssignableFrom(instance.GetType()))
                throw new InvalidOperationException("instance not compatible with the service type");

            Registry[serviceType] = instance;
        }

        internal T GetService<T>()
        {
            object instance;
            Registry.TryGetValue(typeof(T), out instance);

            return (T)instance;
        }

        public static ServiceLocator Default { get; } = new ServiceLocator();
    }
}
