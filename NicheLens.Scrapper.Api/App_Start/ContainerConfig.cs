using System.Reflection;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Http.Validation;

using Ab;
using Ab.Amazon.Configuration;
using Ab.Azure.Configuration;
using Ab.Configuration;
using Ab.Reflection;
using Ab.SimpleInjector;

using Elmah.Contrib.WebApi;
using FluentValidation.Attributes;
using FluentValidation.WebApi;

using Microsoft.WindowsAzure.Storage.Table;

using SimpleInjector;
using SimpleInjector.Extensions;

namespace NicheLens.Scrapper.Api
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
			#region Providers
			container.RegisterSingle<IConfigurationProvider, WebConfigurationProvider>();

			container.RegisterSingle<IEnvironmentProvider, ConfigurationEnvironmentProvider>();

			container.RegisterSingle<IConnectionStringProvider, ConfigurationConnectionStringProvider>();

			container.RegisterSingle<IDateTimeProvider, UtcDateTimeProvider>();

			container.RegisterSingle<IAssemblyProvider, ReflectionAssemblyProvider>();
			#endregion

			#region Configuration
			container.RegisterFactory<AzureOptions, AzureOptionsFactory>();

			container.Register<IConverter<DynamicTableEntity, AwsOptions>, DynamicAwsOptionsConverter>();
			container.Register<IOptionsProvider<AwsOptions>, AzureAwsOptionsProvider>();
			container.RegisterDecorator(typeof(IOptionsProvider<AwsOptions>), typeof(CachingOptionsProvider<AwsOptions>));
			container.RegisterFactory<AwsOptions, RoundrobinAwsOptionsFactory>(Lifestyle.Singleton);
			#endregion

			#region Web API
			// Filters
			container.RegisterAll<IFilter>(typeof(WebApiContrib.Filters.ValidationAttribute));

			// Handlers

			// Services
			container.Register<IExceptionLogger, ElmahExceptionLogger>();

			// Controllers
			// TODO: remove arr once the new version is pushed
			container.RegisterWebApiControllers(GlobalConfiguration.Configuration, new[] { Assembly.GetExecutingAssembly() });
			#endregion

			#region Fluent Validation
			container.Register<FluentValidation.IValidatorFactory, AttributedValidatorFactory>();
			container.Register<ModelValidatorProvider, FluentValidationModelValidatorProvider>();
			#endregion
		}
	}
}