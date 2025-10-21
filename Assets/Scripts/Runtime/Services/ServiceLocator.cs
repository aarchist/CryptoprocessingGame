using System;
using System.Collections.Generic;
using Services.Core;

namespace Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IService> _services = new();

        public static TService Get<TService>()
        {
            return (TService)_services[typeof(TService)];
        }

        public static void Register<TService>(TService service) where TService : IService
        {
            _services.Add(typeof(TService), service);
        }

        public static void Initialize()
        {
            foreach (var service in _services.Values)
            {
                service.Initialize();
            }
        }

        public static void Dispose()
        {
            foreach (var service in _services.Values)
            {
                service.Dispose();
            }
            _services.Clear();
        }
    }
}
