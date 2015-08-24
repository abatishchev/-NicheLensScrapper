namespace NicheLens.Scrapper.WebJobs.Data
{
	public class CsvCategory
	{
		public string Name { get; set; }

		public long ParentNodeId { get; set; }

		public long NodeId { get; set; }

		public string SearchIndex { get; set; }

		public string Catalog { get; set; }

		public string Path { get; set; }

		public string Description { get; set; }

		public bool ShowInStore { get; set; }
	}
}