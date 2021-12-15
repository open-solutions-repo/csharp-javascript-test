using Lamar;
using Owin;
using System;
using Topshelf.Lamar;

namespace CSharpJs.Test.Api.Infraestructure
{
    /// <summary>
    /// 
    /// </summary>
    public static class LoggerMiddlewareConfiguratorExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <returns></returns>
        public static IAppBuilder UseLoggerRequest(this IAppBuilder appBuilder)
        {
            IContainer container = LamarBuilderConfigurator.Container;

            if (container == null)
                throw new Exception("You must call UseLamar() to use the WebApi Topshelf Lamar integration.");

            return appBuilder;
        }
    }
}
