using System.ComponentModel.DataAnnotations;

namespace Test.Models.DTO
{
    public class PostTag
    {
        [Required]
        public string postId { get; set; }

        [Required]
        public string tagId { get; set; }

    }
}
