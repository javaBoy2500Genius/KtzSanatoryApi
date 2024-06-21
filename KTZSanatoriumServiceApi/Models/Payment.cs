using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KTZSanatoriumServiceApi.Models
{
   public class Payment
    {
        [BsonElement("name")]
        public string Name { get; set; }=string.Empty;

        [BsonElement("sum")]
        public double Sum { get; set; }


        [BsonElement("tax")]
        public Tax? Tax { get; set; }
    }
}
