using System;

using Ab.Amazon.Data;
using Ab.Filtering;

namespace NicheLens.Scrapper.WebJobs.Data
{
	public sealed class EmptySearchIndexCategoryFIlter : IFilter<Category>
	{
		public bool Filter(Category category)
		{
			return !String.IsNullOrEmpty(category.SearchIndex);
		}
	}
}
