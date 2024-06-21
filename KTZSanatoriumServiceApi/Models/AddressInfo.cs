using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KTZSanatoriumServiceApi.Models
{
    public class AddressInfo
    {
        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("Region")]

        public string Region { get; set; } = string.Empty;

        [BsonElement("postal_code")]
        public string PostalCode { get; set; } = string.Empty;

        [BsonElement("country")]
        public string Country { get; set; } = string.Empty; 
    }
}
