using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace backend.Swagger
{
    public class BadRequestOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            // if the operation does not have a response for unauthenticated, create a new one and add it
            if (!operation.Responses.TryGetValue("400", out Response response))
            {
                response = (operation.Responses["400"] = new Response());
            }

            // set the description
            if (string.IsNullOrWhiteSpace(response.Description))
            {
                response.Description = "The request is invalid, see response for more details.";
            }
        }
    }
}
