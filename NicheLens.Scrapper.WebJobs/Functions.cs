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
using Microsoft.WindowsAzure.Storage.Blob;
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

		public async Task ParseCategoriesFromCsv([BlobTrigger("categories-csv")] ICloudBlob blob,
												 CancellationToken token)
		{
			try
			{
				var stream = await blob.OpenReadAsync(token);
				var textReader = new StreamReader(stream);

				var categories = _categoryParser.Parse(textReader)
												.Select(_categoryConverter.Convert)
												.ToArray();
				await _categoryProvider.SaveCategories(categories);

				await blob.DeleteAsync(token);
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

			return Task.CompletedTask;
		}
	}
}