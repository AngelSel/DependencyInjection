using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyInjectionLibrary
{
    public class DependencyProvider
    {
        private ConcurrentDictionary<Type, object> Instances = new ConcurrentDictionary<Type, object>();
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
            bool contains = _dependencyConfig.registeredConfigurations.TryGetValue(tDependency, out var res);

            if(typeof(IEnumerable).IsAssignableFrom(tDependency))
            {
                return Create(tDependency);
            }
            else if(tDependency.IsGenericType && !contains)
            {

            }
            else if(contains)
            {
                return CreateInstance(res.Last());
            }

            return null;
        }


        private object Create(Type type)
        {
            var argumentType = type.GetGenericArguments()[0];

            if(_dependencyConfig.registeredConfigurations.TryGetValue(argumentType, out var configuratedTypes))
            {
                var deps = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(argumentType));

                foreach (var configuratedType in configuratedTypes)
                {
                    deps.Add(CreateInstance(configuratedType));
                }

                return deps;
            }
            return null;
        }


        private object CreateInstance(Configurator config)
        {

            Type implementation = config.Implementation;

            if (config.LifeTime == Configurator.Lifetime.Singleton && Instances.ContainsKey(implementation))
                return Instances[implementation];

            ConstructorInfo[] constructors = implementation.GetConstructors().OrderByDescending(x => x.GetParameters().Length).ToArray();

            bool isCreated = false;
            int constructorsAmount = 1;
            object resultObject = null;

            while(!isCreated && constructorsAmount<=constructors.Count())
            {
                try
                {
                    ConstructorInfo currentConstructor = constructors[constructorsAmount - 1];
                    object[] parametrs = GetParams(currentConstructor);
                    resultObject = Activator.CreateInstance(implementation, parametrs);
                    isCreated = true;
                }
                catch(Exception e)
                {
                    isCreated = false;
                    constructorsAmount++;
                }
            }


            if (config.LifeTime == Configurator.Lifetime.Singleton && !Instances.ContainsKey(implementation))
                if (!Instances.TryAdd(implementation, resultObject))
                    return Instances[implementation];
            return resultObject;

        }


        private object[] GetParams(ConstructorInfo info)
        {
            ParameterInfo[] parameters = info.GetParameters();
            object[] values = new object[parameters.Length];

            for(int i = 0;i<parameters.Length;i++)
            {
                Type paramType = null;
                if (_dependencyConfig.registeredConfigurations.TryGetValue(parameters[i].ParameterType, out var currentParameters))
                {
                    var currentParam = currentParameters.Last();
                    paramType = currentParam.Interface;
                }
                else
                {
                    paramType = parameters[i].ParameterType;
                }

                values[i] = Resolve(paramType);

            }

            return values;
        }

    }
}

