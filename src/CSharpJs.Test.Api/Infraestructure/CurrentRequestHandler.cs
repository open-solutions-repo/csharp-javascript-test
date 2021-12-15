using Serilog;
using Serilog.Context;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpJs.Test.Api.Infraestructure
{
    /// <summary>
    /// 
    /// </summary>
    public class CurrentRequestHandler : DelegatingHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            try
            {
                using (LogContext.PushProperty("CorrelationId", request.GetCorrelationId()))
                {
                    var scope = request.GetDependencyScope();
                    var currentRequest = (CurrentRequest)scope.GetService(typeof(CurrentRequest));
                    currentRequest.RequestData = request;
                    currentRequest.RequestId = Guid.NewGuid();
                    var threadId = Thread.CurrentThread.ManagedThreadId;

                    if (string.IsNullOrWhiteSpace(currentRequest.CompanyKey)) throw new Exception("Company null hostname");

                    Log.Information("Thread {0} {1} {2} started at {3}.", threadId
                        , request.Method, request.RequestUri, DateTime.Now.ToLongTimeString());

                    var sw = new Stopwatch();
                    sw.Start();
                    var response = await base.SendAsync(request, cancellationToken);
                    sw.Stop();

                    Log.Information("Thread {0} {1} {2} ended at {3} with {4} ms.", threadId
                        , request.Method, request.RequestUri, DateTime.Now.ToLongTimeString(), sw.ElapsedMilliseconds);

                    return response;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{request.GetCorrelationId()} {ex.Message}\n{ex.StackTrace}");
                return request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Por favor, refaça a operação");
            }
        }
    }
}
