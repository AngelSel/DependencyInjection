namespace DependencyInjectionTests.TestClasses
{
    public class DoubleInnerClass : ISingleDependency
    {
        public IService service;

        public DoubleInnerClass(IService s)
        {
            service = s;
        }
    }
}
