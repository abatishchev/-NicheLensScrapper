using System;

using Ab;

using Elmah;
using Microsoft.ApplicationInsights;

namespace NicheLens.Scrapper.WebJobs
{
	public sealed class Logger : ILogger
	{
		private readonly ErrorLog _elmah;
		private readonly TelemetryClient _ai;

		public Logger(ErrorLog elmah, TelemetryClient ai)
		{
			_elmah = elmah;
			_ai = ai;
		}

		public void LogException(Exception exception)
		{
			_elmah.Log(new Error(exception));
			_ai.TrackException(exception);
		}
	}
}