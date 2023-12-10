using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Test.Models.DTO
{
    public class CommunityFullDto
    {
        [Required]
        public Guid id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public string createTime { get; set; }

        [Required]
        [MinLength(1)]
        public string name { get; set; }

        public string description { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool isClosed { get; set; }

        [Required]
        [DefaultValue(0)]
        public int subscribersCount { get; set; }

        [Required]
        public List<UserDto> administrators { get; set; }

    }
}
