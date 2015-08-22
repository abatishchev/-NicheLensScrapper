using Ab.Configuration;
using Ab.Factory;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace NicheLens.Scrapper.WebJobs.Configuration
{
	public sealed class JobHostFactory : IFactory<JobHost>
	{
		private readonly IConfigurationProvider _configurationProvider;
		private readonly IJobActivator _activator;

		public JobHostFactory(IConfigurationProvider configurationProvider, IJobActivator activator)
		{
			_configurationProvider = configurationProvider;
			_activator = activator;
		}

		public JobHost Create()
		{
			var connectionString = _configurationProvider.GetValue("azure:Blob");
			var configuration = new JobHostConfiguration
			{
				DashboardConnectionString = connectionString,
				StorageConnectionString = connectionString,
				JobActivator = _activator
			};
			return new JobHost(configuration);
		}
	}
}