using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models
{
    public class CommunityUser
    {
        public Guid communityId { get; set; }

        public Guid userId { get; set; }

        [Column(TypeName = "text")]
        public CommunityRole role { get; set; }
    }
}
