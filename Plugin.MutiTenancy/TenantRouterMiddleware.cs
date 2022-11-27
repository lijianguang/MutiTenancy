using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Plugin.MultiTenancy
{
    public class TenantRouterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public TenantRouterMiddleware(RequestDelegate next,
            ILogger<TenantRouterMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var _tenantContext = httpContext.RequestServices.GetRequiredService<TenantContext>();

            if(_tenantContext.Current != null)
            {
                PathString prefix = "/" + _tenantContext.Current.RequestUrlPrefix;
                httpContext.Request.PathBase += prefix;
                httpContext.Request.Path.StartsWithSegments(prefix, StringComparison.OrdinalIgnoreCase, out PathString remainingPath);
                httpContext.Request.Path = remainingPath;
            }

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    internal static class TenantRouterMiddlewareExtensions
    {
        internal static IApplicationBuilder UseTenantRouterMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantRouterMiddleware>();
        }
    }
}
