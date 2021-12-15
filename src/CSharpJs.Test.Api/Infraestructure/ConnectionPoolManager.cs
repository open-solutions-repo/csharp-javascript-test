using System;
using System.Collections.Concurrent;
using System.Linq;

namespace CSharpJs.Test.Api.Infraestructure
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionPoolManager: IDisposable
    {
        private ConcurrentDictionary<string, Connection> _connectionStore = new ConcurrentDictionary<string, Connection>();
      
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            foreach (var item in _connectionStore)
            {
                item.Value.Close();
            }
        }

        /// <summary>
        /// Obterm uma conexão com o SAP Business One
        /// </summary>
        /// <param name="companyKey">Nome da chave de configuração</param>
        /// <returns>Uma conexão com SAP Business One</returns>
        public Connection GetConnection(string companyKey)
        {
            if (string.IsNullOrWhiteSpace(companyKey) || companyKey.ToUpper().Equals("COMMON") ) return null;

            if(_connectionStore.ContainsKey(companyKey))
                return _connectionStore[companyKey];

            if(!(from s in Connection.Configurations()
                    where s.Key == companyKey
                    select s).Any())
                throw new Exception($"Não há configurações para 'company':{companyKey}");

            return _connectionStore.GetOrAdd(companyKey, new Connection(companyKey));
        }
    }
}
