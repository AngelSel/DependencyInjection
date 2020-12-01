namespace DependencyInjectionTests.TestClasses
{
    public interface IService { }

    public class ServiceImplementation : IService
    {
        public IRepository repository = null;

        public ServiceImplementation(IRepository rep)
        {
            repository = rep;
        }
    }
}
