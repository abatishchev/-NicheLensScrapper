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
			container.Verify(VerificationOption.VerifyOnly);
		}
	}
}