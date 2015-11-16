using System;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;

using NicheLens.Scrapper.Data.Models;

namespace NicheLens.Scrapper.Data
{
	public sealed class SqlProductRepository : IProductRepository
	{
		private readonly IModelContext _db;

		public SqlProductRepository(IModelContext db)
		{
			_db = db;
		}

		public Task<Product> GetProduct(Guid productId)
		{
			return _db.Products.FindAsync(productId);
		}

		public Task MergeProducts(Product[] products)
		{
			_db.Products.AddOrUpdate(p => p.Asin, products);

			return _db.SaveChangesAsync();
		}
	}
}