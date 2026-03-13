using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace GF
{
    public class GameServiceController
    {
        private Dictionary<Type, IService> services = new Dictionary<Type, IService>();
        private List<IService> updateRequiredServices = new List<IService>();
        public void Initialize()
        {
            Logger.Log(LogType.Log, "GameServiceController created");
            foreach (var type in GetAllTypesThatImplementInterface<IService>())
            {
                services.Add(type, (IService)Activator.CreateInstance(type));
            }
        }

        private IEnumerable<Type> GetAllTypesThatImplementInterface<T>()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => type.Namespace == "GF" && typeof(T).IsAssignableFrom(type) && !type.IsInterface);
        }
        public void RegisterListener()
        {
            foreach (var service in services.Values)
            {
                service.Initialize();
                service.RegisterListener();
            }
            updateRequiredServices = services.Values.Where(s => s.IsUpdateRequired == true).ToList();
        }
        public void AddService(Type serviceType)
        {
            var service = (IService)Activator.CreateInstance(serviceType);
            service.Initialize();
            service.RegisterListener();
            services.Add(serviceType, service);
            if (service.IsUpdateRequired) updateRequiredServices.Add(service);
        }
        public void Update()
        {
            foreach (var service in updateRequiredServices)
            {
                service.Update();
            }
        }
        public void RemoveListener()
        {
            foreach (var service in services.Values)
            {
                if (service.GetType() == typeof(Service))
                {
                    ((Service)service).Dispose();
                }
                else
                {
                    service.RemoveListener();
                }
            }
            services.Clear();
            updateRequiredServices.Clear();
        }
    }
}