namespace DependencyInjectionTests.TestClasses
{
    public class ClassWithoutPublicConstructor:ISingleDependency
    {
        private ClassWithoutPublicConstructor() { }
    }
}
