﻿using CMS.Entities.Common.Enums;
using CMS.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.DataLayer.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("AppRoles");
            builder.Property(p => p.Status).HasDefaultValue(Status.Active);
        }
    }
}
