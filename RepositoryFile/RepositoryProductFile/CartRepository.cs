using MegaStor.DATA;
using MegaStor.Models;
using Microsoft.EntityFrameworkCore;
namespace MegaStor.RepositoryFile.RepositoryProductFile
{
	public class CartRepository
	{
		private readonly AppDbContext _dbcontext;

		public CartRepository(AppDbContext Dbcontext)
		{
			_dbcontext = Dbcontext;
		}

		public async Task<Cart> GetCartByUserIdAsync(string userId)
		{
			return await _dbcontext.Carts
				.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
				.FirstOrDefaultAsync(c => c.UserId == userId);
		}

    	public async Task<Cart> GetCartByIdAsync(string cartId)
		{
			return await _dbcontext.Carts
				.Include(c => c.CartItems)
				.FirstOrDefaultAsync(c => c.CartId == cartId);
		}

		public async Task AddToCartAsync(Cart cart)
		{
			await _dbcontext.Carts.AddAsync(cart);
 		}


		public void UpdateCart(Cart cart)
		{
			_dbcontext.Carts.Update(cart);
 		}
		public void DeleteCart(Cart cart)
		{
			 _dbcontext.Carts.Remove(cart);
 		}


		public IQueryable<Cart> GetAllCarts(String id)
		{
			return _dbcontext.Carts.Include(c => c.CartItems).Where(i => i.UserId == id);
		}


		public async Task SaveChangesAsync()
		{
			await _dbcontext.SaveChangesAsync();
		}


		///////////////////////////////
		///some methods for CartItem///
		///////////////////////////////
		public  CartItem FindCartItem(int cartitemId)
		{
			return _dbcontext.CartItems.Include(C=>C.Cart).Include(p=>p.Product).FirstOrDefault(c => c.Id == cartitemId);

		}
		 
		public void DeleteCartItem(int cartitemId)
		{
			var item =  FindCartItem(cartitemId);
		    _dbcontext.CartItems.Remove(item);
 
		}
	 
		public void UpdateCartItem(CartItem cart)
		{
			_dbcontext.CartItems.Update(cart);
		}

	}
}
