using System.ComponentModel.DataAnnotations;

namespace MegaStor.ViewModel
{
    public class AddRoleViewModel
    {

        [Required]
        public string RoleName { get; set; }

    }
}
