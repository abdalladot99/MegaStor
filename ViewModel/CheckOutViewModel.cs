using System.ComponentModel.DataAnnotations;
using MegaStor.Constants.Enum;
using MegaStor.Models;

namespace MegaStor.ViewModel
{
	public class CheckOutViewModel
	{
		public List<CartItemViewModel> CartItems { get; set; }
		public ShippingAddressViewModel ShippingAddress { get; set; }
		public BillingAddressViewModel BillingAddress { get; set; }

		[Required(ErrorMessage = "Please select a payment method")]
		public PaymentMethodsEnum SelectedPaymentMethod { get; set; }
		public decimal TotalAmount { get; set; }
	}
}
