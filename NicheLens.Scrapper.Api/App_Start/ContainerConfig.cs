using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Http.Validation;

using Ab;
using Ab.Configuration;
using Ab.WebApi.AppInsights;

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

			return container;
		}

		private static void RegisterTypes(Container container)
		{
			#region Providers
			container.RegisterSingleton<IDateTimeProvider, UtcDateTimeProvider>();
			#endregion

			#region Configuration
			container.RegisterSingleton<IConfigurationProvider, WebConfigurationProvider>();
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

			#region  Elmah
			ServiceCenter.Current = c => container;
			container.Register<ErrorLog>(() =>
				new AzureTableStorageErrorLog(
					new Dictionary<string, string>
					{
						{ "connectionString", container.GetInstance<IConfigurationProvider>().GetValue("azure:Container:ConnectionString") },
						{ "applicationName", "WebJobs" }
					}));
			#endregion

			#region AppInsights
			container.Register<TelemetryClient>(() => new TelemetryClient());
			#endregion

			#region Fluent Validation
			container.Register<FluentValidation.IValidatorFactory, AttributedValidatorFactory>();
			container.Register<ModelValidatorProvider, FluentValidationModelValidatorProvider>();
			#endregion

			#region Azure
			#endregion

			#region Amazon
			#endregion
		}
	}
}