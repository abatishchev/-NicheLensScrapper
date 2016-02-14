using System;
using System.Collections.Generic;
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
			var existing = await GetCategory(categories.Select(p => p.CategoryId).ToArray()).ToArrayAsync();
			if (existing.Any())
				await DeleteCategories(existing);

			_db.Categories.AddRange(categories);

			await _db.SaveChangesAsync();
		}

		private IQueryable<CategoryEntity> GetCategory(ICollection<Guid> ids)
		{
			return from p in _db.Categories
				   where ids.Contains(p.CategoryId)
				   select p;
		}

		private Task DeleteCategories(ICollection<CategoryEntity> categories)
		{
			_db.Categories.RemoveRange(categories);

			return _db.SaveChangesAsync();
		}
	}
}