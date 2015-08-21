using Microsoft.Azure.WebJobs;

namespace NicheLens.Scrapper.WebJobs
{
	class Program
	{
		static void Main()
		{
			var container = ContainerConfig.CreateContainer();

			var host = container.GetInstance<JobHost>();

			host.RunAndBlock();
		}
	}
}