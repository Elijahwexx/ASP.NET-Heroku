using backend.Swagger;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using backend;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Swagger
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            // Inject an implementation of ISwaggerProvider with defaulted settings applied
            services.AddSwaggerGen(options =>
            {
                // at the moment, there is only one version of the API
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "API Test 1.0",
                    Description = File.ReadAllText("apidesc.md"),
                });

                // specify the authentication scheme
                options.AddSecurityDefinition("Bearer Token", new ApiKeyScheme
                {
                    In = "Header",
                    Name = "Authorization",
                    Description = "JWT Bearer token obtained from the authentication service"
                });

                // Display routes in lower case
                options.DocumentFilter<LowercaseDocumentFilter>();

                // apply common response filters
                options.OperationFilter<AuthorizationOperationFilter>();
                options.OperationFilter<BadRequestOperationFilter>();

                var app = PlatformAbstractions.PlatformServices.Default.Application;
                options.IncludeXmlComments(Path.Combine(app.ApplicationBasePath, app.ApplicationName + ".xml"));
            });

            return services;
        }
    }
}
