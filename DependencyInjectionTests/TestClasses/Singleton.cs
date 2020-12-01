namespace DependencyInjectionTests.TestClasses
{
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
