using System.Collections.Generic;
using System.IO;

using Ab;
using Ab.Amazon;
using Ab.Amazon.Data;
using Ab.Azure;
using Ab.Configuration;
using Ab.SimpleInjector;

using AutoMapper;
using AutoMapper.Mappers;
using CsvHelper;

using Elmah;
using Elmah.AzureTableStorage;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using NicheLens.Scrapper.WebJobs.Configuration;
using NicheLens.Scrapper.WebJobs.Data;

using SimpleInjector;

namespace NicheLens.Scrapper.WebJobs
{
	public static class ContainerConfig
	{
		public static Container CreateContainer()
		{
			Container container = new Container();

			RegisterTypes(container);
			RegisterMapping(container);

			return container;
		}

		private static void RegisterTypes(Container container)
		{
			#region Configuration
			container.RegisterSingleton<Ab.Configuration.IConfigurationProvider, AppSettingsConfigurationProvider>();
			container.RegisterFactory<WebJobsOptions, WebJobsOptionsFactory>(Lifestyle.Singleton);
			#endregion

			#region Web Jobs
			container.Register<IJobActivator, ContainerJobActivator>(Lifestyle.Singleton);
			container.RegisterFactory<JobHost, JobHostFactory>();
			#endregion

			#region CSV
			container.RegisterSingleton<CsvFactory>();
			container.RegisterFactory<ICsvReader, TextReader, CsvReaderFactory>();
			container.Register<IConverter<CsvCategory, Category>, MappingCsvCategoryConverter>();
			#endregion

			#region  Elmah
			ServiceCenter.Current = c => container;
			container.Register<ErrorLog>(() =>
				new AzureTableStorageErrorLog(
					new Dictionary<string, string>
					{
						{ "connectionString", container.GetInstance<WebJobsOptions>().ConnectionString },
						{ "applicationName", "WebJobs" }
					}));
			#endregion

			#region AutoMapper
			container.RegisterSingleton<ITypeMapFactory, TypeMapFactory>();
			container.RegisterCollection<IObjectMapper>(MapperRegistry.Mappers);

			container.RegisterSingleton<ConfigurationStore>();
			container.Register<IConfiguration>(container.GetInstance<ConfigurationStore>);
			container.RegisterSingleton<AutoMapper.IConfigurationProvider>(container.GetInstance<ConfigurationStore>);

			container.RegisterSingleton<IMappingEngine>(() => new MappingEngine(container.GetInstance<AutoMapper.IConfigurationProvider>()));
			#endregion

			#region Azure
			container.Register<IAzureContainerClient, AzureContainerClient>();

			container.Register<IBlobClient, AzureBlobClient>();
			container.Register<ITableClient, AzureTableClient>();
			container.Register<IQueueClient, AzureQueueClient>();
			container.Register<IDocumentDbClient, DocumentDbClient>();

			container.Register<IAzureClient, AzureClient>();

			container.Register<IConverter<string, Product>, JsonProductConverter>();
			container.Register<IConverter<Category, string>, JsonCategoryConverter>();

			container.Register<IAzureProductProvider, AzureProductProvider>();
			container.Register<IAzureCategoryProvider, AzureCategoryProvider>();
			#endregion
		}

		private static void RegisterMapping(Container container)
		{
			var configuration = container.GetInstance<IConfiguration>();

			Mapper.Initialize(_ =>
			{
				configuration.ConstructServicesUsing(container.GetInstance);

				configuration.AddProfile<CsvCategoryMappingProfile>();
			});
		}
	}
}