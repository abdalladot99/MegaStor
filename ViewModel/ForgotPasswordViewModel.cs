using System.ComponentModel.DataAnnotations;

namespace MegaStor.ViewModel
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
