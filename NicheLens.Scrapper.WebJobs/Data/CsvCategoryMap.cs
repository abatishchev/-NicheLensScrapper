using System;

using CsvHelper;
using CsvHelper.Configuration;

namespace NicheLens.Scrapper.WebJobs.Data
{
	public sealed class CsvCategoryMap : CsvClassMap<CsvCategory>
	{
		public CsvCategoryMap()
		{
			AutoMap();
			Map(c => c.ParentNodeId).Name("ParentNodeID");
			Map(c => c.NodeId).Name("NodeID");
			Map(c => c.Catalog).ConvertUsing(ReadCatalog);
			Map(c => c.Path).ConvertUsing(ReadPath);
			Map(c => c.ShowInStore).TypeConverterOption(true, "Y")
								   .TypeConverterOption(false, "N");
		}

		private static string ReadCatalog(ICsvReaderRow row)
		{
			var catalog = row.GetField<string>("Catalog");
			return !String.Equals(catalog, "-unknown-", StringComparison.OrdinalIgnoreCase) ? catalog : String.Empty;
		}

		private static string ReadPath(ICsvReaderRow row)
		{
			return System.Web.HttpUtility.HtmlDecode(row.GetField<string>("Path"));
		}
	}
}