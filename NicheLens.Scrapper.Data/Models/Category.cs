using System;

namespace NicheLens.Scrapper.Data.Models
{
	public class Category
	{
		public Guid CategoryId { get; set; }

		public string Name { get; set; }

		public long NodeId { get; set; }

		public long ParentNodeId { get; set; }

		public string SearchIndex { get; set; }
	}
}