using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using Ab.Amazon;
using Ab.Amazon.Data;

namespace NicheLens.Scrapper.Data
{
	public sealed class EntityProductRepository : IProductRepository
	{
		private readonly IModelContext _db;

		public EntityProductRepository(IModelContext db)
		{
			_db = db;
		}

		public Task<ProductEntity> GetProduct(Guid productId)
		{
			return _db.Products.FindAsync(productId);
		}

		public async Task UpdateProducts(ProductEntity[] products)
		{
			var existing = await GetProducts(products).ToArrayAsync();
			if (existing.Any())
				await DeleteProducts(existing);

			_db.Products.AddRange(products);

			await _db.SaveChangesAsync();
		}

		private IQueryable<ProductEntity> GetProducts(ICollection<ProductEntity> products)
		{
			var ids = products.Select(p => p.ProductId).ToArray();
			var asins = products.Select(p => p.Asin).ToArray();
			return from p in _db.Products
				   where ids.Contains(p.ProductId) || asins.Contains(p.Asin)
				   select p;
		}

		private Task DeleteProducts(ICollection<ProductEntity> products)
		{
			_db.Products.RemoveRange(products);

			return _db.SaveChangesAsync();
		}
	}
}