using Lamar;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace Topshelf.WebApi.Lamar
{
    /// <summary>
    /// 
    /// </summary>
    public class LamarDependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        public LamarDependencyResolver(IContainer kernel)
        {
            _container = kernel;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            return _container.TryGetInstance(serviceType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            var services = new List<object>();
            try
            {
                foreach (var item in _container.GetAllInstances(serviceType))
                    services.Add(item);

                return services;
            }
            catch (Exception)
            {
                return services;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDependencyScope BeginScope()
        {
            return this;
        }
    }
}