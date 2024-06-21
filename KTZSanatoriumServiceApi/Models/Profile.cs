using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KTZSanatoriumServiceApi.Models
{
    public class Profile : Base
    {
        [BsonElement("iin")]
        public string IIN { get; set; } = string.Empty;

        [BsonElement("personal_number")]
        public string PersonalNumber { get; set; } = string.Empty;

        [BsonElement("surname")]
        public string Surname { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("middle_name")]
        public string MiddleName { get; set; } = string.Empty;

        [BsonElement("birthday_date")]
        public DateTime BirthDayDate { get; set; } = DateTime.Now;

        [BsonElement("number_of_card_id")]
        public string NumberOfCardId { get; set; } = string.Empty;

        [BsonElement("come_in_date")]
        public DateTime ComeInDate { get; set; } = DateTime.Now;



        [BsonElement("major")]
        public string Major { get; set; } = string.Empty;

        [BsonElement("branch")]
        public string Branch { get; set; } = string.Empty;

        [BsonElement("category")]
        public string Category { get; set; } = string.Empty;

        [BsonElement("dreport")]
        public string DReport { get; set; } = string.Empty;

        [BsonElement("medical_report")]
        public string MedicalReport { get; set; } = string.Empty;

        [BsonElement("honarary_railway")]
        public string HonararyRailway { get; set; } = string.Empty;

        [BsonElement("application_status")]
        public ApplicationStatus ApplicationStatus { get; set; }


        [BsonElement("date_of_isue_id_card")]
        public DateTime DateOfIsueIdCard { get; set; } = DateTime.Now;



    }
}
