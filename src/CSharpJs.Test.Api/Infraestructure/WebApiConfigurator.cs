using Microsoft.Owin.Hosting;
using System;
using Topshelf.Logging;
using CSharpJs.Test.Api;

namespace Topshelf.WebApi
{
    /// <summary>
    /// Configuração da Webapi
    /// </summary>
    public class WebApiConfigurator
    {
        private readonly LogWriter log = HostLogger.Get(typeof(WebApiConfigurator));
        private IDisposable _webApp;
        private string _url;

        /// <summary>
        /// Protocolo Http, Https
        /// </summary>
        public string Scheme { get; set; }
        /// <summary>
        /// Host
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// Porta
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public WebApiConfigurator()
        {
            Scheme = "http";
            Domain = "localhost";
            Port = 8080;
        }

        /// <summary>
        /// Inclui configurações para acesso via local host, http, para a porta 8080 por default
        /// </summary>
        /// <param name="port">Uma parte diferente de 8080 (Opcional)</param>
        /// <returns></returns>
        public WebApiConfigurator OnLocalhost(int port = 8080)
        {
            return OnHost("http", "localhost", port);
        }

        /// <summary>
        /// Configura o endpoint para a webapi
        /// </summary>
        /// <param name="scheme">Protocolo de conexão Http ou Https</param>
        /// <param name="domain">Host</param>
        /// <param name="port">Porta</param>
        /// <returns></returns>
        public WebApiConfigurator OnHost(string scheme = null, string domain = null, int port = 8080)
        {
            Scheme = !string.IsNullOrEmpty(scheme) ? scheme : Scheme;
            Domain = !string.IsNullOrEmpty(domain) ? domain : Domain;
            Port = port;

            return this;
        }

        /// <summary>
        /// Configura o endpoint fornecendo uma url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public WebApiConfigurator ByUrl(string url)
        {
            _url = url;
            return this;
        }

        /// <summary>
        /// Inicaliza o servidor da webapi
        /// </summary>
        public virtual void Initialize()
        {
            var url = BuildUrl();
             _webApp = WebApp.Start<Startup>(url);
            log.Debug(string.Format("[Topshelf.WebApi] Configuring WebAPI Selfhost for URI: {0}", url));
        }

        private string BuildUrl()
        {
            return _url ?? (new UriBuilder(Scheme, Domain, Port).Uri).ToString();
        }

        /// <summary>
        /// Termina o servidor da webapi
        /// </summary>
        public virtual void Shutdown()
        {
            log.Debug("[Topshelf.WebApi] Shutdown WebAPI Selfhost");
            _webApp.Dispose();
        }
    }
}