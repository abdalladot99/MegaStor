using System.ComponentModel.DataAnnotations;

namespace MegaStor.ViewModel
{
    public class LoginAccountViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
