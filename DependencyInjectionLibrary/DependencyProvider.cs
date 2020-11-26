
using System;

namespace DependencyInjectionLibrary
{
    public class DependencyProvider
    {

        private DependencyConfigurator _dependencyConfig;

        public DependencyProvider(DependencyConfigurator config)
        {
            _dependencyConfig = config;
        }

        public TDependency Resolve<TDependency>() where TDependency : class
        {
            return (TDependency)Resolve(typeof(TDependency));
        }


        private object Resolve(Type tDependency)
        {
            return null;
        }
    }
}

