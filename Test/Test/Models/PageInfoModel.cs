using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class PageInfoModel
    {
        [Required]

        public int size { get; set; }

        [Required]

        public int count { get; set; }

        [Required]
        public int current { get; set; }
    }
}
