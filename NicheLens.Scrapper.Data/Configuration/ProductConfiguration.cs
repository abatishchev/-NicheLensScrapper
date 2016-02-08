using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using NicheLens.Scrapper.Data.Models;

namespace NicheLens.Scrapper.Data.Configuration
{
	public class ProductConfiguration : EntityTypeConfiguration<Product>
	{
		public ProductConfiguration()
		{
			ToTable("Product").HasKey(x => x.ProductId);

			Property(x => x.ProductId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
		}
	}
}