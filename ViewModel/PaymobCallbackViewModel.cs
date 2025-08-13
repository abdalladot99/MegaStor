namespace MegaStor.ViewModel
{
	public class PaymobCallbackViewModel
	{
		public CallbackObj obj { get; set; }

	}
	public class CallbackObj
	{
		public bool success { get; set; }
		public CallbackOrder order { get; set; }
	}

	public class CallbackOrder
	{
		public int id { get; set; }
	}


	

}

