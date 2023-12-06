using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Runtime.InteropServices;
using Npgsql.Internal.TypeHandlers;

namespace Test.Models.DTO
{
    public class PostDto
    {
        public string id { get; set; }

        [Required]
        [DataType(DataType.Date)]
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

        public string authorId { get; set; }

        [Required]
        [MinLength(1)]

        public string? author { get; set;}

        public string? communityId { get; set; }

        public string? communityName { get; set; }

        public string? addressId { get; set; }

        [Required]
        [DefaultValue(0)]

        public int likes { get; set; }

        [Required]
        [DefaultValue(false)]

        public bool hasLike { get; set;}

        [Required]
        [DefaultValue(0)]

        public int commentsCount { get; set; }

        [Required]
        public List<TagDto> tags { get; set; }


    }
}
