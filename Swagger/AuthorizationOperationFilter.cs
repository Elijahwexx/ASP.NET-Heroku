using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace backend.Swagger
{
    public class AuthorizationOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var controllerAuth = context.ApiDescription.ControllerAttributes().OfType<AuthorizeAttribute>();
            var actionAuth = context.ApiDescription.ActionAttributes().OfType<AuthorizeAttribute>();
            var combinedAuth = controllerAuth.Union(actionAuth).Distinct();

            var controllerAnonymous = context.ApiDescription.ControllerAttributes().OfType<AllowAnonymousAttribute>();
            var actionAnonymous = context.ApiDescription.ActionAttributes().OfType<AllowAnonymousAttribute>();
            var combinedAnonymous = controllerAnonymous.Union(actionAnonymous).Distinct();

            if (combinedAuth.Any() && !combinedAnonymous.Any())
            {
                // if the operation does not have a response for unauthenticated, create a new one and add it
                if (!operation.Responses.TryGetValue("401", out Response response))
                {
                    response = new Response
                    {
                        Description = "The user is not authenticated",
                        Headers = new Dictionary<string, Header>
                        {
                            [Microsoft.Net.Http.Headers.HeaderNames.WWWAuthenticate] = new Header
                            {
                                Description = "The reason why authentication failed",
                                Type = "string"
                            }
                        }
                    };
                    operation.Responses["401"] = response;
                }
            }
        }
    }
}
