using CMS.Entities.AuditableEntity;
using Microsoft.AspNetCore.Identity;

namespace CMS.Entities.Identity
{
    
    public class RoleClaim : IdentityRoleClaim<int>, IAuditableEntity
    {
        public virtual Role Role { get; set; }
    }
}