//----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//----------------------------------------------------------------------------------

using AzureQuickStarts.Common;
using Microsoft.Azure;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace NicheLens.Scrapper.Data.Management
{
	internal class CredentialsHelper
	{
		internal static SubscriptionCloudCredentials GetSubscriptionCloudCredentials(string publishSettingsFilePath)
		{
			PublishSettingsSubscriptionItem publishSettingCreds = null;

			using (var fs = File.OpenRead(publishSettingsFilePath))
			{
				var document = XDocument.Load(fs);
				var subscriptions = document.Descendants("Subscription").Select(e => e).ToArray();

				if (subscriptions.Any())
				{
					// use first subscription in the publish settings file
					var subscription = subscriptions.First();

					publishSettingCreds = new PublishSettingsSubscriptionItem
					{
						SubscriptionName = subscription.Attribute("Name").Value,
						SubscriptionId = subscription.Attribute("Id").Value,
						ManagementCertificate = subscription.Attribute("ManagementCertificate").Value
					};
				}
				else
				{
					Console.WriteLine("Invalid publishsettings file: Subscription not found.");
				}
			}

			return new CertificateCloudCredentials(
				publishSettingCreds.SubscriptionId,
				new X509Certificate2(Convert.FromBase64String(publishSettingCreds.ManagementCertificate)));
		}

	}
}
