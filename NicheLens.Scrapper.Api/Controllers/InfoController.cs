using System.Web.Http;

using Ab.Configuration;
using Ab.Reflection;

namespace NicheLens.Scrapper.Api.Controllers
{
	public class InfoController : ApiController
	{
		private readonly IAssemblyProvider _assemblyProvider;
		private readonly IEnvironmentProvider _environmentProvider;

		public InfoController(IAssemblyProvider assemblyProvider, IEnvironmentProvider environmentProvider)
		{
			_assemblyProvider = assemblyProvider;
			_environmentProvider = environmentProvider;
		}

		[Route("api/info/version")]
		public IHttpActionResult GetVersion()
		{
			return Ok(_assemblyProvider.GetCallingAssembly().GetName().Version.ToString());
		}

		[Route("api/info/environment")]
		public IHttpActionResult GetEnvironment()
		{
			return Ok(_environmentProvider.GetCurrentEnvironment());
		}
	}
}