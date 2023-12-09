using System.ComponentModel.DataAnnotations;

namespace Test.Models.DTO
{
    public class CreatePostDto
    {

        [Required]
        [MinLength(5)]
        [MaxLength(1000)]

        public string title { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(5000)]
        public string description { get; set; }

        [Required]
        public int readingTime { get; set; }

        [Url]
        [MaxLength(1000)]
        public string? image { get; set; }

        public Guid? addressId { get; set; }

        [Required]
        [MinLength(1)]
        public string[] tags { get; set; }
    }
}
