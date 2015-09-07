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
		private readonly IFilter<Category> _categoryFilter;

		public Functions(IAzureCategoryProvider categoryProvider,
						 CsvCategoryParser categoryParser,
						 IConverter<CsvCategory, Category> categoryConverter,
						 IFilter<Category> categoryFilter)
		{
			_categoryProvider = categoryProvider;
			_categoryParser = categoryParser;
			_categoryConverter = categoryConverter;
			_categoryFilter = categoryFilter;
		}

		public async Task ParseCategoriesFromCsv([BlobTrigger("categories-csv")] ICloudBlob blob,
												 TextWriter log,
												 CancellationToken cancellationToken)
		{
			var blobName = HttpUtility.HtmlDecode(blob.Name);
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

				await _categoryProvider.SaveCategories(categories);
				log.WriteLine("Saved {0} categories", categories.Length);

				await _categoryProvider.EnqueueCategories(categories);
				log.WriteLine("Enqueued {0} categories", categories.Length);

				await blob.DeleteAsync(cancellationToken);
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

		public Task ProcessCategoryQueue([QueueTrigger("categories")] Category category,
										 TextWriter log,
										 CancellationToken cancellationToken)
		{
			log.WriteLine("Starting scrapping {0} (id={1})", category.Name, category.NodeId);

			try
			{
				//throw new Exception();
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				log.WriteLine("Error scrapping category {0} (id={1}): {2}", category.Name, category.NodeId, ex.Message);
				ErrorLog.GetDefault(null).Log(new Error(ex));
				throw;
			}
		}
	}
}