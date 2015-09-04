using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Validation;

using Ab;
using Ab.Amazon;
using Ab.Amazon.Data;
using Ab.Azure;
using Ab.Azure.Configuration;
using Ab.Configuration;
using Ab.SimpleInjector;
using Ab.Threading;
using Ab.Web;
using Ab.WebApi.AppInsights;

using AutoMapper;
using AutoMapper.Mappers;

using Elmah;
using Elmah.AzureTableStorage;

using FluentValidation.Attributes;
using FluentValidation.WebApi;

using Microsoft.ApplicationInsights;

using SimpleInjector;
using SimpleInjector.Integration.WebApi;

namespace NicheLens.Scrapper.Api
{
	public static class ContainerConfig
	{
		public static Container CreateContainer()
		{
			Container container = new Container();
			container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();

			RegisterTypes(container);
			RegisterMapping(container);

			return container;
		}

		private static void RegisterTypes(Container container)
		{
			#region Providers
			container.RegisterSingleton<IDateTimeProvider, UtcDateTimeProvider>();
			#endregion

			#region Configuration
			container.RegisterSingleton<Ab.Configuration.IConfigurationProvider, WebConfigurationProvider>();
			#endregion

			#region Web API
			// Filters
			container.RegisterCollection<IFilter>(
				new[]
				{
					typeof(AiTrackingFilter),
					typeof(WebApiContrib.Filters.ValidationAttribute)
				});

			// Handlers
			container.RegisterCollection<DelegatingHandler>(Enumerable.Empty<DelegatingHandler>());

			// Services
			container.RegisterCollection<System.Web.Http.ExceptionHandling.IExceptionLogger>(
				new[]
				{
					typeof(AiExceptionLogger),
					typeof(Elmah.Contrib.WebApi.ElmahExceptionLogger)
				});

			// Controllers
			container.RegisterWebApiControllers(GlobalConfiguration.Configuration, Assembly.GetExecutingAssembly());
			#endregion

			#region  Elmah
			ServiceCenter.Current = c => container;
			container.Register<ErrorLog>(() =>
				new AzureTableStorageErrorLog(
					new Dictionary<string, string>
					{
						{ "connectionString", container.GetInstance<Ab.Configuration.IConfigurationProvider>().GetValue("azure:Container:ConnectionString") },
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

			#region AppInsights
			container.Register<TelemetryClient>(() => new TelemetryClient());
			#endregion

			#region Fluent Validation
			container.Register<FluentValidation.IValidatorFactory, AttributedValidatorFactory>();
			container.Register<ModelValidatorProvider, FluentValidationModelValidatorProvider>();
			#endregion

			#region Scheduler
			container.RegisterFactory<ITaskScheduler, TaskSchedulerSettings, DelayTaskSchedulerFactory>(Lifestyle.Singleton);
			container.RegisterSingleton<IScheduler>(Scheduler.Default);
			container.Register<ITaskScheduler, DelayTaskScheduler>();
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

			container.Register<IStringBuilder<string>, DatabaseLinkBuilder>(Lifestyle.Singleton);
			container.Register<IStringBuilder<string, string>, CollectionLinkBuilder>(Lifestyle.Singleton);
			container.Register<IStringBuilder<string, string, string>, DocumentLinkBuilder>(Lifestyle.Singleton);

			container.RegisterFactory<DocumentDbOptions, DocumentDbOptionsFactory>(Lifestyle.Singleton);
			container.RegisterInitializer((DocumentDbOptions opt) =>
			{
				(opt.DatabaseOptions = new DatabaseOptionsCollection(container.GetInstance))
					.AddDatabase("Scrapper", "88AvAA==")
						.AddCollection("Categories", "88AvAL3WZgA=", 250)
						.AddCollection("Products", "88AvAI2XrgA=", 250);
			});
			container.Register<IPartitionResolverProvider, CategoryPartitionResolverProvider>();
			container.Register<IExceptionHandler, DocumentClientExceptionHandler>();
			container.Register<IDocumentDbClient, DocumentDbClient>();
			container.RegisterDecorator<IDocumentDbClient, ExceptionHandlingDocumentDbClientAdapter>();

			container.Register<IAzureClient, AzureClient>();

			container.Register<IConverter<string, Product>, JsonProductConverter>();
			container.Register<IConverter<Category, string>, JsonCategoryConverter>();
			container.Register<IConverter<Category, CategoryDocument>, MappingCategoryDocumentConverter>();
			container.Register<IAzureProductProvider, AzureProductProvider>();

			container.Register<IAzureCategoryProvider, AzureCategoryProvider>();
			#endregion

			#region Amazon
			#endregion
		}

		private static void RegisterMapping(Container container)
		{
			var configuration = container.GetInstance<IConfiguration>();

			Mapper.Initialize(_ =>
			{
				configuration.ConstructServicesUsing(container.GetInstance);

				configuration.AddProfile<CategoryDocumentMappingProfile>();
			});
		}
	}
}