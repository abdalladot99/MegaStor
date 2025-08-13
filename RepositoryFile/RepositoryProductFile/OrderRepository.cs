using System.Linq.Expressions;
using MegaStor.DATA;
using MegaStor.Models;
using Microsoft.EntityFrameworkCore;

namespace MegaStor.RepositoryFile.RepositoryProductFile
{
	public class OrderRepository
	{
		private readonly AppDbContext _orderDb;

		public OrderRepository(AppDbContext orderDb)
		{
			_orderDb = orderDb;
		}


		public async Task<Order> GetById(string Id)
		{
			return await _orderDb.Orders.FirstOrDefaultAsync(i => i.Id == Id);
		}
		 

		public async Task<Order> GetOrderByOrderIdAsync(string orderId)
		{
			return await _orderDb.Orders
								.Include(o => o.Customer)
								.Include(o => o.OrderItems)
									.ThenInclude(oi => oi.Product)
										.ThenInclude(p => p.ProductImages)
								.Include(o => o.OrderItems)
									.ThenInclude(oi => oi.Product)
										.ThenInclude(p => p.Category)
								.Include(o => o.OrderItems)
									.ThenInclude(oi => oi.Product)
										.ThenInclude(p => p.SubCategory)
								.Include(o => o.Payment)
								.Include(o => o.ShippingAddress)
								.Include(o => o.BillingAddress)
								.FirstOrDefaultAsync(o => o.Id == orderId);
		}

		public async Task AddAsync(Order order)
		{
			await _orderDb.Orders.AddAsync(order);
		}



		public async Task<IEnumerable<Order>> GetAllAsync()
		{
			return await _orderDb.Orders.ToListAsync();
		}
		 
		public void Update(Order order)
		{
			_orderDb.Orders.Update(order);
		}

		public async Task<bool> DeleteAsync(string id)
		{
			var entity = await GetById(id);
			if (entity != null)
			{
				_orderDb.Orders.Remove(entity);
				_orderDb.SaveChanges();
				return true;
			}
			return false;
		}



		public IQueryable<Order> GetAllOrder()
		{
			return _orderDb.Orders;
		}



		public async Task SaveAsync()
		{
			await _orderDb.SaveChangesAsync();
		}

		 

		public async Task<IEnumerable<Order>> GetAllWithIncludeAsync(params Expression<Func<Order, object>>[] includeExpressions)
		{
			IQueryable<Order> query = _orderDb.Orders;

			foreach (var includeExpression in includeExpressions)
			{
				query = query.Include(includeExpression);
			}

			return await query.ToListAsync();
		}

	}
}
