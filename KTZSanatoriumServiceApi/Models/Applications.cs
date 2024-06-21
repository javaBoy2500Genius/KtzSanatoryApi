using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KTZSanatoriumServiceApi.Models
{
    public class Applications : Base
    {
       
        [BsonElement("user_id")]
        public required string UserId { get; set; } =string.Empty;


     
        [BsonElement("sanatory_id")]
        public required string SanatoryId { get; set; } = string.Empty;
        
       

        [BsonElement("social_help")]
        public SocialTypeHelp SocialHelp { get; set; }

        [BsonElement("come_in_date")]
        public DateTime ComeInDate { get; set; } = DateTime.Now;

        [BsonElement("application_status")]
        public ApplicationStatus ApplicationStatus { get; set; }
    }
}
