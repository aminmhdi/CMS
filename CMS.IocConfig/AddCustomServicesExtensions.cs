using System.Security.Claims;
using System.Security.Principal;
using CMS.ServiceLayer.Contracts.Identity;
using CMS.ServiceLayer.Contracts.Sample;
using CMS.ServiceLayer.Identity;
using CMS.ServiceLayer.Sample;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.IocConfig
{
    public static class AddCustomServicesExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IPrincipal>(provider => provider.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.User ?? ClaimsPrincipal.Current);

            services.AddScoped<IIdentityDbInitializer, IdentityDbInitializer>();

            services.AddScoped<ISampleService, SampleService>();

            return services;
        }
    }
}