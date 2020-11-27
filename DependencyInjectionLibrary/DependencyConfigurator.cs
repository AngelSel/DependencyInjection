using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjectionLibrary
{
    public class DependencyConfigurator
    {
        public Dictionary<Type, List<Configurator>> registeredConfigurations = new Dictionary<Type, List<Configurator>>();

        public void Register<TInterface,TImplementation>(Configurator.Lifetime lifetime = Configurator.Lifetime.Instance) 
            where TInterface:class 
            where TImplementation:TInterface
        {
            Register(typeof(TInterface), typeof(TImplementation));
        }


        public void Register(Type tInterface, Type tImplementation, Configurator.Lifetime lifetime = Configurator.Lifetime.Instance)
        {
            if(CanCreate(tInterface,tImplementation))
            {
                Configurator configurator = new Configurator(tInterface, tImplementation, lifetime);

                if(registeredConfigurations.ContainsKey(tInterface))
                {
                    registeredConfigurations[tInterface].Add(configurator);
                }
                else
                {
                    registeredConfigurations.Add(tInterface, new List<Configurator> { configurator });
                }           
            }
        }

        private bool CanCreate(Type tInterface, Type tImplementation)
        {
            if (!tImplementation.IsInterface && !tImplementation.IsAbstract && (tInterface.IsAssignableFrom(tImplementation) || tInterface.IsGenericTypeDefinition))
                return true;
            else
                return false;
        }

        private Configurator GetConfigurator(Type tInterface)
        {
            if (registeredConfigurations.TryGetValue(tInterface, out var types))
                return types.Last();
            else
                return null;    
        }

        private IEnumerable<Configurator> GetConfigurators(Type tInterface)
        {
            if (registeredConfigurations.TryGetValue(tInterface, out var types))
                return types;
            else
                return null;
        }

    }
}
