using System;
using System.IO;
using CMS.Common.PersianToolkit;
using CMS.Common.WebToolkit;
using CMS.DataLayer.Context;
using CMS.ViewModel.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.DataLayer.Mssql
{
    public static class MsSqlServiceCollectionExtensions
    {
        public static IServiceCollection AddConfiguredMsSqlDbContext
        (
            this IServiceCollection services,
            AppSettings appSettings
        )
        {
            services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<ApplicationDbContext>());
            services.AddEntityFrameworkSqlServer(); // It's added to access services from the dbcontext, remove it if you are using the normal `AddDbContext` and normal constructor dependency injection.
            services.AddDbContextPool<ApplicationDbContext, MsSqlDbContext>
            (
                (serviceProvider, optionsBuilder) => optionsBuilder.UseConfiguredMsSql(appSettings, serviceProvider)
            );
            return services;
        }

        public static void UseConfiguredMsSql
        (
            this DbContextOptionsBuilder optionsBuilder, 
            AppSettings appSettings, 
            IServiceProvider serviceProvider
            )
        {
            var connectionString = appSettings.GetMsSqlDbConnectionString();
            optionsBuilder.UseSqlServer(
                        connectionString,
                        sqlServerOptionsBuilder =>
                        {
                            sqlServerOptionsBuilder.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds);
                            sqlServerOptionsBuilder.EnableRetryOnFailure();
                            sqlServerOptionsBuilder.MigrationsAssembly(typeof(MsSqlServiceCollectionExtensions).Assembly.FullName);
                        });
            optionsBuilder.UseInternalServiceProvider(serviceProvider); // It's added to access services from the dbcontext, remove it if you are using the normal `AddDbContext` and normal constructor dependency injection.
            optionsBuilder.AddInterceptors(new PersianYeKeCommandInterceptor());
            optionsBuilder.ConfigureWarnings(warnings =>
            {
               //warnings.Throw(RelationalEventId.QueryClientEvaluationWarning);
            });
        }

        public static string GetMsSqlDbConnectionString(this AppSettings appSettingsValue)
        {
            if (appSettingsValue == null)
            {
                throw new ArgumentNullException(nameof(appSettingsValue));
            }

            switch (appSettingsValue.ActiveDatabase)
            {
                case ActiveDatabase.SqlServer:
                    return appSettingsValue.ConnectionStrings.SqlServer.ApplicationDbContextConnection;

                case ActiveDatabase.MongoDb:
                    return appSettingsValue.ConnectionStrings.MongoDb.Connection;

                default:
                    throw new NotSupportedException("Please set the ActiveDatabase in appsettings.json file to `LocalDb` or `SqlServer`.");
            }
        }
    }
}