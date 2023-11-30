using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class UserDto
    {
        public string id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public string createTime { get; set; }

        [Required]
        [MinLength(1)]
        public string fullname { get; set; }

        [DataType(DataType.DateTime)]
        public string birthDate { get; set; }

        [Required]
        [EnumDataType(typeof(Gender))]
        public string gender { get; set; }

        [Required]
        [EmailAddress]
        [MinLength(1)]
        public string email { get; set; }

        [Phone]
        public string phoneNumber { get; set; }

    }
}
