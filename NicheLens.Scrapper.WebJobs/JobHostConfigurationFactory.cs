using Ab.Configuration;
using Ab.Factory;

using Microsoft.Azure.WebJobs;

namespace NicheLens.Scrapper.WebJobs
{
	public sealed class JobHostConfigurationFactory : IFactory<JobHostConfiguration>
	{
		private readonly IConfigurationProvider _configurationProvider;

		public JobHostConfigurationFactory(IConfigurationProvider configurationProvider)
		{
			_configurationProvider = configurationProvider;
		}

		public JobHostConfiguration Create()
		{
			var connectionString = _configurationProvider.GetValue("azure:Blob");
			return new JobHostConfiguration
			{
				DashboardConnectionString = connectionString,
				StorageConnectionString = connectionString
			};
		}
	}
}