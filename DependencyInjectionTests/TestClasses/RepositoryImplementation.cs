namespace DependencyInjectionTests.TestClasses
{
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

}
