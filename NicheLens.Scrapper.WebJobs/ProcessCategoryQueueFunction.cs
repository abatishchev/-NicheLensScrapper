using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ab;
using Ab.Amazon;
using Ab.Amazon.Data;

using Microsoft.Azure.WebJobs;
using NicheLens.Scrapper.Data;

using AmazonProduct = Ab.Amazon.Data.Product;
using ModelProduct = NicheLens.Scrapper.Data.Models.Product;

namespace NicheLens.Scrapper.WebJobs
{
	public class ProcessCategoryQueueFunction
	{
		private readonly ILogger _logger;
		private readonly IAwsCategoryProvider _awsCategoryProvider;
		private readonly IConverter<AmazonProduct, ModelProduct> _productConverter;
		private readonly IProductRepository _productRepository;

		public ProcessCategoryQueueFunction(ILogger logger,
											IAwsCategoryProvider awsCategoryProvider,
											IConverter<AmazonProduct, ModelProduct> productConverter,
											IProductRepository productRepository)
		{
			_logger = logger;
			_awsCategoryProvider = awsCategoryProvider;
			_productConverter = productConverter;
			_productRepository = productRepository;
		}

		public async Task ProcessCategoryQueue([QueueTrigger("%azure:Queue:Categories%")] Category category,
											   TextWriter log,
											   CancellationToken cancellationToken)
		{
			log.WriteLine("Starting scrapping {0} (id={1})", category.Name, category.NodeId);

			try
			{
				var awsProducts = await _awsCategoryProvider.GetProductsInCategory(category);

				log.WriteLine("Found {0} products", awsProducts.Length);

				var products = awsProducts.Select(_productConverter.Convert).ToArray();

				await _productRepository.MergeProducts(products);
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