using System.ComponentModel.DataAnnotations;

namespace Test.Models.DTO
{
    public class CreateCommentDto
    {
        [Required]
        [MaxLength(1000)]
        [MinLength(1)]
        public string content { get; set; }

        public Guid? parentId { get; set; }
    }
}
