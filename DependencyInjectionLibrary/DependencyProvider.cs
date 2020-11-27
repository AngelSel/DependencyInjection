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
    
            if(typeof(IEnumerable).IsAssignableFrom(tDependency) && tDependency.IsGenericType)
            {
                return Create(tDependency);
            }
            else if(tDependency.IsGenericType && _dependencyConfig.GetConfigurator(tDependency)==null)
            {
                Type implementation = GetImplementationType(tDependency);
                Configurator configurator = _dependencyConfig.GetConfigurator(tDependency.GetGenericTypeDefinition());
                return CreateInstance(configurator);

            }
            else if(_dependencyConfig.GetConfigurator(tDependency) != null)
            {
                return CreateInstance(_dependencyConfig.GetConfigurator(tDependency));
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


        private object CreateInstance(Configurator config,Type implement = null)
        {
            Type implementation = null;
            if(implement == null)
            {
                implementation = config.Implementation;
            }
            else
            {
                implementation = implement;
            }
                

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
                var currentParameters = _dependencyConfig.GetConfigurator(parameters[i].ParameterType);
                if (currentParameters!=null)
                {
                    paramType = currentParameters.Interface;
                }
                else
                {
                    paramType = parameters[i].ParameterType;
                }

                values[i] = Resolve(paramType);
            }
            return values;
        }


        private Type GetImplementationType(Type tDependency)
        {
            var argument = tDependency.GetGenericArguments().FirstOrDefault();
            var implemetation = _dependencyConfig.GetConfigurator(tDependency.GetGenericTypeDefinition()).Implementation;
            if (implemetation!=null)
            {
                return implemetation.MakeGenericType(argument);
            }
            else
                return null;
        }
    }
}

