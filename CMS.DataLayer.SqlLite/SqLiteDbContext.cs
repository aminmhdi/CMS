using CMS.Common.EFCoreToolkit;
using CMS.DataLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace CMS.DataLayer.Sqlite
{
    public class SqLiteDbContext : ApplicationDbContext
    {
        public SqLiteDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.AddDateTimeOffsetConverter();
        }
    }
}