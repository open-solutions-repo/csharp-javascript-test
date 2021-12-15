using System.Threading.Tasks;
using CSharpJs.Test.Api.Infraestructure;

namespace CSharpJs.Test.Api
{
    internal class Store5Service : IStore5Service
    {
        public bool Start()
        {
            var migration = ServiceLocator.Get<Migration>();
            Task.Run(async () =>
            {
                
                await migration.RunMigration();
            }).Wait();

            return true;
        }

        public bool Stop()
        {
            return true;
        }
    }
}