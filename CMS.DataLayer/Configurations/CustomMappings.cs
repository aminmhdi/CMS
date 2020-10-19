using CMS.ViewModel.Settings;
using Microsoft.EntityFrameworkCore;

namespace CMS.DataLayer.Mappings
{
    public static class CustomMappings
    {
        /// <summary>
        /// Adds all of the ASP.NET Core Identity related mappings at once.
        /// More info: http://www.dotnettips.info/post/2577
        /// and http://www.dotnettips.info/post/2578
        /// </summary>
        public static void AddCustomMappings(this ModelBuilder modelBuilder, AppSettings appSettings)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomMappings).Assembly);

            // IEntityTypeConfiguration's which have constructors with parameters
            //modelBuilder.ApplyConfiguration(new AppSqlCacheConfiguration(appSettings));
        }
    }
}