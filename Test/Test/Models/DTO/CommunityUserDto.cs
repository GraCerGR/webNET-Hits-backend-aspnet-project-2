using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models
{
    public class CommunityUserDto
    {
        [Required]
        public Guid communityId { get; set; }

        [Required]
        public Guid userId { get; set; }
        
        [Required]
        [Column(TypeName = "text")]
        public CommunityRole role { get; set; }
    }
}
