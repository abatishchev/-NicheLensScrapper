using Ab.Amazon.Data;
using AutoMapper;

namespace NicheLens.Scrapper.WebJobs.Data
{
	public sealed class CsvCategoryMappingProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<CsvCategory, Category>();
		}

		public override string ProfileName
		{
			get { return typeof(CsvCategoryMappingProfile).Name; }
		}
	}
}