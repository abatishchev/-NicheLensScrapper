﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Ab;
using Ab.Amazon;
using Ab.Amazon.Data;
using Ab.Filtering;
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
		private readonly IScrapperApi _scrapperApi;
		private readonly IAzureCategoryProvider _categoryProvider;
		private readonly CsvCategoryParser _categoryParser;
		private readonly IConverter<CsvCategory, Category> _categoryConverter;
		private readonly IFilter<Category> _categoryFilter;

		public Functions(IScrapperApi scrapperApi,
						 IAzureCategoryProvider categoryProvider,
						 CsvCategoryParser categoryParser,
						 IConverter<CsvCategory, Category> categoryConverter,
						 IFilter<Category> categoryFilter)
		{
			_scrapperApi = scrapperApi;
			_categoryProvider = categoryProvider;
			_categoryParser = categoryParser;
			_categoryConverter = categoryConverter;
			_categoryFilter = categoryFilter;
		}

		[NoAutomaticTrigger]
		public Task StartScrapper(CancellationToken token)
		{
			var scrapperClient = new ScrapperApi();
			return scrapperClient.Scrapper.GetWithOperationResponseAsync(token);
		}

		public async Task ParseCategoriesFromCsv([BlobTrigger("categories-csv")] ICloudBlob blob,
												 TextWriter log,
												 CancellationToken token)
		{
			var blobName = HttpUtility.HtmlDecode(blob.Name);

			log.WriteLine("Starting parsing {0}", blobName);

			try
			{
				var stream = await blob.OpenReadAsync(token);
				var textReader = new StreamReader(stream);

				var categories = _categoryParser.Parse(textReader)
												.Select(_categoryConverter.Convert)
												.Where(_categoryFilter.Filter)
												.ToArray();
				await _categoryProvider.SaveCategories(categories);
				log.WriteLine("Parsed and saved {0} categories", categories.Length);

				await blob.DeleteAsync(token);
				log.WriteLine("Blob {0} deleted", blobName);

			}
			catch (Exception ex)
			{
				log.WriteLine("Error parsing {0}: {1}", blobName, ex.Message);
				ErrorLog.GetDefault(null).Log(new Error(ex));
				throw;
			}

			log.WriteLine("Finished parsing {0}", blobName);
		}

		/*
		public Task ProcessCategoryQueue([QueueTrigger("categories")] Category category,
		                                 CancellationToken token)
		{
			Console.WriteLine(category.Name);

			return Task.CompletedTask;
		}
		*/
	}
}