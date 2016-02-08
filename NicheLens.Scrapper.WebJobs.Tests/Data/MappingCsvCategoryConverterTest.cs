using Ab.Amazon.Data;

using AutoMapper;
using FluentAssertions;

using NicheLens.Scrapper.WebJobs.Data;

using Ploeh.AutoFixture;
using Xunit;

namespace NicheLens.Scrapper.WebJobs.Tests.Data
{
	public class MappingCsvCategoryConverterTest
	{
		[Fact]
		public void Convert_Should_Map_Properties()
		{
			// Arrange
			var container = ContainerConfig.CreateContainer();

			var mapper = container.GetInstance<IMapper>();

			var converter = new MappingCsvCategoryConverter(mapper);
			var csvCategy = new Fixture().Create<CsvCategory>();

			// Act
			Category category = converter.Convert(csvCategy);

			// Assert
			category.Name.Should().Be(csvCategy.Title);
			category.NodeId.Should().Be(csvCategy.NodeId);
			category.ParentNodeId.Should().Be(csvCategy.ParentNodeId);
			category.SearchIndex.Should().Be(csvCategy.SearchIndex);
		}
	}
}