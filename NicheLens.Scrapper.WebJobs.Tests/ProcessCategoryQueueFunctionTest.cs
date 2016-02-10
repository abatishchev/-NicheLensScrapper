using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Ab;
using Ab.Amazon;
using Ab.Amazon.Data;

using FluentAssertions;
using Moq;

using Ploeh.AutoFixture;
using Xunit;

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

			var provider = new Mock<IAwsCategoryProvider>();

			var repository = new Mock<IProductRepository>();

			var functions = CreateFunctions(provider.Object, productRepository: repository.Object);

			// Act
			await functions.ProcessCategoryQueue(category, TextWriter.Null, CancellationToken.None);

			// Assert
			provider.VerifyAll();
			repository.VerifyAll();
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

			var functions = CreateFunctions(provider.Object, logger: logger.Object);

			// Act
			Func<Task> action = () => functions.ProcessCategoryQueue(category, TextWriter.Null, CancellationToken.None);

			// Assert
			action.ShouldThrow<Exception>();
			logger.VerifyAll();
		}

		private static ProcessCategoryQueueFunction CreateFunctions(IAwsCategoryProvider awsCategoryProvider = null,
																	IConverter<Product, ProductEntity> productConverter = null,
																	IProductRepository productRepository = null,
																	ILogger logger = null)
		{
			return new ProcessCategoryQueueFunction(logger ?? Mock.Of<ILogger>(),
													awsCategoryProvider ?? Mock.Of<IAwsCategoryProvider>(),
													productConverter ?? Mock.Of<IConverter<Product, ProductEntity>>(),
													productRepository ?? Mock.Of<IProductRepository>());
		}
	}
}