using Ab.Amazon.Data;
using AutoMapper;

namespace NicheLens.Scrapper.Data.Models
{
	public sealed class CategoryMappingProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<Category, CategoryEntity>()
				.ForMember(dest => dest.Products, opt => opt.Ignore())
			.ReverseMap();
		}
	}
}