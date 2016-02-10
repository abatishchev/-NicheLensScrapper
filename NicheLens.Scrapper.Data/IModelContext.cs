using System;
using System.Data.Entity;
using System.Threading.Tasks;

using Ab.Amazon.Data;

namespace NicheLens.Scrapper.Data
{
	public interface IModelContext : IDisposable
	{
		DbSet<CategoryEntity> Categories { get; }

		DbSet<ProductEntity> Products { get; }

		Task<int> SaveChangesAsync();
	}
}