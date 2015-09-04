using System;
using System.Net;
using System.Net.Http;

using Ab.Configuration;
using Ab.Factory;

namespace NicheLens.Scrapper.Api.Client
{
	public sealed class ScrapperApiFactory : IFactory<IScrapperApi>
	{
		private readonly IEnvironmentProvider _environmentProvider;

		public ScrapperApiFactory(IEnvironmentProvider environmentProvider)
		{
			_environmentProvider = environmentProvider;
		}

		public IScrapperApi Create()
		{
			return new ScrapperApi(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })
			{
				BaseUri = new Uri(GetUrl())
			};
		}

		// TODO: replace with "environment:ApiUrl"?
		private string GetUrl()
		{
			switch (_environmentProvider.GetCurrentEnvironment())
			{
				case EnvironmentName.Production:
					return "https://microsoft-apiapp9b2b9b0667ef43058cfe467f6b8d6828.azurewebsites.net";
				case EnvironmentName.Local:
				case EnvironmentName.Testing:
					return "http://localhost:8088";
				default:
					throw new NotImplementedException();
			}
		}
	}
}