using Ab.Factory;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace NicheLens.Scrapper.WebJobs.Configuration
{
	public sealed class JobHostFactory : IFactory<JobHost>
	{
		private readonly WebJobsOptions _options;
		private readonly IJobActivator _activator;
		private readonly INameResolver _nameResolver;

		public JobHostFactory(WebJobsOptions options, IJobActivator activator, INameResolver nameResolver)
		{
			_options = options;
			_activator = activator;
			_nameResolver = nameResolver;
		}

		public JobHost Create()
		{
			var configuration = new JobHostConfiguration
			{
				JobActivator = _activator,
				NameResolver = _nameResolver,
				DashboardConnectionString = _options.ConnectionString,
				StorageConnectionString = _options.ConnectionString,
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