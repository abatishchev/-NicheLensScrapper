using System.Linq;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Http.Validation;
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
using Ab.Reflection;
using Ab.SimpleInjector;
using Ab.Threading;
using Ab.Validation;
using Ab.Web;
using Ab.WebApi.AppInsights;

using FluentValidation.Attributes;
using FluentValidation.WebApi;

using Microsoft.ApplicationInsights;
using Microsoft.WindowsAzure.Storage.Table;

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

			return container;
		}

		private static void RegisterTypes(Container container)
		{
			#region Providers
			container.RegisterSingleton<IConfigurationProvider, WebConfigurationProvider>();

			container.RegisterSingleton<IEnvironmentProvider, ConfigurationEnvironmentProvider>();

			container.RegisterSingleton<IConnectionStringProvider, ConfigurationConnectionStringProvider>();

			container.RegisterSingleton<IDateTimeProvider, UtcDateTimeProvider>();

			container.RegisterSingleton<IAssemblyProvider, ReflectionAssemblyProvider>();
			#endregion

			#region Configuration
			container.RegisterFactory<AzureOptions, AzureOptionsFactory>(Lifestyle.Singleton);

			container.Register<IConverter<DynamicTableEntity, AwsOptions>, DynamicAwsOptionsConverter>();
			container.Register<IOptionsProvider<AwsOptions>, AzureAwsOptionsProvider>();
			container.RegisterDecorator<IOptionsProvider<AwsOptions>, OptionsProviderAdapter<AwsOptions>>(Lifestyle.Singleton);
			container.RegisterFactory<AwsOptions, RoundrobinAwsOptionsFactory>(Lifestyle.Singleton);
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
			container.RegisterCollection<IExceptionLogger>(
				new[]
				{
					typeof(AiExceptionLogger),
					typeof(Elmah.Contrib.WebApi.ElmahExceptionLogger)
				});

			// Controllers
			container.RegisterWebApiControllers(GlobalConfiguration.Configuration, Assembly.GetExecutingAssembly());
			#endregion

			#region AppInsights
			container.Register<TelemetryClient>(() => new TelemetryClient());
			#endregion

			#region Fluent Validation
			container.Register<FluentValidation.IValidatorFactory, AttributedValidatorFactory>();
			container.Register<ModelValidatorProvider, FluentValidationModelValidatorProvider>();
			#endregion

			#region Azure
			container.Register<IAzureContainerClient, AzureContainerClient>();
			container.Register<IBlobClient, AzureBlobClient>();
			container.Register<ITableClient, AzureTableClient>();
			container.Register<IQueueClient, AzureQueueClient>();
			container.Register<IAzureClient, AzureClient>();

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

			container.RegisterSingleton<IScheduler>(Scheduler.Default);
			container.Register<IRequestScheduler, IntervalRequestScheduler>();
			container.Register<HttpClient>(() => HttpClientFactory.Create());
			container.Register<IHttpClient, HttpClientAdapter>();
			container.RegisterDecorator<IHttpClient, ThrottlingHttpClient>();

			container.Register<IValidator<XElement>, XmlRequestValidator>();
			container.Register<IItemSelector, XmlItemSelector>();
			container.Register<IPipeline<Product, XElement, SearchCriteria>, ResponseGroupProductPipeline>();
			container.RegisterFactory<Product, XElement, SearchCriteria, XmlProductFactory>();
			container.Register<IFilter<XElement>, PrimaryVariantlItemFilter>();

			container.Register<IAwsClient, XmlAwsClient>();
			container.Register<IAwsCategoryProvider, AwsCategoryProvider>();

			container.Register<IConverter<Category, string>, JsonCategoryConverter>();
			#endregion
		}
	}
}