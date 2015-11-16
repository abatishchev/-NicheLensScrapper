using Ab;
using AutoMapper;

using AmazonProduct = Ab.Amazon.Data.Product;

namespace NicheLens.Scrapper.Data.Models
{
	public class MappingProductConverter : IConverter<AmazonProduct, Product>
	{
		private readonly IMappingEngine _mapper;

		public MappingProductConverter(IMappingEngine mapper)
		{
			_mapper = mapper;
		}

		public Product Convert(AmazonProduct product)
		{
			return _mapper.Map<Product>(product);
		}

		public AmazonProduct ConvertBack(Product product)
		{
			return _mapper.Map<AmazonProduct>(product);
		}
	}
}