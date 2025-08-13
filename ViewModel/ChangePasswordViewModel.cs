using System.ComponentModel.DataAnnotations;

namespace MegaStor.ViewModel
{
	public class ChangePasswordViewModel
	{
		public string UserId { get; set; }

		[Required(ErrorMessage = "Current password is required")]
		[DataType(DataType.Password)]
		public string CurrentPassword { get; set; }

		[Required(ErrorMessage = "New password is required")]
		[DataType(DataType.Password)]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Confirm password is required")]
		[DataType(DataType.Password)]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}

}