using System.Collections.Generic;

namespace CSharpJs.Test.Api.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseService<T> : IService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        void Save(T model);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="N"></typeparam>
        /// <param name="code"></param>
        /// <returns></returns>
        T Get<N>(N code);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<dynamic> GetAll();
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IService
    {

    }
}
