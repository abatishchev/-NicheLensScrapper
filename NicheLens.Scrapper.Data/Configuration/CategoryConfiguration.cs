using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using Ab.Amazon.Data;

namespace NicheLens.Scrapper.Data.Configuration
{
	public class CategoryConfiguration : EntityTypeConfiguration<CategoryEntity>
	{
		public CategoryConfiguration()
		{
			ToTable("Category").HasKey(x => x.CategoryId);

			Property(x => x.CategoryId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
		}
	}
}