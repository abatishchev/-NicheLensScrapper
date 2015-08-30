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
			Map(c => c.SearchIndex).ConvertUsing(r => FilterUnknown(r.GetField<string>("SearchIndex")));
			Map(c => c.Catalog).ConvertUsing(r => FilterUnknown(r.GetField<string>("Catalog")));
			Map(c => c.Path).ConvertUsing(ReadPath);
			Map(c => c.ShowInStore).TypeConverterOption(true, "Y")
								   .TypeConverterOption(false, "N");
		}

		private static string FilterUnknown(string value)
		{
			return !String.Equals(value, "-unknown-", StringComparison.OrdinalIgnoreCase) ? value : String.Empty;
		}

		private static string ReadPath(ICsvReaderRow row)
		{
			return System.Web.HttpUtility.HtmlDecode(row.GetField<string>("Path"));
		}
	}
}