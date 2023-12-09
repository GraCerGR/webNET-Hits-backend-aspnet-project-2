using System.ComponentModel.DataAnnotations;

namespace Test.Models.DTO
{
    public class CommentDto
    {

        [Required]
        public Guid id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public string createTime { get; set; }

        [Required]
        [MinLength(5)]

        public string content { get; set; }

        [DataType(DataType.DateTime)]
        public string modifiedDate { get; set; }

        [DataType(DataType.DateTime)]
        public string deleteDate { get; set; }

        [Required]

        public Guid authorId { get; set; }

        [Required]
        [MinLength(1)]

        public string author { get; set; }

        [Required]

        public int subComments { get; set; }
    }
}
