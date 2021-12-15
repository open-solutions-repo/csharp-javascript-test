using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpJs.Test.Api.Infraestructure
{
    /// <summary>
    /// 
    /// </summary>
    public class Migration
    {
        private readonly ConnectionPoolManager _poolManager;
        private readonly CurrentRequest _request;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="poolManager"></param>
        /// <param name="request"></param>
        public Migration(ConnectionPoolManager poolManager, CurrentRequest request)
        {
            _poolManager = poolManager;
            _request = request;
        }
        /// <summary>
        /// Executa todas as migrations pendentes
        /// </summary>
        /// <returns></returns>
        public async Task RunMigration()
        {
            Log.Information($"Migrations StartUp in thread {Thread.CurrentThread.ManagedThreadId}");

            //Rodar migrations e criar cache para conexões
            foreach (var cname in Connection.Configurations())
            {
                if (cname.Key == "Common")
                    continue;

                _request.CompanyKey = cname.Key;

                Log.Information($"Company:{cname.Key}");
            }

            Log.Information("Migrations Finish");
        }
    }
}
