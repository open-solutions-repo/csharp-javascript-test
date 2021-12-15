using CSharpJs.Test.Api.Infraestructure;
using Lamar;
using Serilog;
using System;
using System.Configuration;
using System.IO;
using Topshelf;
using Topshelf.Lamar;
using Topshelf.WebApi;

namespace CSharpJs.Test.Api
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss.fff zzz} {Level:u3}]{CorrelationId} {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "logs\\service-log-.txt")
                    , outputTemplate: "[{Timestamp:HH:mm:ss.fff zzz} {Level:u3}]{CorrelationId} {Message:lj}{NewLine}{Exception}"
                    , rollingInterval: RollingInterval.Day
                    , retainedFileCountLimit: 14)
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                var container = new Container(config =>
                {
                    config.Scan(scanner =>
                    {
                        scanner.TheCallingAssembly();
                        scanner.LookForRegistries();
                    });
                });

                //Inicia o container de dependencia para a Connection
                Connection.Init(container);

                HostFactory.Run(c =>
                {
                    c.UseLamar(container);

                    c.UseSerilog();

                    c.Service<IStore5Service>(s =>
                    {
                        s.ConstructUsingLamar();
                        s.WhenStarted((service, control) => service.Start());
                        s.WhenStopped((service, control) => service.Stop());

                        s.WebApiEndpoint(api => api.ByUrl(ConfigurationManager.AppSettings["url"]));
                    });
                    c.SetServiceName("CSharpJsTest");
                    c.SetDisplayName("C# + Javascript Test");
                    c.SetDescription("Esta Api provem recursos e métodos para criação de documentos no SAP Business One e Informações sobre processos para operação");
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
