namespace MegaStor.ViewModel
{
	public class WishlistViewModel
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public string? ImageUrl { get; set; }
		public decimal? pricediscount { get; set; }
 		public string? SelerName { get; set; }
		public DateTime DateAdded { get; set; }

	}
}
