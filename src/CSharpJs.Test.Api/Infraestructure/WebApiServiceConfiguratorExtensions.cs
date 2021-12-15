using System;
using Topshelf.ServiceConfigurators;

namespace Topshelf.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebApiServiceConfiguratorExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="webConfigurator"></param>
        /// <returns></returns>
        public static ServiceConfigurator<T> WebApiEndpoint<T>(this ServiceConfigurator<T> configurator, Action<WebApiConfigurator> webConfigurator) where T : class
        {
            var config = new WebApiConfigurator();

            webConfigurator(config);

            configurator.BeforeStartingService(t => config.Initialize());
            configurator.BeforeStoppingService(t => config.Shutdown());

            return configurator;
        }
    }
}