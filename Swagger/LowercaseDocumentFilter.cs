using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace backend.Swagger
{
    public class LowercaseDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths;
            var pathsList = paths.ToList();

            foreach(var path in pathsList)
            {
                if (!path.Key.All(c => char.IsLower(c)))
                {
                    paths.Remove(path.Key);
                    paths[path.Key.ToLower()] = path.Value;
                }
            }
        }
    }
}
