using AutoMapper;

namespace NicheLens.Scrapper.Data.Models
{
	public sealed class ProductMappingProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<Ab.Amazon.Data.Product, Product>(); //.ForMember(d => d.CustomerReviewsUrl, o => o.MapFrom(x => x.CustomerReviewsUrl.ToString()));
		}
	}
}