using Ab.Amazon.Data;
using AutoMapper;

namespace NicheLens.Scrapper.Data.Models
{
	public sealed class ProductMappingProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<Product, ProductEntity>()
				.ForMember(d => d.CustomerReviewsUrl, o => o.MapFrom(x => x.CustomerReviewsUrl.ToString()))
				.ForMember(d => d.DetailsPageUrl, o => o.MapFrom(x => x.DetailsPageUrl.ToString()))
				.ForMember(dest => dest.Category, opt => opt.Ignore())
				.ForMember(dest => dest.CategoryId, opt => opt.Ignore())
			.ReverseMap();
		}
	}
}