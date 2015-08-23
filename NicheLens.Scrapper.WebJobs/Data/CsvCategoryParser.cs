using System.Collections.Generic;
using System.IO;

using Ab.Factory;
using CsvHelper;

namespace NicheLens.Scrapper.WebJobs.Data
{
	public sealed class CsvCategoryParser
	{
		private readonly IFactory<ICsvReader, TextReader> _readerFactory;

		public CsvCategoryParser(IFactory<ICsvReader, TextReader> readerFactory)
		{
			_readerFactory = readerFactory;
		}

		public IEnumerable<CsvCategory> Parse(TextReader reader)
		{
			var csvReader = _readerFactory.Create(reader);
			return csvReader.GetRecords<CsvCategory>();
		}
	}
}