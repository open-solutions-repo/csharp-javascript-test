using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace CSharpJs.Test.Api.Infraestructure
{
    /// <summary>
    /// 
    /// </summary>
    public class CurrentRequest
    {
        private readonly ConnectionPoolManager _poolManager;

        /// <summary>
        /// 
        /// </summary>
        public string CompanyKey
        {
            get
            {
                //if (_cKey != null)
                    //return _cKey;

                IEnumerable<string> l = new List<string>();
                
                var b = RequestData?.Headers.TryGetValues("company", out l) ?? false;

                if (b && string.IsNullOrEmpty( l.First())) throw new Exception("Por favor limpar o cache do navegador");
                
                return b ? new List<string>(l).First() : _cKey;
            }
            set => _cKey = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="poolManager"></param>
        public CurrentRequest(ConnectionPoolManager poolManager)
        {
            _poolManager = poolManager;
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid RequestId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public HttpRequestMessage RequestData { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private string _cKey;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Connection ConnectionFromRequest()
        {
           
            return _poolManager.GetConnection(CompanyKey);
        }
    }
}
