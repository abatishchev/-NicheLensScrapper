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
	public class FunctionsTest
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
			provider.Setup(p => p.EnqueueCategories(categories)).Returns(Task.CompletedTask);

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

			var functions = CreateFunctions(logger.Object);

			// Act
			Func<Task> action = () => functions.ParseCategoriesFromCsv(blob.Object, TextWriter.Null, CancellationToken.None);

			// Assert
			action.ShouldThrow<Exception>();
			logger.VerifyAll();
		}

		[Fact]
		public async Task ProcessCategoryQueue_Should_Get_Products_And_Save_Products()
		{
			// Arrange
			var fixture = new Fixture();
			var category = fixture.Create<Category>();

			var categoryProvider = new Mock<IAwsCategoryProvider>();

			var productProvider = new Mock<IAzureProductProvider>();

			var functions = CreateFunctions(awsCategoryProvider: categoryProvider.Object, azureProductProvider: productProvider.Object);

			// Act
			await functions.ProcessCategoryQueue(category, TextWriter.Null, CancellationToken.None);

			// Assert
			categoryProvider.VerifyAll();
			productProvider.VerifyAll();
		}

		[Fact]
		public void ProcessCategoryQueue_Should_Log_Exception()
		{
			// Arrange
			var exception = new Exception();

			var logger = new Mock<ILogger>();
			logger.Setup(l => l.LogException(exception));

			var provider = new Mock<IAwsCategoryProvider>();
			provider.Setup(p => p.GetProductsInCategory(It.IsAny<Category>())).Throws(exception);

			var category = new Fixture().Create<Category>();

			var functions = CreateFunctions(logger.Object, provider.Object);

			// Act
			Func<Task> action = () => functions.ProcessCategoryQueue(category, TextWriter.Null, CancellationToken.None);

			// Assert
			action.ShouldThrow<Exception>();
			logger.VerifyAll();
		}

		private static Functions CreateFunctions(IAzureCategoryProvider provider,
												 IFactory<ICsvReader, TextReader> factory,
												 IConverter<CsvCategory, Category> converter)
		{
			return CreateFunctions(null, null, provider, null, factory, converter);
		}

		private static Functions CreateFunctions(ILogger logger = null,
												 IAwsCategoryProvider awsCategoryProvider = null,
												 IAzureCategoryProvider azureCategoryProvider = null,
												 IAzureProductProvider azureProductProvider = null,
												 IFactory<ICsvReader, TextReader> factory = null,
												 IConverter<CsvCategory, Category> converter = null)
		{
			return new Functions(logger ?? Mock.Of<ILogger>(),
								 awsCategoryProvider ?? Mock.Of<IAwsCategoryProvider>(),
								 azureCategoryProvider ?? Mock.Of<IAzureCategoryProvider>(),
								 azureProductProvider ?? Mock.Of<IAzureProductProvider>(),
								 new CsvCategoryParser(factory),
								 converter ?? Mock.Of<IConverter<CsvCategory, Category>>(),
								 new FilterAdapter<Category>(true));
		}
	}
}