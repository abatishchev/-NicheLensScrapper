using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace NicheLens.Scrapper.Data
{
	public interface IModelContext : IDisposable
	{
		DbSet<Models.Product> Products { get; }

		Task<int> SaveChangesAsync();
	}
}