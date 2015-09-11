using System;

using Ab;

using Elmah;
using Microsoft.ApplicationInsights;
using Mindscape.Raygun4Net;

namespace NicheLens.Scrapper.WebJobs
{
	public sealed class Logger : ILogger
	{
		private readonly ErrorLog _elmah;
		private readonly TelemetryClient _ai;
		private readonly RaygunClient _raygun;

		public Logger(ErrorLog elmah, TelemetryClient ai, RaygunClient raygun)
		{
			_elmah = elmah;
			_ai = ai;
			_raygun = raygun;
		}

		public void LogException(Exception exception)
		{
			_elmah.Log(new Error(exception));
			_ai.TrackException(exception);
			_raygun.Send(exception);
		}
	}
}