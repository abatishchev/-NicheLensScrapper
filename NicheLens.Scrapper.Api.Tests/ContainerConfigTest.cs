using System.Web.Http;

using FluentAssertions;

using SimpleInjector;
using SimpleInjector.Integration.WebApi;

using Xunit;

namespace NicheLens.Scrapper.Api.Tests
{
	public class ContainerConfigTest
	{
		[Fact]
		public void Container_Verify_Should_Not_Throw_Exception()
		{
			// Arrange
			var container = ContainerConfig.CreateContainer();
			WebApiConfig.Configure(new HttpConfiguration(), container);

			// Act
			container.Verify(VerificationOption.VerifyOnly);
		}

		[Fact]
		public void Container_DefaultScopedLifestyle_Should_Be_Of_Type_WebApiRequestLifestyle()
		{
			// Arrange
			var container = ContainerConfig.CreateContainer();

			// Act
			var lifestyle = container.Options.DefaultScopedLifestyle;

			// Act
			lifestyle.Should().BeOfType<WebApiRequestLifestyle>();
		}
	}
}