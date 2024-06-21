using KTZSanatoriumServiceApi.Models;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KTZSanatoriumServiceApi.ModelsDto
{
    public class UsersDto:Base
    {
        [BsonIgnore]

        public string Login { get; set; } = string.Empty;

        [BsonIgnore]

        public string Password { get; set; } = string.Empty;

        [BsonIgnore]

        public required Profile Profile { get; set; }

  



        [BsonIgnore]
        public UserRole Role { get; set; }
    }
}
