using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ab.Amazon.Data;
using Ab.Factory;

using CsvHelper;

using Microsoft.Azure.WebJobs;
using NicheLens.Scrapper.Api.Client;
using NicheLens.Scrapper.WebJobs.Data;

namespace NicheLens.Scrapper.WebJobs
{
	public class Functions
	{
		private readonly IFactory<ICsvReader, TextReader> _readerFactory;

		public Functions(IFactory<ICsvReader, TextReader> readerFactory)
		{
			_readerFactory = readerFactory;
		}

		[NoAutomaticTrigger]
		public static Task StartScrapperAsync(CancellationToken token)
		{
			var scrapperClient = new ScrapperApi();
			return scrapperClient.Scrapper.GetWithOperationResponseAsync(token);
		}

		public void ProcessCategoryCsv([BlobTrigger("categories-csv/{name}")] TextReader textReader,
									   string name,
									   CancellationToken token)
		{
			name = Uri.UnescapeDataString(name);

			var csvReader = _readerFactory.Create(textReader);
			try
			{
				var records = csvReader.GetRecords<CsvCategory>().ToArray();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Data["CsvHelper"]);
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