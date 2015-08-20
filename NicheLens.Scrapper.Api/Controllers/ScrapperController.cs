using System.Reflection;
using System.Web.Http;

namespace NicheLens.Scrapper.Api.Controllers
{
	public class ScrapperController : ApiController
	{
		[Route("api/scrapper/start")]
		public IHttpActionResult Get()
		{
			return Ok(MethodBase.GetCurrentMethod().GetCustomAttribute<RouteAttribute>().Template);
		}
	}
}