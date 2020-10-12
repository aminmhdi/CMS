using CMS.Entities.Common.Enums;
using CMS.Entities.Sample;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.DataLayer.Configurations
{
    public class SampleConfiguration : IEntityTypeConfiguration<Sample>
    {
        public void Configure(EntityTypeBuilder<Sample> builder)
        {
            builder.ToTable("Sample");
            builder.Property(p => p.Status).HasDefaultValue(Status.Active);
        }
    }
}
