using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CSharpJs.Test.Api.Repository;

namespace CSharpJs.Test.Api.Controllers
{
    /// <summary>
    /// Controller para módulo de Manutenção de Terminal
    /// </summary>
    [Authorize]
    [RoutePrefix("api/terminal")]
    public class TerminalController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public TerminalController(SqlRepository repository)
            : base(repository)
        {
        }
        /// <summary>
        /// Get para trazer informações de um terminal
        /// </summary>
        /// <param name="code"></param>
        /// <param name="top"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Read/{code}/{top}/{skip}")]
        public HttpResponseMessage GetTerminal([FromUri] int? code = null, [FromUri] int? top = null, [FromUri] int? skip = null)
        {
            try
            {
                var retorno = _repository.GetTerminal(code, top, skip);

                List<dynamic> values = new List<dynamic>();

                retorno.Values.ForEach(value =>
                {
                    var businessPlace = _repository.GetBusinessPlace(Convert.ToInt32(value.U_OPEN_BPLId), null, null);

                    values.Add(new { value.DocEntry, value.U_OPEN_TerminalCode, value.U_OPEN_BPLId, businessPlace.BPLName });
                });

                retorno.Values = values;

                return Request.CreateResponse(HttpStatusCode.OK, new { data = retorno });
            }
            catch (Exception ex)
            {
                Log.Error("{erro}", ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        /// <summary>
        /// Post para criar um novo terminal
        /// </summary>
        /// <param name="BranchesServiceCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create")]
        public HttpResponseMessage CreateTerminal([FromBody] List<int> BranchesServiceCode)
        {
            try
            {
                _repository.CreateTerminal(BranchesServiceCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { data = "ok" });
            }
            catch (Exception ex)
            {
                Log.Error("{erro}", ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        /// <summary>
        /// Delete para cancelar um terminal
        /// </summary>
        /// <param name="BranchCashierCode"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete")]
        public HttpResponseMessage CancelTerminal([FromBody] List<int> BranchCashierCode)
        {
            try
            {
                _repository.CancelTerminal(BranchCashierCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { data = "ok" });
            }
            catch (Exception ex)
            {
                Log.Error("{erro}", ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
