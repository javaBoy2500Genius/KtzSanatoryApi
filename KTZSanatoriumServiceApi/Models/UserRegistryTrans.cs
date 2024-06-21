using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KTZSanatoriumServiceApi.Models
{
    public class UserRegistryTrans : Base
    {

        [BsonElement("iin")]
        public string IIN { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("midle_name")]
        public string MiddleName { get; set; } = string.Empty;

        [BsonElement("surname")]
        public string SurName { get; set; } = string.Empty;

        [BsonElement("personal_number")]
        public string PersonalNumber { get; set; } = string.Empty;

        [BsonElement("branch")]
        public string Branch { get; set; } = string.Empty;

        [BsonElement("plot")]
        public string Plot { get; set; } = string.Empty;

        [BsonElement("user_id")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("sanatory_name")]
        public string SanatoryName { get; set; } = string.Empty;

        [BsonElement("is_dreport")]
        public bool IsDReport { get; set; }

        [BsonElement("points")]
        [Range(0,35)]
        public int Points { get; set; }

        [BsonElement("come_in_date")]
        public DateTime ComeInDate { get; set; } = DateTime.Now;

        [BsonElement("trip_status")]
        public TripStatus TripStatus { get; set; }

        [BsonElement("social_help")]
        public SocialTypeHelp SocialHelp { get; set; }

    }
}
