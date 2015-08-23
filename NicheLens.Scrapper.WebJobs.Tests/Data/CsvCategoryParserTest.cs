using System.IO;
using System.Linq;

using CsvHelper;
using FluentAssertions;

using NicheLens.Scrapper.WebJobs.Data;

using Xunit;

namespace NicheLens.Scrapper.WebJobs.Tests.Data
{
	public class CsvCategoryParserTest
	{
		[Fact]
		public void ParseCategoriesFromCsv_Should_Parse_Csv()
		{
			// Assert
			const string csv = @"
Title|ParentNodeID|NodeID|SearchIndex|Catalog|Path|Description|ShowInStore
Brands|1036592|1294227011|Apparel|-unknown-|Apparel & Accessories &gt; Brands||Y";

			var parser = new CsvCategoryParser(new CsvReaderFactory(new CsvFactory()));

			// Act
			var categories = parser.Parse(new StringReader(csv)).ToArray();

			// Assert
			var category = categories.Should().ContainSingle().Which;
			category.Title.Should().Be("Brands");
			category.ParentNodeId.Should().Be(1036592);
			category.NodeId.Should().Be(1294227011);
			category.SearchIndex.Should().Be("Apparel");
			category.Catalog.Should().BeEmpty();
			category.Path.Should().Be("Apparel & Accessories > Brands");
			category.Description.Should().BeEmpty();
			category.ShowInStore.Should().BeTrue();
		}
	}
}