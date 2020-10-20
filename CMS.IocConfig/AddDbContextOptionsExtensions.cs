using System;
using CMS.DataLayer.Context;
using CMS.ServiceLayer.Contracts.Identity;
using CMS.ViewModel.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.IocConfig
{
    public static class AddDbContextOptionsExtensions
    {
        public static IServiceCollection AddConfiguredDbContext
        (
            this IServiceCollection serviceCollection, 
            AppSettings appSettings
        )
        {
            switch (appSettings.ActiveDatabase)
            {
                case ActiveDatabase.MongoDb:
                    serviceCollection.AddConfiguredMongoDbContext(appSettings);
                    break;

                default:
                    throw new NotSupportedException("Please set the ActiveDatabase in appsettings.json file.");
            }

            return serviceCollection;
        }

        /// <summary>
        /// Creates and seeds the database.
        /// </summary>
        public static void InitializeDb(this IServiceProvider serviceProvider)
        {
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var identityDbInitialize = scope.ServiceProvider.GetRequiredService<IIdentityDbInitializer>();
            identityDbInitialize.Initialize();
            identityDbInitialize.SeedData();
        }
    }
}