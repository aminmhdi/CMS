using System;
using System.IO;
using CMS.DataLayer.Context;
using CMS.ViewModel.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CMS.DataLayer.Sqlite
{
    public class SqLiteContextFactory : IDesignTimeDbContextFactory<SqLiteDbContext>
    {
        public SqLiteDbContext CreateDbContext(string[] args)
        {
            var services = new ServiceCollection();
            services.AddOptions();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();

            var basePath = Directory.GetCurrentDirectory();
            Console.WriteLine($"Using `{basePath}` as the ContentRootPath");
            var configuration = new ConfigurationBuilder()
                                .SetBasePath(basePath)
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                .Build();
            services.AddSingleton<IConfigurationRoot>(provider => configuration);
            services.Configure<AppSettings>(options => configuration.Bind(options));

            var appSettings = services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<AppSettings>>();
			appSettings.Value.ActiveDatabase = ActiveDatabase.SqLite;

            services.AddEntityFrameworkSqlite(); // It's added to access services from the dbcontext, remove it if you are using the normal `AddDbContext` and normal constructor dependency injection.
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseConfiguredSqLite(appSettings.Value, services.BuildServiceProvider());

            return new SqLiteDbContext(optionsBuilder.Options);
        }
    }
}