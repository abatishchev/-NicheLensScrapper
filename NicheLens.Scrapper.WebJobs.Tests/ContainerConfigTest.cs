using AutoMapper;
using FluentAssertions;
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
			container.Verify();
		}

		[Fact]
		public void GetSIntance_Functions_Should_Not_Throw_Exception()
		{
			// Arrange
			var container = ContainerConfig.CreateContainer();

			// Act
			var functions = container.GetInstance<Functions>();

			// Arrange
			functions.Should().NotBeNull();
		}

		[Fact]
		public void Mapper_AssertConfigurationIsValid_Should_Not_Throw_Exception()
		{
			// Arrange
			var container = ContainerConfig.CreateContainer();

			// Act
			// Arrange
			Mapper.AssertConfigurationIsValid();
		}
	}
}