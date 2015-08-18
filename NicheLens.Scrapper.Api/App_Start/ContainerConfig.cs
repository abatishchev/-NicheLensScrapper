using System.Reflection;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Http.Validation;

using Ab;
using Ab.Amazon.Configuration;
using Ab.Azure;
using Ab.Azure.Configuration;
using Ab.Configuration;
using Ab.Reflection;
using Ab.SimpleInjector;

using Elmah.Contrib.WebApi;
using FluentValidation.Attributes;
using FluentValidation.WebApi;

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
			container.RegisterCollection<IFilter>(new[] { typeof(WebApiContrib.Filters.ValidationAttribute) });

			// Handlers

			// Services
			container.Register<IExceptionLogger, ElmahExceptionLogger>();

			// Controllers
			container.RegisterWebApiControllers(GlobalConfiguration.Configuration, Assembly.GetExecutingAssembly());
			#endregion

			#region Fluent Validation
			container.Register<FluentValidation.IValidatorFactory, AttributedValidatorFactory>();
			container.Register<ModelValidatorProvider, FluentValidationModelValidatorProvider>();
			#endregion

			#region Azure
			container.Register<IBlobClient, AzureBlobClient>();
			container.Register<ITableClient, AzureTableClient>();
			container.Register<IAzureContainerClient, AzureContainerClient>();
			container.Register<IAzureClient, AzureClient>();
			#endregion
		}
	}
}