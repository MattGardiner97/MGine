using MGine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Structures
{
    public class ServiceManager<TInterface>
    {
        private Engine engine;
        private Dictionary<Type, TInterface> services = new Dictionary<Type, TInterface>();

        public ServiceManager(Engine Engine)
        {
            this.engine = Engine;
        }

        public TService RegisterService<TService>() where TService : TInterface
        {
            if (services.ContainsKey(typeof(TService)))
                return default(TService);

            TService service;
            if (typeof(TService).GetConstructor(new Type[] { typeof(Engine) }) != null)
                service = (TService)Activator.CreateInstance(typeof(TService), new object[] { engine });
            else
                service = Activator.CreateInstance<TService>();

            services.Add(typeof(TService), service);

            return service;
        }

        public TService RegisterService<TService>(TService Service) where TService : TInterface
        {
            if (services.ContainsKey(typeof(TService)))
                throw new ArgumentOutOfRangeException($"{typeof(TService).FullName} has already been registered.");

            services.Add(typeof(TService), Service);

            return Service;
        }

        public TService GetService<TService>() where TService : TInterface
        {
            if (services.ContainsKey(typeof(TService)) == false)
                throw new ArgumentOutOfRangeException($"{typeof(TService).FullName} has not been registered.");

            return (TService)services[typeof(TService)];
        }

        public void ForAll(Action<TInterface> Action)
        {
            foreach (TInterface service in services.Values)
                Action(service);
        }

    }
}
