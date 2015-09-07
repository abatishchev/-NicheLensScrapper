using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Xml.Linq;

using Ab;
using Ab.Amazon;
using Ab.Amazon.Configuration;
using Ab.Amazon.Cryptography;
using Ab.Amazon.Data;
using Ab.Amazon.Filtering;
using Ab.Amazon.Pipeline;
using Ab.Amazon.Validation;
using Ab.Amazon.Web;
using Ab.Azure;
using Ab.Azure.Configuration;
using Ab.Configuration;
using Ab.Filtering;
using Ab.Pipeline;
using Ab.SimpleInjector;
using Ab.Threading;
using Ab.Validation;
using Ab.Web;

using AutoMapper;
using AutoMapper.Mappers;

using CsvHelper;

using Elmah;
using Elmah.AzureTableStorage;

using Microsoft.ApplicationInsights;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;

using NicheLens.Scrapper.Api.Client;
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
			container.RegisterSingleton<Ab.Configuration.IConfigurationProvider, WebConfigurationProvider>();

			container.RegisterFactory<WebJobsOptions, WebJobsOptionsFactory>(Lifestyle.Singleton);

			container.RegisterSingleton<IConverter<DynamicTableEntity, AwsOptions>, DynamicAwsOptionsConverter>();
			container.RegisterSingleton<IOptionsProvider<AwsOptions[]>, AzureAwsOptionsProvider>();
			container.RegisterDecorator<IOptionsProvider<AwsOptions>, LazyOptionsProviderAdapter<AwsOptions>>(Lifestyle.Singleton);
			container.RegisterFactory<AwsOptions, RoundrobinAwsOptionsFactory>(Lifestyle.Singleton);
			#endregion

			#region Providers
			container.RegisterSingleton<IEnvironmentProvider, ConfigurationEnvironmentProvider>();

			container.RegisterSingleton<IDateTimeProvider, UtcDateTimeProvider>();
			#endregion

			#region WebJobs
			container.RegisterSingleton<IJobActivator, ContainerJobActivator>();
			container.RegisterFactory<JobHost, JobHostFactory>();
			#endregion

			#region Scrapper Api
			container.RegisterFactory<IScrapperApi, ScrapperApiFactory>();
			#endregion

			#region CSV
			container.RegisterSingleton<CsvFactory>();
			container.RegisterFactory<ICsvReader, TextReader, CsvReaderFactory>();
			container.Register<IConverter<CsvCategory, Category>, MappingCsvCategoryConverter>();
			container.Register<IFilter<Category>, EmptySearchIndexCategoryFIlter>();
			#endregion

			#region AppInsights
			container.Register<TelemetryClient>(() => new TelemetryClient());
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

			#region Raygun
			container.Register(() => new Mindscape.Raygun4Net.RaygunClient(container.GetInstance<Ab.Configuration.IConfigurationProvider>().GetValue("raygun:ApiKey")));
			#endregion

			#region AutoMapper
			container.RegisterSingleton<ITypeMapFactory, TypeMapFactory>();
			container.RegisterCollection<IObjectMapper>(MapperRegistry.Mappers);

			container.RegisterSingleton<ConfigurationStore>();
			container.Register<IConfiguration>(container.GetInstance<ConfigurationStore>);
			container.RegisterSingleton<AutoMapper.IConfigurationProvider>(container.GetInstance<ConfigurationStore>);

			container.RegisterSingleton<IMappingEngine>(() => new MappingEngine(container.GetInstance<AutoMapper.IConfigurationProvider>()));
			#endregion

			#region Scheduler
			container.RegisterFactory<ITaskScheduler, TaskSchedulerSettings, DelayTaskSchedulerFactory>(Lifestyle.Singleton);
			container.RegisterSingleton<IScheduler>(Scheduler.Default);
			container.RegisterSingleton<ITaskScheduler, DelayTaskScheduler>();
			#endregion

			#region Http
			container.Register<HttpClient>(() => HttpClientFactory.Create());
			container.Register<IHttpClient, HttpClientAdapter>();
			container.RegisterDecorator<IHttpClient, ThrottlingHttpClient>();
			#endregion

			#region Azure
			container.RegisterFactory<AzureOptions, AzureOptionsFactory>(Lifestyle.Singleton);

			container.Register<IAzureContainerClient, AzureContainerClient>();

			container.Register<IBlobClient, AzureBlobClient>();
			container.Register<ITableClient, AzureTableClient>();
			container.Register<IQueueClient, AzureQueueClient>();

			container.RegisterSingleton<IStringBuilder<string>, DatabaseLinkBuilder>();
			container.RegisterSingleton<IStringBuilder<string, string>, CollectionLinkBuilder>();
			container.RegisterSingleton<IStringBuilder<string, string, string>, DocumentLinkBuilder>();

			container.RegisterFactory<DocumentDbOptions, DocumentDbOptionsFactory>(Lifestyle.Singleton);
			container.RegisterInitializer((DocumentDbOptions opt) =>
			{
				(opt.DatabaseOptions = new DatabaseOptionsCollection(container.GetInstance))
					.AddDatabase("Scrapper", "88AvAA==")
						.AddCollection("Categories", "88AvAL3WZgA=", 250)
						.AddCollection("Products", "88AvAI2XrgA=", 250);
			});
			container.RegisterSingleton<IPartitionResolverProvider, CategoryPartitionResolverProvider>();
			container.RegisterFactory<DocumentClient, DocumentClientFactory>();
			container.Register<IDocumentDbClient, ReliableDocumentDbClient>();

			container.Register<IAzureClient, AzureClient>();

			container.Register<IConverter<string, Product>, JsonProductConverter>();
			container.Register<IConverter<Category, string>, JsonCategoryConverter>();
			container.Register<IConverter<Category, CategoryDocument>, MappingCategoryDocumentConverter>();
			container.Register<IAzureProductProvider, AzureProductProvider>();

			container.Register<IAzureCategoryProvider, AzureCategoryProvider>();
			#endregion

			#region Amazon
			container.Register<IArgumentBuilder, AwsArgumentBuilder>();
			container.Register<IPipeline<string>, PercentUrlEncodingPipeline>();
			container.Register<IUrlEncoder, PercentUrlEncoder>();
			container.Register<IQueryBuilder, EncodedQueryBuilder>();
			container.RegisterFactory<System.Security.Cryptography.HashAlgorithm, AwsAlgorithmFactory>();
			container.Register<IQuerySigner, AwsQuerySigner>();
			container.Register<IUrlBuilder, AwsUrlBuilder>();

			container.Register<IValidator<XElement>, XmlRequestValidator>();
			container.Register<IItemSelector, XmlItemSelector>();
			container.Register<IPipeline<Product, XElement, SearchCriteria>, ResponseGroupProductPipeline>();
			container.RegisterFactory<Product, XElement, SearchCriteria, XmlProductFactory>();
			container.Register<IFilter<XElement>, PrimaryVariantlItemFilter>();

			container.Register<IAwsClient, XmlAwsClient>();

			container.Register<IAwsProductProvider, AwsProductProvider>();
			#endregion
		}

		private static void RegisterMapping(Container container)
		{
			var configuration = container.GetInstance<IConfiguration>();

			Mapper.Initialize(_ =>
			{
				configuration.ConstructServicesUsing(container.GetInstance);

				configuration.AddProfile<CsvCategoryMappingProfile>();
				configuration.AddProfile<CategoryDocumentMappingProfile>();
			});
		}
	}
}