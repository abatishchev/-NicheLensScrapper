using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Validation;

using Ab;
using Ab.Amazon.Data;
using Ab.Configuration;
using Ab.Reflection;
using Ab.SimpleInjector;
using Ab.Threading;
using Ab.Web;
using Ab.WebApi.AppInsights;

using AutoMapper;

using Elmah;
using Elmah.AzureTableStorage;

using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.WebApi;

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
			container.RegisterSingleton<IEnvironmentProvider, ConfigurationEnvironmentProvider>();
			container.RegisterSingleton<IDateTimeProvider, UtcDateTimeProvider>();
			container.RegisterSingleton<IAssemblyProvider, ReflectionAssemblyProvider>();
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
			container.RegisterCollection<DelegatingHandler>(
				new Type[]
				{
				});

			// Services
			container.RegisterCollection<System.Web.Http.ExceptionHandling.IExceptionLogger>(
				new[]
				{
					typeof(Elmah.Contrib.WebApi.ElmahExceptionLogger),
					typeof(AiExceptionLogger)
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

			#region AppInsights
			container.Register(() => new Microsoft.ApplicationInsights.TelemetryClient());
			#endregion

			#region AutoMapper
			container.RegisterSingleton(new MapperConfiguration(c => CreateMapperConfiguration(c, container)));
			container.RegisterSingleton(() => container.GetInstance<MapperConfiguration>().CreateMapper());
			#endregion

			#region Fluent Validation
			container.Register<IValidatorFactory>(() => new AttributedValidatorFactory(t => (IValidator)container.GetInstance(t)));
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
			#endregion
		}

		private static void CreateMapperConfiguration(IMapperConfiguration config, Container container)
		{
			config.ConstructServicesUsing(container.GetInstance);

			config.AddProfile<CategoryDocumentMappingProfile>();
		}
	}
}