namespace MegaStor.ViewModel
{
	public class ShowUsersViewModel
	{
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty; 
        public string Password { get; set; } = string.Empty;
        public string? DateCreated  { get; set; } 
		  
        public string urlImg { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

    }
}
