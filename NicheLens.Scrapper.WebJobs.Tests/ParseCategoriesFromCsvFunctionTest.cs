using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ab;
using Ab.Amazon;
using Ab.Amazon.Data;
using Ab.Factory;
using Ab.Filtering;

using CsvHelper;
using FluentAssertions;

using Microsoft.WindowsAzure.Storage.Blob;

using Moq;

using NicheLens.Scrapper.WebJobs.Data;

using Ploeh.AutoFixture;
using Xunit;

namespace NicheLens.Scrapper.WebJobs.Tests
{
	public class ParseCategoriesFromCsvFunctionTest
	{
		[Fact]
		public async Task ParseCategoriesFromCsv_Should_Parse_Csv_And_Save_Categories_And_Enqueue_Categories_And_Delete_Blob()
		{
			// Arrange
			var fixture = new Fixture();
			var csvCategories = fixture.Build<CsvCategory>()
									   .CreateMany()
									   .ToArray();
			var categories = fixture.Build<Category>()
									.CreateMany()
									.ToArray();
			var e = categories.AsEnumerable().GetEnumerator();

			var provider = new Mock<IAzureCategoryProvider>();
			provider.Setup(p => p.SaveCategories(categories)).Returns(Task.CompletedTask);
			provider.Setup(p => p.EnqueueCategory(It.IsIn(categories))).Returns(Task.CompletedTask);

			var csvReader = new Mock<ICsvReader>();
			csvReader.Setup(r => r.GetRecords<CsvCategory>()).Returns(csvCategories);

			var factory = new Mock<IFactory<ICsvReader, TextReader>>();
			factory.Setup(f => f.Create(It.IsAny<TextReader>())).Returns(csvReader.Object);

			var converter = new Mock<IConverter<CsvCategory, Category>>();
			converter.Setup(c => c.Convert(It.IsIn(csvCategories))).Returns(() => e.MoveNext() ? e.Current : null);

			var blob = new Mock<ICloudBlob>();
			blob.Setup(b => b.OpenReadAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new MemoryStream());
			blob.Setup(b => b.DeleteAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

			var functions = CreateFunctions(provider.Object, factory.Object, converter.Object);

			// Act
			await functions.ParseCategoriesFromCsv(blob.Object, TextWriter.Null, CancellationToken.None);

			// Assert
			provider.VerifyAll();
			factory.VerifyAll();
			converter.VerifyAll();
			blob.VerifyAll();
		}

		[Fact]
		public void ParseCategoriesFromCsv_Should_Log_Exception()
		{
			// Arrange
			var exception = new Exception();

			var logger = new Mock<ILogger>();
			logger.Setup(l => l.LogException(exception));

			var blob = new Mock<ICloudBlob>();
			blob.Setup(b => b.OpenReadAsync(It.IsAny<CancellationToken>())).Throws(exception);

			var functions = CreateFunctions(logger: logger.Object);

			// Act
			Func<Task> action = () => functions.ParseCategoriesFromCsv(blob.Object, TextWriter.Null, CancellationToken.None);

			// Assert
			action.ShouldThrow<Exception>();
			logger.VerifyAll();
		}

		private static ParseCategoriesFromCsvFunction CreateFunctions(IAzureCategoryProvider azureCategoryProvider = null,
																	  IFactory<ICsvReader, TextReader> factory = null,
																	  IConverter<CsvCategory, Category> converter = null,
																	  ILogger logger = null)
		{
			return new ParseCategoriesFromCsvFunction(logger ?? Mock.Of<ILogger>(),
													  azureCategoryProvider ?? Mock.Of<IAzureCategoryProvider>(),
													  new CsvCategoryParser(factory),
													  converter ?? Mock.Of<IConverter<CsvCategory, Category>>(),
													  new FilterAdapter<Category>(true));
		}
	}
}