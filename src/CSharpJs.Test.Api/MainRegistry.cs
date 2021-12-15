using Lamar;
using Microsoft.Owin;
using System.Net.Http;
using CSharpJs.Test.Api.Infraestructure;

namespace CSharpJs.Test.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class MainRegistry : ServiceRegistry
    {
        /// <summary>
        /// 
        /// </summary>
        public MainRegistry()
        {
            For<IStore5Service>().Use<Store5Service>();

            ForConcreteType<ConnectionPoolManager>().Configure.Singleton();
            ForConcreteType<Migration>().Configure.Singleton();

            For<CurrentRequest>().Use<CurrentRequest>().Scoped();
            For<IOwinContext>().Use(ctx =>
                ctx.GetInstance<CurrentRequest>()
                    .RequestData.GetOwinContext());

            For<Connection>().Use(ctx => GetConnection(ctx));
        }

        private Connection GetConnection(IServiceContext ctx)
        {
            var request = ctx.GetInstance<CurrentRequest>();
            return request.ConnectionFromRequest();
        }
    }
}
