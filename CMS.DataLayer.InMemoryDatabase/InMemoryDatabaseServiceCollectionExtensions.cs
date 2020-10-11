using System;
using CMS.Common.PersianToolkit;
using CMS.DataLayer.Context;
using CMS.ViewModel.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.DataLayer.InMemoryDatabase
{
    public static class InMemoryDatabaseServiceCollectionExtensions
    {
        public static IServiceCollection AddConfiguredInMemoryDbContext
        (
            this IServiceCollection services, 
            AppSettings appSettings
        )
        {
            services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<ApplicationDbContext>());
            services.AddEntityFrameworkInMemoryDatabase(); // It's added to access services from the dbcontext, remove it if you are using the normal `AddDbContext` and normal constructor dependency injection.
            services.AddDbContextPool<ApplicationDbContext, InMemoryDatabaseContext>
            (
                (serviceProvider, optionsBuilder) =>
                    optionsBuilder.UseConfiguredInMemoryDatabase(appSettings, serviceProvider)
            );
            return services;
        }

        public static void UseConfiguredInMemoryDatabase
        (
            this DbContextOptionsBuilder optionsBuilder, 
            AppSettings appSettings, 
            IServiceProvider serviceProvider
        )
        {
            optionsBuilder.UseInMemoryDatabase(appSettings.ConnectionStrings.LocalDb.InitialCatalog);
            optionsBuilder.UseInternalServiceProvider(serviceProvider); // It's added to access services from the dbcontext, remove it if you are using the normal `AddDbContext` and normal constructor dependency injection.
            optionsBuilder.AddInterceptors(new PersianYeKeCommandInterceptor());
            optionsBuilder.ConfigureWarnings(warnings =>
            {
                // ...
            });
        }
    }
}