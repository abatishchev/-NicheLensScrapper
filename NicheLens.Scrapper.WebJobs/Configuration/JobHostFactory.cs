using Ab.Factory;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace NicheLens.Scrapper.WebJobs.Configuration
{
	public sealed class JobHostFactory : IFactory<JobHost>
	{
		private readonly WebJobsOptions _options;
		private readonly IJobActivator _activator;

		public JobHostFactory(WebJobsOptions options, IJobActivator activator)
		{
			_options = options;
			_activator = activator;
		}

		public JobHost Create()
		{
			var configuration = new JobHostConfiguration
			{
				DashboardConnectionString = _options.ConnectionString,
				StorageConnectionString = _options.ConnectionString,
				JobActivator = _activator,
				Queues =
				{
					MaxPollingInterval = _options.MaxPollingInterval,
					MaxDequeueCount = _options.MaxDequeueCount,
					BatchSize = _options.BatchSize
				}
			};
			return new JobHost(configuration);
		}
	}
}