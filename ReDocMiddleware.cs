using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace backend
{
    public class ReDocMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TemplateMatcher _requestMatcher;
        private readonly string _swaggerPath;
        private readonly string _title;

        public ReDocMiddleware(RequestDelegate next, string path, string swaggerPath, string title)
        {
            _next = next;
            _requestMatcher = new TemplateMatcher(TemplateParser.Parse(path), new RouteValueDictionary());
            _swaggerPath = swaggerPath;
            _title = title;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!IsRequestingReDocUi(httpContext.Request))
            {
                await _next(httpContext);
                return;
            }

            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "text/html";
            var content = BuildResponseBodyAsync(httpContext.Request, _swaggerPath, _title);
            content.CopyTo(httpContext.Response.Body);
        }

        private bool IsRequestingReDocUi(HttpRequest request)
        {
            if (request.Method != "GET") return false;

            return _requestMatcher.TryMatch(request.Path, new RouteValueDictionary());
        }

        private static Stream BuildResponseBodyAsync(HttpRequest request, string swaggerPath, string title)
        {
            var swaggerUrl = new UriBuilder(
                request.Scheme,
                request.Host.Host,
                request.Host.Port ?? (request.IsHttps ? 443 : 80),
                swaggerPath).Uri.ToString();

            var body = "<!DOCTYPE html>"
                + "<html>"
                + "<head>"
                + $"<title>{title}</title>"
                + "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">"
                + "</head>"
                + "<body style=\"margin: 0;\">"
                + $"<redoc spec-url=\"{swaggerUrl}\" expand-responses=\"200,201\"></redoc>"
                + "<script src=\"https://rebilly.github.io/ReDoc/releases/latest/redoc.min.js\"></script>"
                + "</body>"
                + "</html>";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(body));
            return stream;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ReDocMiddlewareExtensions
    {
        public static IApplicationBuilder UseReDoc(
            this IApplicationBuilder builder,
            string path = "api-docs",
            string swaggerUrl = "/swagger/v1/swagger.json",
            string title = "API Docs")
        {
            builder.UseMiddleware<ReDocMiddleware>(path, swaggerUrl, title);
            return builder;
        }
    }
}
