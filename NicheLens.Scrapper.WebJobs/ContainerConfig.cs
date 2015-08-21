using Ab.Configuration;
using Ab.SimpleInjector;

using Microsoft.Azure.WebJobs;

using SimpleInjector;

namespace NicheLens.Scrapper.WebJobs
{
	public static class ContainerConfig
	{
		public static Container CreateContainer()
		{
			Container container = new Container();

			RegisterTypes(container);

			return container;
		}

		private static void RegisterTypes(Container container)
		{
			// Providers
			container.RegisterSingleton<IConfigurationProvider, AppSettingsConfigurationProvider>();

			// WEb Jobs
			container.RegisterFactory<JobHost, JobHostFactory>();
			container.RegisterFactory<JobHostConfiguration, JobHostConfigurationFactory>();
		}
	}
}