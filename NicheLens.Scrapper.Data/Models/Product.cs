using System;

namespace NicheLens.Scrapper.Data.Models
{
	public class Product
	{
		public Guid ProductId { get; set; }

		public string Asin { get; set; }

		public string SearchIndex { get; set; }

		public long BrowseNode { get; set; }

		public string ProductGroup { get; set; }

		public string Title { get; set; }

		public string Brand { get; set; }

		public string LargeImageUrl { get; set; }

		public int? LowestNewPrice { get; set; }

		public string DetailsPageUrl { get; set; }

		public string CustomerReviewsUrl { get; set; }
	}
}