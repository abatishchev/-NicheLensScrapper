using System;
using System.IO;
using System.Linq;
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
		private readonly IAwsCategoryProvider _provider;
		private readonly IConverter<Product, ProductEntity> _converter;
		private readonly IProductRepository _repository;

		public ProcessCategoryQueueFunction(ILogger logger,
											IAwsCategoryProvider provider,
											IConverter<Product, ProductEntity> converter,
											IProductRepository repository)
		{
			_logger = logger;
			_provider = provider;
			_converter = converter;
			_repository = repository;
		}

		public async Task ProcessCategoryQueue([QueueTrigger("%azure:Queue:Categories%")] Category category,
											   TextWriter log,
											   CancellationToken cancellationToken)
		{
			log.WriteLine("Starting scrapping {0} (id={1})", category.Name, category.NodeId);

			try
			{
				var products = await _provider.GetProductsInCategory(category);

				log.WriteLine("Found {0} products", products.Length);

				var entities = products.Select(_converter.Convert).ToArray();
				await _repository.UpdateProducts(entities);
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