using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;

using Ab.Amazon;
using Ab.Amazon.Data;

namespace NicheLens.Scrapper.Data
{
	public sealed class SqlCategoryRepository : ICategoryRepository
	{
		private readonly IModelContext _db;

		public SqlCategoryRepository(IModelContext db)
		{
			_db = db;
		}

		public Task<CategoryEntity[]> GetCategories(string[] indices)
		{
			var q = from c in _db.Categories
					where indices.Contains(c.SearchIndex)
					select c;
			return q.ToArrayAsync();
		}

		public Task<int> SaveCategory(CategoryEntity category)
		{
			_db.Categories.Add(category);
			return _db.SaveChangesAsync();
		}

		public Task<int> UpdateCategories(CategoryEntity[] categories)
		{
			_db.Categories.AddOrUpdate(c => c.SearchIndex, categories);
			return _db.SaveChangesAsync();
		}
	}
}