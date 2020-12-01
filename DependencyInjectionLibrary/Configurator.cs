using System;

namespace DependencyInjectionLibrary
{
    public class Configurator
    {
        public enum Lifetime
        {
            Singleton,
            Instance
        }

        public Lifetime LifeTime { get; }
        public Type Interface { get; }
        public Type Implementation { get; }

        public Configurator(Type currentInterface,Type currentImplementation, Lifetime lifetime)
        {

            Interface = currentInterface;
            Implementation = currentImplementation;
            LifeTime = lifetime;
        }
    }
}
