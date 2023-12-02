using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class User
    {
        public string id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public string createTime { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string fullName { get; set; }

        [Required]
        [MinLength(6)]
        public string password { get; set; }

        [Required]
        [EmailAddress]
        [MinLength(1)]
        public string email { get; set; }

        [DataType(DataType.DateTime)]
        public string? birthDate { get; set; }

        [Required]
        [EnumDataType(typeof(Gender))]
        public string gender { get; set; }

        [Phone]
        public string? phoneNumber { get; set; }
    }
}
