using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KTZSanatoriumServiceApi.Models
{
    public class Base
    {
        
        [BsonId]
        public string Id { get; set; } = Guid.NewGuid().ToString();
      //  [BsonElement("is_deleted")]
        public bool IsDeleted { get; set; }

        [BsonElement("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;


    }
}
