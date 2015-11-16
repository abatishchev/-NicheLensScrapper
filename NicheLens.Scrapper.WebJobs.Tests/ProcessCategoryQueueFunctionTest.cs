using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Ab;
using Ab.Amazon;
using Ab.Amazon.Data;

using FluentAssertions;
using Moq;

using NicheLens.Scrapper.Data;

using Ploeh.AutoFixture;
using Xunit;

using AmazonProduct = Ab.Amazon.Data.Product;
using ModelProduct = NicheLens.Scrapper.Data.Models.Product;

namespace NicheLens.Scrapper.WebJobs.Tests
{
	public class ProcessCategoryQueueFunctionTest
	{
		[Fact]
		public async Task ProcessCategoryQueue_Should_Get_Products_And_Save_Products()
		{
			// Arrange
			var fixture = new Fixture();
			var category = fixture.Create<Category>();

			var categoryProvider = new Mock<IAwsCategoryProvider>();

			var productRepository = new Mock<IProductRepository>();

			var functions = CreateFunctions(categoryProvider.Object, productRepository: productRepository.Object);

			// Act
			await functions.ProcessCategoryQueue(category, TextWriter.Null, CancellationToken.None);

			// Assert
			categoryProvider.VerifyAll();
			productRepository.VerifyAll();
		}

		[Fact]
		public void ProcessCategoryQueue_Should_Log_Exception()
		{
			// Arrange
			var exception = new Exception();

			var logger = new Mock<ILogger>();
			logger.Setup(l => l.LogException(exception));

			var categoryProvider = new Mock<IAwsCategoryProvider>();
			categoryProvider.Setup(p => p.GetProductsInCategory(It.IsAny<Category>())).Throws(exception);

			var category = new Fixture().Create<Category>();

			var functions = CreateFunctions(categoryProvider.Object, logger: logger.Object);

			// Act
			Func<Task> action = () => functions.ProcessCategoryQueue(category, TextWriter.Null, CancellationToken.None);

			// Assert
			action.ShouldThrow<Exception>();
			logger.VerifyAll();
		}

		private static ProcessCategoryQueueFunction CreateFunctions(IAwsCategoryProvider awsCategoryProvider = null,
																	IConverter<AmazonProduct, ModelProduct> productConverter = null,
																	IProductRepository productRepository = null,
																	ILogger logger = null)
		{
			return new ProcessCategoryQueueFunction(logger ?? Mock.Of<ILogger>(),
													awsCategoryProvider ?? Mock.Of<IAwsCategoryProvider>(),
													productConverter ?? Mock.Of<IConverter<AmazonProduct, ModelProduct>>(),
													productRepository ?? Mock.Of<IProductRepository>());
		}
	}
}