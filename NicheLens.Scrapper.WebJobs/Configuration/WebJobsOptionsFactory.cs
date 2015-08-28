using System;
using Ab.Configuration;
using Ab.Factory;

namespace NicheLens.Scrapper.WebJobs.Configuration
{
	public sealed class WebJobsOptionsFactory : IFactory<WebJobsOptions>
	{
		private readonly IConfigurationProvider _configurationProvider;

		public WebJobsOptionsFactory(IConfigurationProvider configurationProvider)
		{
			_configurationProvider = configurationProvider;
		}

		public WebJobsOptions Create()
		{
			return new WebJobsOptions
			{
				ConnectionString = _configurationProvider.GetValue("azure:Container:ConnectionString"),

				BatchSize = Int32.Parse(_configurationProvider.GetValue("azure:Queue:BatchSize")),
				MaxDequeueCount = Int32.Parse(_configurationProvider.GetValue("azure:Queue:MaxDequeueCount")),
				MaxPollingInterval = TimeSpan.Parse(_configurationProvider.GetValue("azure:Queue:MaxPollingInterval"))
			};
		}
	}
}