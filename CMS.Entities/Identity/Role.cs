using System.Collections.Generic;
using CMS.Entities.AuditableEntity;
using Microsoft.AspNetCore.Identity;

namespace CMS.Entities.Identity
{
    
    public class Role : IdentityRole<int>, IAuditableEntity
    {
        public Role()
        {
        }

        public Role(string name) : this()
        {
            Name = name;
        }

        public Role(string name, string description) : this(name)
        {
            Description = description;
        }

        public string Description { get; set; }

        public virtual ICollection<UserRole> Users { get; set; }

        public virtual ICollection<RoleClaim> Claims { get; set; }
    }
}