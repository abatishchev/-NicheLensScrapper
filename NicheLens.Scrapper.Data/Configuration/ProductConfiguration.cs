using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using Ab.Amazon.Data;

namespace NicheLens.Scrapper.Data.Configuration
{
	public class ProductConfiguration : EntityTypeConfiguration<ProductEntity>
	{
		public ProductConfiguration()
		{
			ToTable("Product").HasKey(x => x.ProductId);

			Property(x => x.ProductId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
		}
	}
}