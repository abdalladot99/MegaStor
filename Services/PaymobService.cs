using System.Text.Json;
using Microsoft.Extensions.Options;

namespace MegaStor.Services
{
	public class PaymobSettings
	{
		public string ApiKey { get; set; }
		public string IframeId { get; set; }
		public string IntegrationId { get; set; }
		public string BaseUrl { get; set; }
	}

	public class PaymobService
	{
		private readonly HttpClient _httpClient;
		private readonly PaymobSettings _settings;

		public PaymobService(HttpClient httpClient, IOptions<PaymobSettings> options)
		{
			_httpClient = httpClient;
			_settings = options.Value;
		}

		public async Task<string> StartPaymentAsync(decimal totalAmount, string email, string phone, string name)
		{
			// 1. Get Token
			var tokenResponse = await _httpClient.PostAsJsonAsync($"{_settings.BaseUrl}/auth/tokens", new
			{
				api_key = _settings.ApiKey
			});
			var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
			using var tokenDoc = JsonDocument.Parse(tokenJson);
			var token = tokenDoc.RootElement.GetProperty("token").GetString();

			// 2. Create Order
			var orderResponse = await _httpClient.PostAsJsonAsync($"{_settings.BaseUrl}/ecommerce/orders", new
			{
				auth_token = token,
				delivery_needed = false,
				amount_cents = (int)(totalAmount * 100),
				currency = "EGP",
				items = new object[] { },
				redirect_url = "https://57cf571c91f1.ngrok-free.app/Order/PaymentResult"

			});

			var orderJson = await orderResponse.Content.ReadAsStringAsync();
			using var orderDoc = JsonDocument.Parse(orderJson);
			int orderId = orderDoc.RootElement.GetProperty("id").GetInt32();

			// 3. Create Payment Key
			var paymentKeyResponse = await _httpClient.PostAsJsonAsync($"{_settings.BaseUrl}/acceptance/payment_keys", new
			{
				auth_token = token,
				amount_cents = (int)(totalAmount * 100),
				expiration = 3600,
				order_id = orderId,
				currency = "EGP",
				integration_id = int.Parse(_settings.IntegrationId),
				billing_data = new
				{
					first_name = name,
					last_name = "NA",
					email = email,
					phone_number = phone,
					apartment = "NA",
					floor = "NA",
					street = "NA",
					building = "NA",
					city = "Cairo",
					country = "EG",
					state = "Cairo"
				}
			});

			var paymentJson = await paymentKeyResponse.Content.ReadAsStringAsync();
			using var paymentDoc = JsonDocument.Parse(paymentJson);
			var paymentToken = paymentDoc.RootElement.GetProperty("token").GetString();

			// 4. Redirect to Paymob Iframe
			return $"https://accept.paymob.com/api/acceptance/iframes/{_settings.IframeId}?payment_token={paymentToken}";
		}
	}
}
