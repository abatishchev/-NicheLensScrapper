using System;

using Microsoft.Azure.WebJobs;

using Mindscape.Raygun4Net;

namespace NicheLens.Scrapper.WebJobs
{
	class Program
	{
		static void Main()
		{
			var container = ContainerConfig.CreateContainer();

			AppDomain.CurrentDomain.UnhandledException += (s, e) => container.GetInstance<RaygunClient>().Send((Exception)e.ExceptionObject);

			var host = container.GetInstance<JobHost>();

			host.RunAndBlock();
		}
	}
}