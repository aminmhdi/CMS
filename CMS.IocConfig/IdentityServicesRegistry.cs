using System;
using CMS.ServiceLayer.SchedulerTask;
using CMS.ViewModel.Settings;
using DNTScheduler.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CMS.IocConfig
{
    public static class IdentityServicesRegistry
    {
        /// <summary>
        /// Adds all of the ASP.NET Core Identity related services and configurations at once.
        /// </summary>
        public static void AddCustomIdentityServices(this IServiceCollection services)
        {
            var siteSettings = GetSiteSettings(services);

            services.AddConfiguredDbContext(siteSettings);
            services.AddCustomServices();

            services.AddSingleton(new ServiceLocator(services));

            services.AddDNTScheduler(options =>
            {
                options.AddScheduledTask<SchedulerTaskService>
                (
                    runAt: utcNow =>
                    {
                        const int intervalTime = 1;
                        var now = utcNow.AddHours(3.5);
                        return now.Minute % intervalTime == 0 && now.Second == 1;
                    },
                    order: 1
                );
                options.AddPingTask = false;
            });
        }

        public static AppSettings GetSiteSettings(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var siteSettingsOptions = provider.GetRequiredService<IOptionsSnapshot<AppSettings>>();
            var siteSettings = siteSettingsOptions.Value;
            if (siteSettings == null) throw new ArgumentNullException(nameof(siteSettings));
            return siteSettings;
        }
    }
}