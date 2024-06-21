using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace KTZSanatoriumServiceApi.Models
{
    public class Sanatory : Base
    {
        [BsonElement("name")]
        public required string Name { get; set; } = string.Empty;

        [BsonElement("address")]
        public AddressInfo? Address { get; set; }

        [BsonElement("organization_name")]
        public string OrganizationName { get; set; } = string.Empty;

        [BsonElement("payment")]
        public Payment? Payment { get; set; }

    }
}
