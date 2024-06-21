

using KTZSanatoriumServiceApi.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace KTZSanatoriumServiceApi.ModelsDto
{

    public class ProfileDto : Base
    {

        public string IIN { get; set; } = string.Empty;

        public string PersonalNumber { get; set; } = string.Empty;

        public string Surname { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string MiddleName { get; set; } = string.Empty;

        public DateTime BirthDayDate { get; set; } = DateTime.Now;

        public string NumberOfCardId { get; set; } = string.Empty;

        public DateTime ComeInDate { get; set; } = DateTime.Now;

        public string Major { get; set; } = string.Empty;

        public string Branch { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string DReport { get; set; } = string.Empty;

        public string MedicalReport { get; set; } = string.Empty;

        public string HonararyRailway { get; set; } = string.Empty;

        public ApplicationStatus ApplicationStatus { get; set; }

        public required string SanatoryId { get; set; }

        public DateTime DateOfIsueIdCard { get; set; } = DateTime.Now;




    }
}
