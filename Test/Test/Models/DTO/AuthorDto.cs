using System.ComponentModel.DataAnnotations;

namespace Test.Models.DTO
{
    public class AuthorDto
    {
        [Required]
        [MinLength(1)]
        public string fullName { get; set; }

        [DataType(DataType.DateTime)]
        public string? birthDate { get; set; }

        [Required]
        [EnumDataType(typeof(Gender))]
        public string gender { get; set; }

        public int posts { get; set; }

        public int likes { get; set; }

        [DataType(DataType.Date)]
        public string created { get; set; }
    }
}
