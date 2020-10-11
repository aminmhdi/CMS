using System;
using CMS.Common.PersianToolkit;
using CMS.Common.WebToolkit;
using CMS.DataLayer.Context;
using CMS.ViewModel.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.DataLayer.Sqlite
{
    public static class SqLiteServiceCollectionExtensions
    {
        public static IServiceCollection AddConfiguredSqLiteDbContext
        (
            this IServiceCollection services,
            AppSettings appSettings
        )
        {
            services.AddScoped<IUnitOfWork>(serviceProvider =>
                serviceProvider.GetRequiredService<ApplicationDbContext>());
            services.AddEntityFrameworkSqlite(); // It's added to access services from the dbcontext, remove it if you are using the normal `AddDbContext` and normal constructor dependency injection.
            services.AddDbContextPool<ApplicationDbContext, SqLiteDbContext>
            (
                (serviceProvider, optionsBuilder) => optionsBuilder.UseConfiguredSqLite(appSettings, serviceProvider)
            );
            return services;
        }

        public static void UseConfiguredSqLite
        (
            this DbContextOptionsBuilder optionsBuilder, 
            AppSettings appSettings, 
            IServiceProvider serviceProvider)
        {
            var connectionString = appSettings.GetSQLiteDbConnectionString();
            optionsBuilder.UseSqlite
            (
                connectionString,
                sqlServerOptionsBuilder =>
                {
                    sqlServerOptionsBuilder.CommandTimeout((int) TimeSpan.FromMinutes(3).TotalSeconds);
                    sqlServerOptionsBuilder.MigrationsAssembly(typeof(SqLiteServiceCollectionExtensions).Assembly
                        .FullName);
                }
            );
            optionsBuilder.UseInternalServiceProvider(serviceProvider); // It's added to access services from the dbcontext, remove it if you are using the normal `AddDbContext` and normal constructor dependency injection.
            optionsBuilder.AddInterceptors(new PersianYeKeCommandInterceptor());
            optionsBuilder.ConfigureWarnings(warnings =>
            {
                // ...
            });
        }

        public static string GetSQLiteDbConnectionString(this AppSettings appSettingsValue)
        {
            if (appSettingsValue == null)
            {
                throw new ArgumentNullException(nameof(appSettingsValue));
            }

            switch (appSettingsValue.ActiveDatabase)
            {
                case ActiveDatabase.SqLite:
                    return appSettingsValue.ConnectionStrings
                        .SqLite
                        .ApplicationDbContextConnection
                        .ReplaceDataDirectoryInConnectionString();

                default:
                    throw new NotSupportedException(
                        "Please set the ActiveDatabase in appsettings.json file to `SQLite`.");
            }
        }

        public static string ReplaceDataDirectoryInConnectionString(this string connectionString)
        {
            return connectionString.Replace("|DataDirectory|", ServerInfo.GetAppDataFolderPath());
        }
    }
}