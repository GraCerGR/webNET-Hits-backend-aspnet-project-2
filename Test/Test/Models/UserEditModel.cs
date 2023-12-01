using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class UserEditModel
    {

        [Required]
        [EmailAddress]
        [MinLength(1)]
        public string email { get; set; }

        [Required]
        [MinLength(1)]
        public string fullName { get; set; }

        [DataType(DataType.DateTime)]
        public string birthDate { get; set; }

        [Required]
        [EnumDataType(typeof(Gender))]
        public string gender { get; set; }

        [Phone]
        public string phoneNumber { get; set; }
    }
}
