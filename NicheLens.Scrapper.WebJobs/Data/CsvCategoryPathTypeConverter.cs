using System;

using CsvHelper.TypeConversion;

namespace NicheLens.Scrapper.WebJobs.Data
{
	public sealed class CsvCategoryPathTypeConverter : ITypeConverter
	{
		public string ConvertToString(TypeConverterOptions options, object value)
		{
			throw new NotSupportedException();
		}

		public object ConvertFromString(TypeConverterOptions options, string text)
		{
			return text != null ? System.Web.HttpUtility.HtmlDecode(text) : null;
		}

		public bool CanConvertFrom(Type type)
		{
			return typeof(string) == type;
		}

		public bool CanConvertTo(Type type)
		{
			return false;
		}
	}
}