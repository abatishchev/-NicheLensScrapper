using AutoMapper;
using SimpleInjector;
using Xunit;

namespace NicheLens.Scrapper.WebJobs.Tests
{
	public class ContainerConfigTest
	{
		[Fact]
		public void Container_Verify_Should_Not_Throw_Exception()
		{
			// Arrange
			var container = ContainerConfig.CreateContainer();

			// Act
			// Assert
			container.Verify(VerificationOption.VerifyOnly);
		}

		[Fact]
		public void MapperConfiguration_AssertConfigurationIsValid_Should_Not_Throw_Exception()
		{
			// Arrange
			var container = ContainerConfig.CreateContainer();
			var config = container.GetInstance<MapperConfiguration>();

			// Act
			// Assert
			config.AssertConfigurationIsValid();
		}
	}
}