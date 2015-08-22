﻿using System.IO;

using Ab.Factory;

using CsvHelper;
using CsvHelper.Configuration;

namespace NicheLens.Scrapper.WebJobs.Data
{
	public sealed class CsvReaderFactory : IFactory<ICsvReader, TextReader>
	{
		private readonly CsvFactory _factory;

		public CsvReaderFactory(CsvFactory factory)
		{
			_factory = factory;
		}

		public ICsvReader Create(TextReader textReader)
		{
			var configuration = new CsvConfiguration
			{
				Delimiter = "|"
			};
			var csvReader = _factory.CreateReader(textReader, configuration);
			csvReader.Configuration.RegisterClassMap<CsvCategoryMap>();
			return csvReader;
		}
	}
}