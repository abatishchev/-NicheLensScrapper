using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

using Ab.Amazon;

namespace NicheLens.Scrapper.Api.Controllers
{
	public class ParserController : ApiController
	{
		private readonly IAzureCategoryProvider _categoryProvider;

		public ParserController(IAzureCategoryProvider categoryProvider)
		{
			_categoryProvider = categoryProvider;
		}

		[Route("api/parser/complete")]
		public async Task<IHttpActionResult> Post([FromBody]ICollection<string> indices)
		{
			var categories = _categoryProvider.GetCategories(0, indices);

			await _categoryProvider.EnqueueCategories(categories);

			return Ok(new { Count = categories.Count });
		}
	}
}