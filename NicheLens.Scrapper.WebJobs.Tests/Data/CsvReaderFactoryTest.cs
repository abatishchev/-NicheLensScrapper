using System.IO;

using CsvHelper;
using FluentAssertions;

using NicheLens.Scrapper.WebJobs.Data;

namespace NicheLens.Scrapper.WebJobs.Tests.Data
{
	public class CsvReaderFactoryTest
	{
		public void Create_Should_Return_CsvReader_With_Configuration()
		{
			// Arrange
			var factory = new CsvReaderFactory(new CsvFactory());

			// Act
			var configuration = factory.Create(TextReader.Null).Configuration;

			// Assert
			configuration.Delimiter.Should().Be("|");
			configuration.Maps.Find<CsvCategoryMap>().Should().NotBeNull();
		}
	}
}