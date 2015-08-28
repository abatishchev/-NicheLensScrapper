using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ab;
using Ab.Amazon;
using Ab.Amazon.Data;

using CsvHelper;
using Elmah;

using Microsoft.Azure.WebJobs;

using NicheLens.Scrapper.Api.Client;
using NicheLens.Scrapper.WebJobs.Data;

namespace NicheLens.Scrapper.WebJobs
{
	public class Functions
	{
		private readonly IAzureCategoryProvider _categoryProvider;
		private readonly CsvCategoryParser _categoryParser;
		private readonly IConverter<CsvCategory, Category> _categoryConverter;

		public Functions(IAzureCategoryProvider categoryProvider, CsvCategoryParser categoryParser, IConverter<CsvCategory, Category> categoryConverter)
		{
			_categoryProvider = categoryProvider;
			_categoryParser = categoryParser;
			_categoryConverter = categoryConverter;
		}

		[NoAutomaticTrigger]
		public Task StartScrapperAsync(CancellationToken token)
		{
			var scrapperClient = new ScrapperApi();
			return scrapperClient.Scrapper.GetWithOperationResponseAsync(token);
		}

		public void ParseCategoriesFromCsv([BlobTrigger("categories-csv")] TextReader textReader,
										   CancellationToken token)
		{
			try
			{
				var categories = _categoryParser.Parse(textReader)
												.Select(_categoryConverter.Convert)
												.ToArray();
				_categoryProvider.SaveCategories(categories);
			}
			catch (CsvHelperException ex)
			{
				ErrorLog.GetDefault(null).Log(new Error(ex));
			}
		}

		public Task ProcessCategoryQueueAsync([QueueTrigger("categories")] Category category,
											  CancellationToken token)
		{
			Console.WriteLine(category.Name);

			return Task.Delay(0, token);
		}
	}
}