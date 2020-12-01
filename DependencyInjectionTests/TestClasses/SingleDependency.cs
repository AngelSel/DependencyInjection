namespace DependencyInjectionTests.TestClasses
{
    public interface ISingleDependency { }

    public class SingleDependency : ISingleDependency
    {
        public int a = 0;
        public char b = 'a';
    }
}
