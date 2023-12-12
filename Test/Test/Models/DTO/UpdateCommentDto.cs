using System.ComponentModel.DataAnnotations;

namespace Test.Models.DTO
{
    public class UpdateCommentDto
    {
        [Required]
        [MaxLength(1000)]
        [MinLength(1)]
        public string content { get; set; }
    }
}
