using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class PostLiked
    {
        [Required]
        public Guid userId { get; set; }

        [Required]
        public Guid postId { get; set; }

    }
}
