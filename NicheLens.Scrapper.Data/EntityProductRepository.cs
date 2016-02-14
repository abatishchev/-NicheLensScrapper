using System;
using System.Data.Entity;
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
			foreach (var product in products)
			{
				var existingProduct = await GetProduct(product.Asin);
				if (existingProduct != null)
					await DeleteProduct(existingProduct);

				_db.Products.Add(product);
			}

			await _db.SaveChangesAsync();
		}

		private Task<ProductEntity> GetProduct(string asin)
		{
			return _db.Products.FirstOrDefaultAsync(p => p.Asin == asin);
		}

		private Task DeleteProduct(ProductEntity product)
		{
			_db.Products.Remove(product);
			return _db.SaveChangesAsync();
		}
	}
}