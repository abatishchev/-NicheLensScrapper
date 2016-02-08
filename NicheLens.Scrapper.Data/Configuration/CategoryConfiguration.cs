using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using NicheLens.Scrapper.Data.Models;

namespace NicheLens.Scrapper.Data.Configuration
{
	public class CategoryConfiguration : EntityTypeConfiguration<Category>
	{
		public CategoryConfiguration()
		{
			ToTable("Category").HasKey(x => x.CategoryId);

			Property(x => x.CategoryId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
		}
	}
}