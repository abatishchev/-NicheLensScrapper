using System;

using Microsoft.ApplicationInsights;
using Microsoft.Azure.WebJobs;

using Mindscape.Raygun4Net;
using SimpleInjector;

namespace NicheLens.Scrapper.WebJobs
{
	class Program
	{
		static void Main()
		{
			var container = ContainerConfig.CreateContainer();

			AppDomain.CurrentDomain.UnhandledException += (s, e) => CurrentDomain_UnhandledException(container, (Exception)e.ExceptionObject);

			var host = container.GetInstance<JobHost>();
			host.RunAndBlock();
		}

		private static void CurrentDomain_UnhandledException(Container container, Exception exception)
		{
			container.GetInstance<TelemetryClient>().TrackException(exception);
			container.GetInstance<RaygunClient>().Send(exception);
		}
	}
}