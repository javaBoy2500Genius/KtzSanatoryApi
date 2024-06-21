using KTZSanatoriumServiceApi.Models;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KTZSanatoriumServiceApi.ModelsDto
{
    public class ApplicationDto :Base
    {
        [BsonIgnore]
        public required UsersDto User { get; set; }

        [BsonIgnore]
        public required Sanatory Sanatory { get; set; }

        [BsonIgnore]
        public SocialTypeHelp SocialHelp { get; set; }

        [BsonIgnore]
        public DateTime ComeInDate { get; set; } = DateTime.Now;

        [BsonIgnore]
        public ApplicationStatus ApplicationStatus { get; set; }

  
    }
}
