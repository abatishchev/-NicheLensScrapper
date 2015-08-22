using System;
using Ab.Configuration;
using FluentAssertions;
using Moq;

using NicheLens.Scrapper.WebJobs.Configuration;

namespace NicheLens.Scrapper.WebJobs.Tests.Configuration
{
	public class WebJobsOptionsFactoryTest
	{
		public void Create_Should_Return_AwsOptions_Having_All_Properties()
		{
			// Arrange
			const string connectionString = "connectionString";
			const int batchSize = 1, maxDequeueCount = 2;
			TimeSpan maxPollingInterval = TimeSpan.FromMinutes(1);

			var configurationProvider = new Mock<IConfigurationProvider>();
			configurationProvider.Setup(p => p.GetValue("azure:Blob")).Returns(connectionString);
			configurationProvider.Setup(p => p.GetValue("azure:Queue:BatchSize")).Returns(batchSize.ToString());
			configurationProvider.Setup(p => p.GetValue("azure:Queue:MaxDequeueCount")).Returns(maxDequeueCount.ToString());
			configurationProvider.Setup(p => p.GetValue("azure:Queue:MaxPollingInterval")).Returns(maxPollingInterval.ToString());

			var factory = new WebJobsOptionsFactory(configurationProvider.Object);

			// Act
			var options = factory.Create();

			// Assert
			options.ConnectionString.Should().Be(connectionString);
			options.BatchSize.Should().Be(batchSize);
			options.MaxDequeueCount.Should().Be(maxDequeueCount);
			options.MaxPollingInterval.Should().Be(maxPollingInterval);
		}
	}
}
