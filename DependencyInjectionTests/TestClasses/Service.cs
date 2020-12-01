namespace DependencyInjectionTests.TestClasses
{
    interface IService<TRepository> where TRepository : IRepository
    {
        string Print();
    }

    class Service<TRepository> : IService<TRepository>
        where TRepository : IRepository
    {
        private string outputString = "Generics";
        public IRepository rep = null;
        public Service(TRepository _rep)
        {
            this.rep = _rep;
        }

        public string Print()
        {
            return outputString;
        }
    }
}
