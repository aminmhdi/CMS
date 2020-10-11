using System.Collections.Generic;
using System.Globalization;
using CMS.ServiceLayer.Contracts.Resources;
using CMS.ServiceLayer.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.IocConfig
{
    public static class AddLanguageExtensions
    {
        public static IServiceCollection AddLanguageServices(this IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("fa-IR"),
                        new CultureInfo("en-US"),
                        new CultureInfo("ar")
                    };

                    opts.DefaultRequestCulture = new RequestCulture("fa-IR");
                    opts.SupportedCultures = supportedCultures;
                    opts.SupportedUICultures = supportedCultures;
                });

            services.AddLocalization(o => o.ResourcesPath = "Resources");
            services.AddSingleton(typeof(ISharedResource), typeof(SharedResource));

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,
                    opts => opts.ResourcesPath = "Resources")
                .AddDataAnnotationsLocalization();

            return services;
        }
    }
}