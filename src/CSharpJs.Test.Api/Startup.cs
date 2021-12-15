using Microsoft.AspNetCore.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Owin;
using Swashbuckle.Application;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Topshelf.WebApi.Lamar;
using CSharpJs.Test.Api.Infraestructure;
using CSharpJs.Test.Api.Providers;

namespace CSharpJs.Test.Api
{
    class Startup
    {
        private HttpConfiguration HttpConfiguration()
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            config.MessageHandlers.Insert(0, new CurrentRequestHandler());            
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                                "DefaultApiWithId"
                            ,   "api/{controller}/{id}"
                            ,   new { id = RouteParameter.Optional }
                            ,   new { id = @"\d+" }
                        );

            var appXmlType = config.Formatters
                                   .XmlFormatter
                                   .SupportedMediaTypes
                                   .FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.EnableSwagger(c =>
            {
                c.IgnoreObsoleteActions();
                c.SingleApiVersion("v1", "Integração SAP Business One")
                 .Description("Esta Api provem recursos e métodos para criação de documentos no SAP Business One e Informações sobre processos para operação");
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Swagger.xml"));
                c.DescribeAllEnumsAsStrings();
                c.OperationFilter<AddRequiredHeaderParameter>();
            }).EnableSwaggerUi(c =>
            {
                c.DisableValidator();
                c.SupportedSubmitMethods(new string[] { "GET", "POST" });
            });

            // config.Services.Replace(typeof(IExceptionHandler), new WebApiExceptionHandler());

            config.UseLamarDependencyResolver();

            return config;
        }

        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = HttpConfiguration();

            /////////
            OAuthAuthorizationServerOptions oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new Microsoft.Owin.PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new ApplicationOAuthProvider()
            };
            //Habilita o Cors
            appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            // Geração do token
            appBuilder.UseOAuthAuthorizationServer(oAuthServerOptions);
            appBuilder.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
            
            //appBuilder.UseLoggerRequest();
            appBuilder.UseWebApi(config);
        }
    }
}