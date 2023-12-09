using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Test.Models.DTO
{
    public class PostFullDto
    {

        public Guid id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public string createTime { get; set; }

        [Required]
        [MinLength(1)]

        public string title { get; set; }

        [Required]
        [MinLength(1)]
        public string description { get; set; }

        [Required]

        public int readingTime { get; set; }

        public string? image { get; set; }

        [Required]

        public Guid authorId { get; set; }

        [Required]
        [MinLength(1)]

        public string? author { get; set; }

        public Guid? communityId { get; set; }

        public string? communityName { get; set; }

        public Guid? addressId { get; set; }

        [Required]
        [DefaultValue(0)]

        public int likes { get; set; }

        [Required]
        [DefaultValue(false)]

        public bool hasLike { get; set; }

        [Required]
        [DefaultValue(0)]

        public int commentsCount { get; set; }

        [Required]
        public List<TagDto> tags { get; set; }

        [Required]

        public CommentDto comments { get; set; }
    }
}
