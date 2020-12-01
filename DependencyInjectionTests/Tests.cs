using NUnit.Framework;
using DependencyInjectionLibrary;
using System.Threading.Tasks;
using DependencyInjectionTests.TestClasses;
using System.Collections.Generic;
using System;

namespace DependencyInjectionTests
{
    public class Tests
    {

        DependencyConfigurator dependencies;
        DependencyProvider provider;

        //Simple test for resolving basic implementation
        [Test]
        public void SimpleDependencyTest()
        {
            dependencies = new DependencyConfigurator();
            dependencies.Register<ISingleDependency, SingleDependency>();
            provider = new DependencyProvider(dependencies);

            var actual = provider.Resolve<ISingleDependency>();
            var expected = new SingleDependency();

            Assert.AreEqual(expected.a, (actual as SingleDependency).a);
        }


        //Test for resolving dependencies with multiple implementation
        [Test]
        public void MultipleImplementations()
        {
            dependencies = new DependencyConfigurator();
            dependencies.Register<ISingleDependency, SingleDependency>(Configurator.Lifetime.Singleton);
            dependencies.Register<ISingleDependency, AnotherSingleDependency>(Configurator.Lifetime.Singleton);
            provider = new DependencyProvider(dependencies);

            var actual = provider.Resolve<IEnumerable<ISingleDependency>>() as List<object>;
            int expected = 2;
            Assert.AreEqual(expected, actual.Count);

            Type[] actual2 = new Type[] { actual[0].GetType(), actual[1].GetType() };
            Type[] expected2 = new Type[] { typeof(SingleDependency), typeof(AnotherSingleDependency) };
            CollectionAssert.AreEquivalent(expected2, actual2);
        }


        //Test for inner dependencies
        [Test]
        public void ClassWithInnerDependencyTest()
        {
            dependencies = new DependencyConfigurator();
            dependencies.Register<IRepository, RepositoryImplementation>();
            dependencies.Register<IService, ServiceImplementation>();
            provider = new DependencyProvider(dependencies);


            var actual = provider.Resolve<IService>();
            var expected = "Repository was created";

            Assert.AreEqual(expected, (actual as ServiceImplementation).repository.Print());
        }


        //Test for double inner dependencies
        [Test]
        public void ClassWithDoubleInnerDependencyTest()
        {
            dependencies = new DependencyConfigurator();
            dependencies.Register<IRepository, RepositoryImplementation>();
            dependencies.Register<IService, ServiceImplementation>();
            dependencies.Register<ISingleDependency, DoubleInnerClass>();
            provider = new DependencyProvider(dependencies);

            var actual = provider.Resolve<ISingleDependency>();
            var expected = "Repository was created";
            Assert.AreEqual(expected, ((actual as DoubleInnerClass).service as ServiceImplementation).repository.Print());
        }


        //Test for checking singleton
        [Test]
        public void SingletonTest()
        {
            dependencies = new DependencyConfigurator();
            dependencies.Register<ISingleton, SingletonClass>(Configurator.Lifetime.Singleton);
            provider = new DependencyProvider(dependencies);

            var actual = provider.Resolve<ISingleton>();
            var expected1 = Task.Run(() => provider.Resolve<ISingleton>());
            var expected2 = Task.Run(() => provider.Resolve<ISingleton>());
            var expected3 = Task.Run(() => provider.Resolve<ISingleton>());

            Assert.AreEqual(expected1.Result, actual);
            Assert.AreEqual(expected2.Result, actual);
            Assert.AreEqual(expected3.Result, actual);
        }


        // Test for registering and resolving of generic dependencies
        [Test]
        public void StandartGenericDependencyTest()
        {
            dependencies = new DependencyConfigurator();
            dependencies.Register<IRepository, RepositoryImplementation>();
            dependencies.Register<IService<IRepository>, Service<IRepository>>();

            var provider = new DependencyProvider(dependencies);
            var actual = provider.Resolve<IService<IRepository>>();
            var expected = "Generics";

            Assert.AreEqual(expected, (actual as Service<IRepository>).Print());
        }


        // Test for registering and resolving with open generics
        [Test]
        public void OpenGenericDependencyTest()
        {
            dependencies = new DependencyConfigurator();
            dependencies.Register(typeof(IService<>), typeof(Service<>));
            dependencies.Register<IRepository, RepositoryImplementation>();

            var provider = new DependencyProvider(dependencies);
            var actual = provider.Resolve<IService<IRepository>>();
            var expected = "Generics";

            Assert.AreEqual(expected, (actual as Service<IRepository>).Print());
        }


        //Test for checking some exeptional situations
        [Test]
        public void TestExceptionSituations()
        {
            dependencies = new DependencyConfigurator();
            dependencies.Register<AService, AbstractClassImplementation>();
            dependencies.Register<ISingleDependency, ClassWithoutPublicConstructor>();

            var provider = new DependencyProvider(dependencies);
            var actual = provider.Resolve<AService>();
            Assert.IsNull(actual);

            var actual2 = provider.Resolve<ISingleDependency>();
            Assert.IsNull(actual2);
        }
    }
}