using System.ComponentModel.DataAnnotations;

namespace Test.Models.DTO
{
    public class CommentDto
    {

        [Required]
        public string id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public string createTime { get; set; }

        [Required]
        [MinLength(5)]

        public string content { get; set; }

        [DataType(DataType.Date)]
        public string modifiedDate { get; set; }

        [DataType(DataType.Date)]
        public string deleteDate { get; set; }

        [Required]

        public string authorId { get; set; }

        [Required]
        [MinLength(1)]

        public string author { get; set; }

        [Required]

        public int subComments { get; set; }
    }
}
