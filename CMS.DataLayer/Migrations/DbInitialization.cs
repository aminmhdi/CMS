using CMS.DataLayer.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.DataLayer.Migrations
{
    public static class DbInitialization
    {
        public static void MigrateDatabase(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            // Applies any pending migrations for the context to the database.
            // Will create the database if it does not already exist.
            context.Database.Migrate();
        }
    }
}