using System;
using CMS.Entities.Common.Enums;

namespace CMS.Entities.AuditableEntity
{
    public interface IAuditableEntity
    {
        DateTime CreateDate { get; set; }
        DateTime? UpdateDate { get; set; }
        Status Status { get; set; }
    }
}