using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class CommentComment
    {
        [Required]
        public Guid commentId1 { get; set; }

        [Required]
        public Guid commentId2 { get; set; }
    }
}
