using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjectionTests
{
    public interface ISingleDependency { }

    public class SingleDependency : ISingleDependency
    {
        public int a = 0;
        public char b = 'a';
    }

    public interface IService { }

    public class ServiceImplementation : IService
    {
        public IRepository repository = null;

        public ServiceImplementation(IRepository rep)
        {
            repository = rep;
        }
    }

    public interface IRepository
    {
        string Print();
    }

    public class RepositoryImplementation : IRepository
    {
        private string message = "Repository was created";
        public RepositoryImplementation() { }

        public string Print()
        {
            return message;
        }

    }

    public class DoubleInnerClass:ISingleDependency
    {
        public IService service;

        public DoubleInnerClass(IService s)
        {
            service = s;
        }
    }

    public interface ISingleton { }
    public class SingletonClass : ISingleton
    {
        public int a { get; }

        public SingletonClass(int value)
        {
            a = value;
        }
    }


}
