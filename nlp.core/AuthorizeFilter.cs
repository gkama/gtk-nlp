using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using nlp.data;
using nlp.data.text;
using System.Text;

namespace nlp.core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute()
            : base(typeof(AuthorizeFilter))
        {
        }
    }

    public class AuthorizeFilter : IAuthorizationFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public AuthorizeFilter(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(env));
            _configuration = configuration ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(configuration));
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (_env.IsDevelopment())
                return;

            string authHeader = context.HttpContext.Request.Headers["Authorization"];

            if (authHeader != null
                && authHeader.StartsWith("Basic "))
            {
                var encodedwriteKey = authHeader.Substring("Basic ".Length).Trim();
                var writeKey = Encoding.UTF8.GetString(Convert.FromBase64String(encodedwriteKey)).TrimEnd(':');
                
                if (writeKey == _configuration["WriteKey"])
                    return;
            }

            context.HttpContext.Response.Headers["WWW-Authenticate"] = "Basic";
            context.Result = new UnauthorizedResult();
        }
    }
}
