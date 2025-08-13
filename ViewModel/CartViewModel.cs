namespace MegaStor.ViewModel
{
	public class CartViewModel
    {
		public string CartId { get; set; }
 		public ICollection<CartItemViewModel>? CartItems { get; set; } 
		public int TotalQuantity { get; set; }
		public decimal TotalAmount { get; set; }
 	}

}
