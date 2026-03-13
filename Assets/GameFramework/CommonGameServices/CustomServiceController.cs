using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace GF
{
    public class CustomServiceController : IService
    {
        public bool IsUpdateRequired { get; private set; }
        private Dictionary<Type, IService> services = new Dictionary<Type, IService>();
        private List<IService> updateRequiredServices = new List<IService>();
        private string nameSpaceType;
        private bool _isRegistered = false;
        public void Initialize()
        {
            IsUpdateRequired = true;
            Logger.Log(LogType.Log, "CustomServiceController created");
        }
        public void RegisterListener()
        {
            EventManager.Instance.AddListener<RegisterCustomServiceEvent>(RegisterCustomService);
        }

        private void RegisterCustomService(RegisterCustomServiceEvent e)
        {
            if(_isRegistered) return;
            nameSpaceType = e.NameSpaceType;
            foreach (var type in GetAllTypesThatImplementInterface<IService>(e))
            {
                services.Add(type, (IService)Activator.CreateInstance(type));
            }
            foreach (var service in services.Values)
            {
                service.Initialize();
                service.RegisterListener();
            }
            updateRequiredServices = services.Values.Where(s => s.IsUpdateRequired == true).ToList();
            _isRegistered=true;
        }
        private IEnumerable<Type> GetAllTypesThatImplementInterface<T>(RegisterCustomServiceEvent e)
        {
            return e.Assembly
                .GetTypes()
                .Where(type => type.Namespace == nameSpaceType && typeof(T).IsAssignableFrom(type) && !type.IsInterface);
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
            EventManager.Instance.RemoveListener<RegisterCustomServiceEvent>(RegisterCustomService);
        }

        public void Update()
        {
            foreach (var service in updateRequiredServices)
            {
                service.Update();
            }
        }
    }
}