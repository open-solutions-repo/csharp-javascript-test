using CSharpJs.Test.Api.Repository;
using Serilog;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CSharpJs.Test.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Operator")]
    public class OperadorController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public OperadorController(SqlRepository repository)
            : base(repository)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Validate/{code}")]
        public HttpResponseMessage Get([FromUri] string code)
        {
            try
            {
                var retorno = _repository.ValidateCollaboratorPosition(code);
                return Request.CreateResponse(HttpStatusCode.OK, new { data = retorno });
            }
            catch (Exception ex)
            {
                Log.Error("{erro}", ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("Sellers")]
        public HttpResponseMessage GetSellers()
        {
            try
            {
                var retorno = _repository.GetSellers();
                return Request.CreateResponse(HttpStatusCode.OK, new { data = retorno });
            }
            catch (Exception ex)
            {
                Log.Error("{erro}", ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
