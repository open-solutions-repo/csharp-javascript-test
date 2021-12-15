using System.Web.Http;
using CSharpJs.Test.Api.Repository;

namespace CSharpJs.Test.Api.Controllers
{
    public abstract class BaseController : ApiController
    {
        protected readonly SqlRepository _repository;
        public BaseController(SqlRepository repository)
        {
            _repository = repository;
        }
    }
}