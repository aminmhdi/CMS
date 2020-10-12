using System.ComponentModel.DataAnnotations;
using CMS.Entities.AuditableEntity;
using CMS.Entities.Common.Enums;

namespace CMS.Entities.Sample
{
    public class Sample : IAuditableEntity
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        public Status Status { get; set; }
    }
}
