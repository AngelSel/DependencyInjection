using System;

namespace DependencyInjectionLibrary
{
    class DependencyConfigurator
    {

        public void Register<TInterface,TImplementation>(Configurator.Lifetime lifetime = Configurator.Lifetime.Instance) 
            where TInterface:class 
            where TImplementation:TInterface
        {

        }


        public void Register(Type tInterface, Type tImplementation, Configurator.Lifetime lifetime = Configurator.Lifetime.Instance)
        {

        }

    }
}
