using Ab.Configuration;
using Microsoft.Azure.WebJobs;

namespace NicheLens.Scrapper.WebJobs.Configuration
{
	public sealed class ConfigurationNameResolver : INameResolver
	{
		private readonly IConfigurationProvider _configurationProvider;

		public ConfigurationNameResolver(IConfigurationProvider configurationProvider)
		{
			_configurationProvider = configurationProvider;
		}

		public string Resolve(string name)
		{
			return _configurationProvider.GetValue(name);
		}
	}
}