using System;
using System.ComponentModel.DataAnnotations;
using CMS.Entities.AuditableEntity;
using CMS.Entities.Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CMS.Entities.Sample
{
    public class Sample : IAuditableEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }

        [StringLength(50)]
        [BsonElement("Name")]
        public string Title { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Status Status { get; set; }
    }
}
