using Ninject.Modules;
using Store.DAL.Interfaces;
using Store.DAL.Repositories;

namespace AsuBlog.Util
{
    public class UnitOfWorkModule : NinjectModule
    {
        private string connectionString;
        public UnitOfWorkModule(string connection)
        {
            connectionString = connection;
        }
        public override void Load()
        {
            Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument(connectionString);
        }
    }
}