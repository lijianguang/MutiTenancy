using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Plugin.MultiTenancy
{
    public class TenantContainerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IOptions<Tenants> _tenants;
        private readonly ITenantContainer _tenantContainer;
        //private readonly TenantContext _tenantContext;

        public TenantContainerMiddleware(RequestDelegate next, 
            ILogger<TenantContainerMiddleware> logger,
            IOptions<Tenants> tenants,
            ITenantContainer tenantContainer)
        {
            _next = next;
            _logger = logger;
            _tenants = tenants;
            _tenantContainer = tenantContainer;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var _tenantContext = httpContext.RequestServices.GetRequiredService<TenantContext>();

            _tenantContainer.Initialize(_tenants.Value.Items);

            _tenantContext.SetTenantContext(_tenantContainer.Match(httpContext.Request.Host, httpContext.Request.Path));

            httpContext.Features.Set(new TenantContextFeature
            {
                TenantContext = _tenantContext.Current,
                OriginalPath = httpContext.Request.Path,
                OriginalPathBase = httpContext.Request.PathBase
            });

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    internal static class TenantContainerMiddlewareExtensions
    {
        internal static IApplicationBuilder UseTenantContainerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantContainerMiddleware>();
        }
    }
}
