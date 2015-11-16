using System;
using System.Threading.Tasks;

using NicheLens.Scrapper.Data.Models;

namespace NicheLens.Scrapper.Data
{
	public interface IProductRepository
	{
		Task<Product> GetProduct(Guid productId);

		Task MergeProducts(Product[] products);
	}
}