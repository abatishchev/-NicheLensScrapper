using Ab.Amazon.Data;
using AutoMapper;

namespace NicheLens.Scrapper.WebJobs.Data
{
	public sealed class CsvCategoryMappingProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<CsvCategory, Category>()
				.ForMember(d => d.Name, o => o.MapFrom(x => x.Title))
				.ForMember(d => d.CategoryId, opt => opt.Ignore());
		}
	}
}