using System;

namespace DependencyInjectionLibrary
{
    class DependencyConfigurator
    {

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

                
            }

        }

        private bool CanCreate(Type tInterface, Type tImplementation)
        {
            if (!tImplementation.IsInterface && !tImplementation.IsAbstract && (tInterface.IsAssignableFrom(tImplementation) || tInterface.IsGenericTypeDefinition))
                return true;
            else
                return false;
        }

    }
}
