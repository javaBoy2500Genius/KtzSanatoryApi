using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KTZSanatoriumServiceApi.Models
{
    public class User : Base
    {

        [BsonElement("login")]
        public string Login { get; set; } = string.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;


        [JsonIgnore]
        [BsonElement("profile_id")]
        public required string ProfileId { get; set; }= string.Empty;
        
        [BsonElement("role")]
        public UserRole Role { get; set; }   
    }
}
