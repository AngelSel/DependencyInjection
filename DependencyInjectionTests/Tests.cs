using NUnit.Framework;
using DependencyInjectionLibrary;
using System.Threading.Tasks;

namespace DependencyInjectionTests
{
    public class Tests
    {

        DependencyConfigurator dependencies;
        DependencyProvider provider;


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
            Assert.AreEqual(expected , ((actual as DoubleInnerClass).service as ServiceImplementation).repository.Print());
        }

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


        [Test]
        public void StandartGenericDependencyTest()
        {
            var dependencies = new DependencyConfigurator();
            dependencies.Register<IRepository, RepositoryImplementation>();
            dependencies.Register<IService<IRepository>, Service<IRepository>>();

            var provider = new DependencyProvider(dependencies);
            var actual = provider.Resolve<IService<IRepository>>();
            var expected = "Generics";

            Assert.AreEqual(expected,(actual as Service<IRepository>).Print());
        }

        [Test]
        public void OpenGenericDependencyTest()
        {
            var dependencies = new DependencyConfigurator();
            dependencies.Register(typeof(IService<>), typeof(Service<>));
            dependencies.Register<IRepository, RepositoryImplementation>();

            var provider = new DependencyProvider(dependencies);
            var actual = provider.Resolve<IService<IRepository>>();
            var expected = "Generics";

            Assert.AreEqual(expected,(actual as Service<IRepository>).Print());
        }

    }
}