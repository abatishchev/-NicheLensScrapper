using System.Collections.Generic;
using System.IO;

using Ab.Configuration;
using Ab.SimpleInjector;

using CsvHelper;

using Elmah;
using Elmah.AzureTableStorage;

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
			// Configuration
			container.RegisterSingleton<IConfigurationProvider, AppSettingsConfigurationProvider>();
			container.RegisterFactory<WebJobsOptions, WebJobsOptionsFactory>(Lifestyle.Singleton);

			// Web Jobs
			container.Register<IJobActivator, ContainerJobActivator>(Lifestyle.Singleton);
			container.RegisterFactory<JobHost, JobHostFactory>();

			// CSV
			container.RegisterSingleton<CsvFactory>();
			container.RegisterFactory<ICsvReader, TextReader, Data.CsvReaderFactory>();

			// Elmah
			ServiceCenter.Current = c => container;
			container.Register<ErrorLog>(() =>
				new AzureTableStorageErrorLog(
					new Dictionary<string, string>
					{
						{ "connectionString", container.GetInstance<WebJobsOptions>().ConnectionString },
						{ "applicationName", "WebJobs" }
					}));
		}
	}
}