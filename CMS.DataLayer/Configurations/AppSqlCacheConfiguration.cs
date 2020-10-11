using CMS.Entities.Identity;
using CMS.ViewModel.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.DataLayer.Mappings
{
    public class AppSqlCacheConfiguration : IEntityTypeConfiguration<AppSqlCache>
    {
        private readonly AppSettings _appSettings;

        public AppSqlCacheConfiguration(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public void Configure(EntityTypeBuilder<AppSqlCache> builder)
        {
            // For Microsoft.Extensions.Caching.SqlServer
            var cacheOptions = _appSettings.CookieOptions.DistributedSqlServerCacheOptions;
            builder.ToTable(cacheOptions.TableName, cacheOptions.SchemaName);
            builder.HasIndex(e => e.ExpiresAtTime).HasName("Index_ExpiresAtTime");
            builder.Property(e => e.Id).HasMaxLength(449);
            builder.Property(e => e.Value).IsRequired();
        }
    }
}