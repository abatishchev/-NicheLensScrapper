using SimpleInjector;

namespace NicheLens.Scrapper.WebJobs.Configuration
{
	public sealed class ContainerJobActivator : Microsoft.Azure.WebJobs.Host.IJobActivator
	{
		private readonly Container _container;

		public ContainerJobActivator(Container container)
		{
			_container = container;
		}

		public T CreateInstance<T>()
		{
			_container.BeginLifetimeScope();

			return (T)_container.GetInstance(typeof(T));
		}
	}
}