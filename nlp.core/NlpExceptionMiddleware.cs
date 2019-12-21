using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

using nlp.data;

namespace nlp.core
{
    public class NlpExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _log;
        private readonly IWebHostEnvironment _env;

        private const string default_message = "An unexpected error has occurred.";

        public NlpExceptionMiddleware(RequestDelegate next, ILogger<NlpExceptionMiddleware> log, IWebHostEnvironment env)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                if (httpContext.Response.HasStarted)
                {
                    _log.LogWarning("The response has already started, the http status code middleware will not be executed.");
                    throw;
                }

                var id = string.IsNullOrEmpty(httpContext?.TraceIdentifier)
                    ? Guid.NewGuid().ToString()
                    : httpContext.TraceIdentifier;

                _log.LogError(e, "an exception was thrown during the request. {exceptionId}", id);

                await WriteExceptionResponseAsync(httpContext, e, id);
            }
        }

        private async Task WriteExceptionResponseAsync(HttpContext httpContext, Exception e, string id)
        {
            var canViewSensitiveInfo = _env
                .IsDevelopment();

            var problem = new ProblemDetails()
            {
                Title = canViewSensitiveInfo
                    ? e.Message
                    : default_message,
                Detail = canViewSensitiveInfo
                    ? e.Demystify().ToString()
                    : null,
                Instance = $"survey:error:{id}"
            };

            if (e is NlpException ne)
                problem.Status = ne.StatusCode;
            else
                problem.Status = StatusCodes.Status500InternalServerError;

            var problemjson = JsonSerializer
                .Serialize(problem);

            httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsync(problemjson);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class HttpStatusCodeExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseNlpException(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NlpExceptionMiddleware>();
        }
    }
}
