using System.ComponentModel.DataAnnotations;

namespace Test.Models.DTO
{
    public class PostTag
    {
        [Required]
        public Guid postId { get; set; }

        [Required]
        public Guid tagId { get; set; }

    }
}
