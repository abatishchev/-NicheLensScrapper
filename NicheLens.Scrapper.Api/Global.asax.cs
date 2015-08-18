using System.Web.Http;

namespace NicheLens.Scrapper.Api
{
	public class GlobalApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			var container = ContainerConfig.CreateContainer();

			GlobalConfiguration.Configure(config => WebApiConfig.Configure(config, container));
		}
	}
}