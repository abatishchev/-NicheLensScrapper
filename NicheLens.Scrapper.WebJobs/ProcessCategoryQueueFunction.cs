using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Ab;
using Ab.Amazon;
using Ab.Amazon.Data;

using Microsoft.Azure.WebJobs;

namespace NicheLens.Scrapper.WebJobs
{
	public class ProcessCategoryQueueFunction
	{
		private readonly ILogger _logger;
		private readonly IAwsCategoryProvider _awsCategoryProvider;
		private readonly IAzureProductProvider _azureProductProvider;

		public ProcessCategoryQueueFunction(ILogger logger,
											IAwsCategoryProvider awsCategoryProvider,
											IAzureProductProvider azureProductProvider)
		{
			_logger = logger;
			_awsCategoryProvider = awsCategoryProvider;
			_azureProductProvider = azureProductProvider;
		}

		public async Task ProcessCategoryQueue([QueueTrigger("%azure:Queue:Categories%")] Category category,
											   TextWriter log,
											   CancellationToken cancellationToken)
		{
			log.WriteLine("Starting scrapping {0} (id={1})", category.Name, category.NodeId);

			try
			{
				var products = await _awsCategoryProvider.GetProductsInCategory(category);

				log.WriteLine("Found {0} products", products.Length);

				await _azureProductProvider.SaveProducts(products);
			}
			catch (Exception ex)
			{
				log.WriteLine("Error scrapping category {0} (id={1}): {2}", category.Name, category.NodeId, ex.Message);
				_logger.LogException(ex);
				throw;
			}

			log.WriteLine("Finished scrapping {0} (id={1})", category.Name, category.NodeId);
		}
	}
}