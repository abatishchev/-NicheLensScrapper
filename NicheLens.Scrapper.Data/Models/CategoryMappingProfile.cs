using AutoMapper;

namespace NicheLens.Scrapper.Data.Models
{
	public sealed class CategoryMappingProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<Ab.Amazon.Data.Category, Category>()
				.ReverseMap();
		}
	}
}