using System;

using CsvHelper;
using CsvHelper.Configuration;

namespace NicheLens.Scrapper.WebJobs.Data
{
	public sealed class CsvCategoryMap : CsvClassMap<CsvCategory>
	{
		public CsvCategoryMap()
		{
			Map(c => c.Title);
			Map(c => c.ParentNodeId).Name("ParentNodeID");
			Map(c => c.NodeId).Name("NodeID");
			Map(c => c.SearchIndex);
			Map(c => c.Catalog).ConvertUsing(ReadCatalog);
			Map(c => c.Path).TypeConverter<CsvCategoryPathTypeConverter>();
			Map(c => c.Description);
			Map(c => c.ShowInStore).TypeConverterOption(true, "Y")
								   .TypeConverterOption(false, "N");
		}

		private static string ReadCatalog(ICsvReaderRow row)
		{
			var catalog = row.GetField<string>("Catalog");
			return !String.Equals(catalog, "-unknown-", StringComparison.OrdinalIgnoreCase) ? catalog : String.Empty;
		}
	}
}