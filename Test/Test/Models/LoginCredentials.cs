using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class LoginCredentials
    {
        [Required]
        [EmailAddress]
        [MinLength(1)]
        public string email { get; set; }

        [Required]
        [MinLength(1)]
        public string password { get; set; }
    }
}
