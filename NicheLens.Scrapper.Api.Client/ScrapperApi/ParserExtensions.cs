﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.9.7.0
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using NicheLens.Scrapper.Api.Client;

namespace NicheLens.Scrapper.Api.Client
{
    public static partial class ParserExtensions
    {
        /// <param name='operations'>
        /// Reference to the NicheLens.Scrapper.Api.Client.IParser.
        /// </param>
        /// <param name='indecies'>
        /// Required.
        /// </param>
        public static string Post(this IParser operations, IList<string> indecies)
        {
            return Task.Factory.StartNew((object s) => 
            {
                return ((IParser)s).PostAsync(indecies);
            }
            , operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
        
        /// <param name='operations'>
        /// Reference to the NicheLens.Scrapper.Api.Client.IParser.
        /// </param>
        /// <param name='indecies'>
        /// Required.
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        public static async Task<string> PostAsync(this IParser operations, IList<string> indecies, CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            Microsoft.Rest.HttpOperationResponse<string> result = await operations.PostWithOperationResponseAsync(indecies, cancellationToken).ConfigureAwait(false);
            return result.Body;
        }
    }
}
