using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using Ab.Amazon;
using Ab.Amazon.Data;

namespace NicheLens.Scrapper.Data
{
	public sealed class EntityCategoryRepository : ICategoryRepository
	{
		private readonly IModelContext _db;

		public EntityCategoryRepository(IModelContext db)
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

		public Task SaveCategory(CategoryEntity category)
		{
			_db.Categories.Add(category);
			return _db.SaveChangesAsync();
		}

		public async Task UpdateCategories(CategoryEntity[] categories)
		{
			foreach (var category in categories)
			{
				var existingProduct = await GetCategory(category.NodeId);
				if (existingProduct != null)
					await DeleteCategory(existingProduct);

				_db.Categories.Add(category);
			}

			await _db.SaveChangesAsync();
		}

		private Task<CategoryEntity> GetCategory(long nodeId)
		{
			return _db.Categories.FirstOrDefaultAsync(c => c.NodeId == nodeId);
		}

		private Task DeleteCategory(CategoryEntity category)
		{
			_db.Categories.Remove(category);
			return _db.SaveChangesAsync();
		}
	}
}