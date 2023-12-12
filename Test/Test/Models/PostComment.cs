using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class PostComment
    {
        [Required]
        public Guid postId { get; set; }

        [Required]
        public Guid commentId { get; set; }
    }
}
