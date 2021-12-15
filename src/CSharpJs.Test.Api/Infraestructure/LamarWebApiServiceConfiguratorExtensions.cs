using Lamar;
using System;
using System.Web.Http;
using Topshelf.Lamar;
using Topshelf.Logging;

namespace Topshelf.WebApi.Lamar
{
    /// <summary>
    /// 
    /// </summary>
    public static class LamarWebApiServiceConfiguratorExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static HttpConfiguration UseLamarDependencyResolver(this HttpConfiguration configurator)
        {
            var log = HostLogger.Get(typeof(LamarWebApiServiceConfiguratorExtensions));

            IContainer container = LamarBuilderConfigurator.Container;

            if (container == null)
                throw new Exception("You must call UseLamar() to use the WebApi Topshelf Lamar integration.");

            configurator.DependencyResolver = new LamarDependencyResolver(container);

            log.Info("[Topshelf.WebApi.Lamar] WebAPI Dependency Resolver configured to use Lamar.");

            return configurator;
        }
    }
}