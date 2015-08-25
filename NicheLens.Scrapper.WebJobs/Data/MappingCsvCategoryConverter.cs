using System;

using Ab;
using Ab.Amazon.Data;

using AutoMapper;

namespace NicheLens.Scrapper.WebJobs.Data
{
	public sealed class MappingCsvCategoryConverter : IConverter<CsvCategory, Category>
	{
		private readonly IMappingEngine _mapper;

		public MappingCsvCategoryConverter(IMappingEngine mapper)
		{
			_mapper = mapper;
		}

		public Category Convert(CsvCategory category)
		{
			return _mapper.Map<Category>(category);
		}

		public CsvCategory ConvertBack(Category category)
		{
			throw new NotImplementedException();
		}
	}
}