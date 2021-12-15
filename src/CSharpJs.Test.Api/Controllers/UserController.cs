using Serilog;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CSharpJs.Test.Api.Repository;

namespace CSharpJs.Test.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UserController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public UserController(SqlRepository repository)
            : base(repository)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("userpermissions/{user}")]
        public HttpResponseMessage UserPermissions([FromUri] string user)
        {
            try
            {
                var ret = _repository.GetAllowed(user);
                return Request.CreateResponse(HttpStatusCode.OK, new { data = ret });
            }
            catch (Exception ex)
            {
                Log.Error("{erro}", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("UserBranchAssignment/{user}")]
        public HttpResponseMessage GetUserBranchAssignment([FromUri] string user)
        {
            try
            {
                var ret = _repository.GetUserBranchAssignment(user);
                return Request.CreateResponse(HttpStatusCode.OK, new { data = ret });
            }
            catch (Exception ex)
            {
                Log.Error("{erro}", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("validToken/{user}")]
        public HttpResponseMessage ValidToken([FromUri] string user)
        {
            try
            {
                var ret = _repository.IsFirstAcess(user);
                return Request.CreateResponse(HttpStatusCode.OK, ret);
            }
            catch (Exception ex)
            {
                Log.Error("{erro}", ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Retorna a a permissão de um usuário referente a uma autorização.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="authorizationId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("userpermission/{user}/{authorizationId}")]
        public HttpResponseMessage GetUserPermission([FromUri] string user, string authorizationId)
        {
            try
            {
                var ret = _repository.GetUserPermission(user, authorizationId);
                return Request.CreateResponse(HttpStatusCode.OK, ret);
            }
            catch (Exception ex)
            {
                Log.Error("{erro}", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}
