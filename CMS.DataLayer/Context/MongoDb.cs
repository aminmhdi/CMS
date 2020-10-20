using CMS.ViewModel.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.DataLayer.Context
{
    public static class AddMongoDbServiceCollectionExtensions
    {
        public static IServiceCollection AddConfiguredMongoDbContext(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddSingleton<IMongoDbContext, MongoDbContext>(provider => new MongoDbContext(appSettings));
            return services;
        }
    }
}
