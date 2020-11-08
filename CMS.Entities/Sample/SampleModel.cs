using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS.Entities.AuditableEntity;
using CMS.Entities.Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CMS.Entities.Sample
{
    [Table("Sample")]
    public class SampleModel : IAuditableEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int64)]
        public int Id { get; set; }

        [StringLength(50)]
        [BsonElement("Name")]
        public string Title { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Status Status { get; set; }
    }
}
