using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Ab;
using Ab.Amazon;
using Ab.Amazon.Data;
using Ab.Filtering;

using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Blob;

using NicheLens.Scrapper.WebJobs.Data;

namespace NicheLens.Scrapper.WebJobs
{
	public class ParseCategoriesFromCsvFunction
	{
		private readonly ILogger _logger;
		private readonly IAzureCategoryProvider _azureCategoryProvider;
		private readonly CsvCategoryParser _categoryParser;
		private readonly IConverter<CsvCategory, Category> _categoryConverter;
		private readonly IFilter<Category> _categoryFilter;

		public ParseCategoriesFromCsvFunction(ILogger logger,
											  IAzureCategoryProvider azureCategoryProvider,
											  CsvCategoryParser categoryParser,
											  IConverter<CsvCategory, Category> categoryConverter,
											  IFilter<Category> categoryFilter)
		{
			_logger = logger;
			_azureCategoryProvider = azureCategoryProvider;
			_categoryParser = categoryParser;
			_categoryConverter = categoryConverter;
			_categoryFilter = categoryFilter;
		}

		public async Task ParseCategoriesFromCsv([BlobTrigger("%azure:Blob:CategoriesCsv%")] ICloudBlob blob,
												 TextWriter log,
												 CancellationToken cancellationToken)
		{
			var blobName = HttpUtility.UrlDecode(blob.Name);
			log.WriteLine("Starting parsing {0}", blobName);

			try
			{
				var stream = await blob.OpenReadAsync(cancellationToken);
				var textReader = new StreamReader(stream);

				var categories = _categoryParser.Parse(textReader)
												.Select(_categoryConverter.Convert)
												.Where(_categoryFilter.Filter)
												.ToArray();
				log.WriteLine("Parsed {0} categories", categories.Length);

				await _azureCategoryProvider.SaveCategories(categories);
				log.WriteLine("Saved {0} categories", categories.Length);

				Parallel.ForEach(categories, async c =>
				{
					await _azureCategoryProvider.EnqueueCategory(c);
					log.WriteLine("Enqueued category {0} (id={1})", c.Name, c.NodeId);
				});

				await blob.DeleteAsync(cancellationToken);
				log.WriteLine("Blob {0} deleted", blobName);
			}
			catch (Exception ex)
			{
				log.WriteLine("Error parsing {0}: {1}", blobName, ex.Message);
				_logger.LogException(ex);
				throw;
			}

			log.WriteLine("Finished parsing {0}", blobName);
		}
	}
}