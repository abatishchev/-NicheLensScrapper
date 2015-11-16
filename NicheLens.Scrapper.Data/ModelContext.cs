using System.Data.Entity;
using System.Reflection;

namespace NicheLens.Scrapper.Data
{
	public class ModelContext : DbContext, IModelContext
	{
		static ModelContext()
		{
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

		public DbSet<Models.Product> Products { get; set; }
	}
}