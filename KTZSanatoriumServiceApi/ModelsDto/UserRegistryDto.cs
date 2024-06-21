using KTZSanatoriumServiceApi.Models;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace KTZSanatoriumServiceApi.ModelsDto
{
    public class UserRegistryDto : Base
    {

        public string IIN { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string MiddleName { get; set; } = string.Empty;

        public string SurName { get; set; } = string.Empty;

        public string PersonalNumber { get; set; } = string.Empty;

        public string Branch { get; set; } = string.Empty;

        public string Plot { get; set; } = string.Empty;

        public  UsersDto User { get; set; }

        public string SanatoryName { get; set; } = string.Empty;

        public bool IsDReport { get; set; }

        public int Points { get; set; }

        public DateTime ComeInDate { get; set; } = DateTime.Now;

        public TripStatus TripStatus { get; set; }

        public SocialTypeHelp SocialHelp { get; set; }

    }
}

