using System;
using System.Web.Http;

using SimpleInjector;
using Xunit;
using FluentAssertions;

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
			Action action = () => container.Verify(VerificationOption.VerifyOnly);

			// Asssert
			action.ShouldNotThrow();
		}
	}
}
