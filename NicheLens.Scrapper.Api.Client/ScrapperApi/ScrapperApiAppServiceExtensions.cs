using System;
using System.Net.Http;
using Microsoft.Azure.AppService;

namespace NicheLens.Scrapper.Api.Client
{
    public static class ScrapperApiAppServiceExtensions
    {
        public static ScrapperApi CreateScrapperApi(this IAppServiceClient client)
        {
            return new ScrapperApi(client.CreateHandler());
        }

        public static ScrapperApi CreateScrapperApi(this IAppServiceClient client, params DelegatingHandler[] handlers)
        {
            return new ScrapperApi(client.CreateHandler(handlers));
        }

        public static ScrapperApi CreateScrapperApi(this IAppServiceClient client, Uri uri, params DelegatingHandler[] handlers)
        {
            return new ScrapperApi(uri, client.CreateHandler(handlers));
        }

        public static ScrapperApi CreateScrapperApi(this IAppServiceClient client, HttpClientHandler rootHandler, params DelegatingHandler[] handlers)
        {
            return new ScrapperApi(rootHandler, client.CreateHandler(handlers));
        }
    }
}
