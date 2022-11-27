using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.MultiTenancy
{
    public static class MultiTenancyConfiguration
    {
        public static void AddMutiTenancyServices(this IServiceCollection services, IConfiguration configuration)
        {
            //add services here to the DI system of ASP.NET Core
            services.AddSingleton<ITenantContainer, TenantContainer>();
            services.AddScoped<TenantContext>();
            services.Configure<Tenants>(configuration.GetSection(nameof(Tenants)));
        }

        public static void UseMutiTenancy(this IApplicationBuilder builder)
        {
            //add middleware to the pipeline being built with the builder

            builder.UseTenantContainerMiddleware();

            builder.UseTenantRouterMiddleware();
        }
    }
}
