using System.Data.Entity;
using System.Reflection;

using Ab.Amazon.Data;

namespace NicheLens.Scrapper.Data
{
	public class ModelContext : DbContext, IModelContext
	{
		static ModelContext()
		{
			Database.SetInitializer<ModelContext>(null);
		}

		public ModelContext(Ab.Configuration.IConfigurationProvider configurationProvider)
			: base(configurationProvider.GetValue("azure:Sql:ConnectionString"))
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());
		}

		public DbSet<CategoryEntity> Categories { get; set; }

		public DbSet<ProductEntity> Products { get; set; }
	}
}