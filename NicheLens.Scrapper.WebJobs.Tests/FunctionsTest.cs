using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ab;
using Ab.Amazon;
using Ab.Amazon.Data;
using Ab.Factory;

using CsvHelper;
using Elmah;
using FluentAssertions;

using Microsoft.Azure.Documents;
using Microsoft.WindowsAzure.Storage.Blob;

using Moq;

using NicheLens.Scrapper.WebJobs.Data;

using Ploeh.AutoFixture;
using Xunit;

namespace NicheLens.Scrapper.WebJobs.Tests
{
	public class FunctionsTest
	{
		[Fact]
		public async Task ParseCategoriesFromCsv_Should_ParseCsv_And_Save_Categories_And_Delete_Blob()
		{
			// Assert
			var fixture = new Fixture();
			var csvCategories = fixture.Build<CsvCategory>()
									   .CreateMany()
									   .ToArray();
			var categories = fixture.Build<Category>()
									.CreateMany()
									.ToArray();
			var e = categories.AsEnumerable().GetEnumerator();

			var provider = new Mock<IAzureCategoryProvider>();
			provider.Setup(p => p.SaveCategories(categories)).Returns(Task.FromResult(new Document[0]));

			var csvReader = new Mock<ICsvReader>();
			csvReader.Setup(r => r.GetRecords<CsvCategory>()).Returns(csvCategories);

			var factory = new Mock<IFactory<ICsvReader, TextReader>>();
			factory.Setup(f => f.Create(It.IsAny<TextReader>())).Returns(csvReader.Object);

			var converter = new Mock<IConverter<CsvCategory, Category>>();
			converter.Setup(c => c.Convert(It.IsIn(csvCategories))).Returns(() =>
				{
					e.MoveNext();
					return e.Current;
				});

			var blob = new Mock<ICloudBlob>();
			blob.Setup(b => b.OpenReadAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new MemoryStream());
			blob.Setup(b => b.DeleteAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

			var functions = CreateFunctions(provider.Object, factory.Object, converter.Object);

			// Act
			await functions.ParseCategoriesFromCsv(blob.Object, CancellationToken.None);

			// Assert
			provider.VerifyAll();
			factory.VerifyAll();
			converter.VerifyAll();
			blob.VerifyAll();
		}

		public async Task ParseCategoriesFromCsv_Should_Log_Exception()
		{
			// Assert
			var csvReader = new Mock<ICsvReader>();
			csvReader.Setup(r => r.GetRecords<CsvCategory>()).Throws<CsvHelperException>();

			var factory = new Mock<IFactory<ICsvReader, TextReader>>();
			factory.Setup(f => f.Create(It.IsAny<TextReader>())).Returns(csvReader.Object);

			var functions = CreateFunctions(factory: factory.Object);

			// Act
			await functions.ParseCategoriesFromCsv(Mock.Of<ICloudBlob>(), CancellationToken.None);

			// Assert
			ErrorLog.GetDefault(null).GetErrors(0, 1, new List<object>()).Should().BeGreaterThan(0);
		}

		private static Functions CreateFunctions(IAzureCategoryProvider provider = null,
												 IFactory<ICsvReader, TextReader> factory = null,
												 IConverter<CsvCategory, Category> converter = null)
		{
			return new Functions(
				provider ?? Mock.Of<IAzureCategoryProvider>(),
				new CsvCategoryParser(factory),
				converter ?? CreateConverter());
		}

		private static IConverter<CsvCategory, Category> CreateConverter()
		{
			var container = ContainerConfig.CreateContainer();
			return container.GetInstance<MappingCsvCategoryConverter>();
		}
	}
}