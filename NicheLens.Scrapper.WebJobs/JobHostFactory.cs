using Ab.Factory;

using Microsoft.Azure.WebJobs;

namespace NicheLens.Scrapper.WebJobs
{
	public sealed class JobHostFactory : IFactory<JobHost>
	{
		private readonly IFactory<JobHostConfiguration> _configurationFactory;

		public JobHostFactory(IFactory<JobHostConfiguration> configurationFactory)
		{
			_configurationFactory = configurationFactory;
		}

		public JobHost Create()
		{
			return new JobHost(_configurationFactory.Create());
		}
	}
}