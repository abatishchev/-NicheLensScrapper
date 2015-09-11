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

			var categoryProvider = new Mock<IAwsCategoryProvider>();

			var productProvider = new Mock<IAzureProductProvider>();

			var functions = CreateFunctions(categoryProvider.Object, productProvider.Object);

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
																	IAzureProductProvider azureProductProvider = null,
																	ILogger logger = null)
		{
			return new ProcessCategoryQueueFunction(logger ?? Mock.Of<ILogger>(),
													awsCategoryProvider ?? Mock.Of<IAwsCategoryProvider>(),
													azureProductProvider ?? Mock.Of<IAzureProductProvider>());
		}
	}
}