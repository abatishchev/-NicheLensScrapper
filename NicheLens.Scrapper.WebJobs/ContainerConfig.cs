using System.IO;

using Ab.Configuration;
using Ab.SimpleInjector;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using NicheLens.Scrapper.WebJobs.Configuration;

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

			// Web Jobs
			container.Register<IJobActivator, ContainerJobActivator>(Lifestyle.Singleton);
			container.RegisterFactory<JobHost, JobHostFactory>();

			container.RegisterFactory<CsvHelper.ICsvReader, TextReader, Data.CsvReaderFactory>();
		}
	}
}