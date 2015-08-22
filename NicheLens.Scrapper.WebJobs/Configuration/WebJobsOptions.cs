using System;

namespace NicheLens.Scrapper.WebJobs.Configuration
{
	public class WebJobsOptions
	{
		public int BatchSize { get; set; }

		public string ConnectionString { get; set; }

		public int MaxDequeueCount { get; set; }

		public TimeSpan MaxPollingInterval { get; set; }
	}
}