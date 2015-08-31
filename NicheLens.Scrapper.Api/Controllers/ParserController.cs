using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;

namespace NicheLens.Scrapper.Api.Controllers
{
	public class ParserController : ApiController
	{
		[Route("api/parser/complete")]
		public IHttpActionResult Post(ICollection<string> indecies)
		{
			return Ok(MethodBase.GetCurrentMethod().GetCustomAttribute<RouteAttribute>().Template);
		}
	}
}