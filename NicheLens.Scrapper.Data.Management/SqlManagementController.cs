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


using System;
using System.Threading.Tasks;

using Microsoft.Azure;
using Microsoft.WindowsAzure.Management.Sql;
using Microsoft.WindowsAzure.Management.Sql.Models;

namespace NicheLens.Scrapper.Data.Management
{
	internal class SqlManagementController : IDisposable
	{
		private readonly SqlManagementClient _sqlManagementClient;

		public SqlManagementController(string publishSettingsFilePath)
		{
			// To authenticate against the Microsoft Azure service management API we require management certificate
			// load this from a publish settings file and use it to new up an instance of SqlManagementClient CloudClient
			var credentials = CredentialsHelper.GetSubscriptionCloudCredentials(publishSettingsFilePath);

			_sqlManagementClient = new SqlManagementClient(credentials);
		}

		internal Task<ServerCreateResponse> CreateServerAsync(string region, string adminUsername, string adminPassword)
		{
			return _sqlManagementClient.Servers.CreateAsync(
						new ServerCreateParameters
						{
							AdministratorPassword = adminPassword,
							AdministratorUserName = adminUsername,
							Location = region,
						});
		}

		internal Task<FirewallRuleCreateResponse> ConfigureFirewallAsync(string serverName, string name, string startIp, string endIp)
		{
			return _sqlManagementClient.FirewallRules.CreateAsync(serverName,
						new FirewallRuleCreateParameters
						{
							Name = name,
							StartIPAddress = startIp,
							EndIPAddress = endIp
						});
		}

		internal Task<DatabaseCreateResponse> CreateDatabaseAsync(string serverName, string databaseName, string collation, string edition, int? maxSizeInGb)
		{
			return _sqlManagementClient.Databases.CreateAsync(serverName,
							new DatabaseCreateParameters
							{
								CollationName = collation,
								Edition = edition,
								MaximumDatabaseSizeInGB = maxSizeInGb,
								Name = databaseName
							});
		}

		internal Task<AzureOperationResponse> DropDatabaseAsync(string serverName, string databaseName)
		{
			return _sqlManagementClient.Databases.DeleteAsync(serverName, databaseName);
		}

		internal Task<AzureOperationResponse> DeleteServerAsync(string serverName)
		{
			return _sqlManagementClient.Servers.DeleteAsync(serverName);
		}

		internal Task<ServerListResponse> ListServersAsync()
		{
			return _sqlManagementClient.Servers.ListAsync();
		}

		internal Task<DatabaseListResponse> ListDatabasesAsync(string serverName)
		{
			return _sqlManagementClient.Databases.ListAsync(serverName);
		}

		internal Task<FirewallRuleListResponse> ListFirewallRulesAsync(string serverName)
		{
			return _sqlManagementClient.FirewallRules.ListAsync(serverName);
		}

		internal Task<AzureOperationResponse> UpdateAdministratorPasswordAsync(string serverName, string newPassword)
		{
			return _sqlManagementClient.Servers.ChangeAdministratorPasswordAsync(serverName,
								new ServerChangeAdministratorPasswordParameters
								{
									NewPassword = newPassword
								});
		}

		internal async Task<DatabaseUpdateResponse> UpdateDatabaseAsync(string serverName, string databaseName, string newName, string edition, int? maxSizeInGb)
		{
			var p = new DatabaseUpdateParameters();
			if (!string.IsNullOrEmpty(newName))
				p.Name = newName;
			if (!string.IsNullOrEmpty(edition))
				p.Edition = newName;
			if (maxSizeInGb != null)
				p.MaximumDatabaseSizeInGB = maxSizeInGb;

			return await _sqlManagementClient.Databases.UpdateAsync(serverName, databaseName, p);
		}

		public void Dispose()
		{
			_sqlManagementClient?.Dispose();
		}
	}
}