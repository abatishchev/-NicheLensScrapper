using System;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;

using Ab.Amazon;
using Ab.Amazon.Data;

namespace NicheLens.Scrapper.Data
{
	public sealed class SqlProductRepository : IProductRepository
	{
		private readonly IModelContext _db;

		public SqlProductRepository(IModelContext db)
		{
			_db = db;
		}

		public Task<ProductEntity> GetProduct(Guid productId)
		{
			return _db.Products.FindAsync(productId);
		}

		public Task UpdateProducts(ProductEntity[] products)
		{
			_db.Products.AddOrUpdate(p => p.Asin, products);
			return _db.SaveChangesAsync();
		}
	}
}