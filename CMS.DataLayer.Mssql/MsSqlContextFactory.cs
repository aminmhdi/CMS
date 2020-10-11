using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using CMS.DataLayer.Context;
using CMS.DataLayer.Mssql;
using CMS.ViewModel.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ORS.DataLayer.MSSQL
{
    public class MsSqlContextFactory : IDesignTimeDbContextFactory<MsSqlDbContext>
    {
        public MsSqlDbContext CreateDbContext(string[] args)
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
            services.AddSingleton(provider => configuration);
            services.Configure<AppSettings>(options => configuration.Bind(options));

            var siteSettings = services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<AppSettings>>();
			siteSettings.Value.ActiveDatabase = ActiveDatabase.LocalDb;

            services.AddEntityFrameworkSqlServer(); // It's added to access services from the dbcontext, remove it if you are using the normal `AddDbContext` and normal constructor dependency injection.
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseConfiguredMsSql(siteSettings.Value, services.BuildServiceProvider());

            return new MsSqlDbContext(optionsBuilder.Options);
        }
    }
}