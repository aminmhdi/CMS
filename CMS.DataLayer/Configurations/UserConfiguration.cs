using CMS.Entities.Common.Enums;
using CMS.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.DataLayer.Mappings
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("AppUsers");
            builder.Property(p => p.Status).HasDefaultValue(Status.Active);
        }
    }
}