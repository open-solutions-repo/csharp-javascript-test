using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Web.Http.Description;

namespace CSharpJs.Test.Api.Infraestructure
{
    /// <summary>
    /// 
    /// </summary>
    public class AddRequiredHeaderParameter : IOperationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="schemaRegistry"></param>
        /// <param name="apiDescription"></param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
                operation.parameters = new List<Parameter>();

            operation.parameters.Add(new Parameter
            {
                name = "company",
                @in = "header",
                type = "string",
                required = true
            });

            operation.parameters.Add(new Parameter
            {
                name = "authorization",
                @in = "header",
                type = "string",
                required = false
            });

            operation.parameters.Add(new Parameter
            {
                name = "username",
                @in = "header",
                type = "string",
                required = false
            });
        }
    }
}
