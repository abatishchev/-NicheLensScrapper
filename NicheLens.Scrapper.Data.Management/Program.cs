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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NicheLens.Scrapper.Data.Management
{
	class Program
	{
		//*************************************************************************************************************
		// The Microsoft Azure Management Libraries are intended for developers who want to automate 
		// the management, provisioning, deprovisioning and test of cloud infrastructure with ease.            
		// These services support Microsoft Azure Virtual Machines, Cloud Services, Storage, Virtual Networks, 
		// Web Sites and core data center infrastructure management. For more information on the Management
		// Libraries for .NET, see https://msdn.microsoft.com/en-us/library/azure/dn722415.aspx. 
		//
		// If you don't have a Microsoft Azure subscription you can get a FREE trial account here:
		// http://go.microsoft.com/fwlink/?LinkId=330212
		//
		// This Quickstart demonstrates using WAML how to provision a new Microsoft Azure SQL DB Server,
		// Configure the Firewall Rules, Create a new Database, Then drop the Database and Delete the Server.
		//
		// TODO: Perform the following steps before running the sample:
		//
		// 1. Download your Microsoft Azure PublishSettings file; to do so click here:
		//    http://go.microsoft.com/fwlink/?LinkID=276844 
		//
		// 2. Fill in the full path of the PublishSettings file below in PublishSettingsFilePath.
		//
		// 3. Choose an [ADMIN USER] and [ADMIN PASSWORD] that you wish to use for the server.
		//
		// 4. Update the FirewallRuleStartIP and FirewallRuleEndIP values.
		//    If you wish to add a firewall rule to allow your local development computer access
		//    to the Server / Database then configure FirewallRuleStartIP and FirewallRuleEndIP to the public IP of 
		//    your local development machine.
		//
		// 6. Adjust values of any other parameter as you wish
		//*************************************************************************************************************

		private static string _serverName;

		static void Main()
		{
			var parameters = new SqlManagementControllerParameters
			{
				PublishSettingsFilePath = @"Properties\OJ.publishsettings",
				ServerRegion = "West US",
				ServerAdminUsername = "sqladmin",
				ServerAdminPassword = "1=SqlAzure!",
				FirewallRuleAllowAzureServices = true,
				FirewallRuleName = "Sample Firewall Rule",
				FirewallRuleStartIP = "0.0.0.0",
				FirewallRuleEndIP = "255.255.255.254", // Example Firewall Rule only. Do Not Use in Production.
				DatabaseName = "Demo",
				DatabaseEdition = "Basic",
				DatabaseMaxSizeInGB = 1,
				DatabaseCollation = "SQL_Latin1_General_CP1_CI_AS"
			};

			if (!VerifyConfiguration(parameters))
			{
				Console.ReadLine();
				return;
			}

			Task.WaitAll(SetupAndTearDownLogicalSqlServer(parameters));

			Console.WriteLine("Done");
			Console.ReadLine();
		}

		private static bool VerifyConfiguration(SqlManagementControllerParameters serviceParameters)
		{
			bool ok = true;
			if (!File.Exists(serviceParameters.PublishSettingsFilePath))
			{
				ok = false;
				Console.WriteLine("Please download your .publishsettings file and specify the location in the Main method.");
			}
			if (serviceParameters.ServerAdminUsername.StartsWith("[") || serviceParameters.ServerAdminPassword.StartsWith("["))
			{
				ok = false;
				Console.WriteLine("Please specify a server admin username and password in the Main method.");
			}
			return ok;
		}

		private static async Task SetupAndTearDownLogicalSqlServer(SqlManagementControllerParameters parameters)
		{
			int step = 1;

			// Create a new logical server     
			await SetupServerAsync(parameters, step++);

			// List servers in subscription
			await ListServersAsync(parameters, step++);

			// Add new Firewall rules on the logical server created
			await ConfigureFirewallAsync(parameters, step++);

			// List Firewall Rules on server
			await ListFirewallRulesAsync(parameters, step++);

			// Create a new database on the server
			await CreateDatabaseAsync(parameters, step++);

			// List Firewall Rules on server
			await ListDatabasesAsync(parameters, step++);

			// Cleanup
			await TearDownDatabaseAsync(parameters, step++);
			await TearDownServerAsync(parameters, step++);
		}

		private static async Task ListDatabasesAsync(SqlManagementControllerParameters parameters, int step)
		{
			using (var controller = new SqlManagementController(parameters.PublishSettingsFilePath))
			{
				Console.WriteLine("\n{1}. Listing Databases on Server {0}", _serverName, step);
				ConsoleContinuePrompt("List");

				var t = await controller.ListDatabasesAsync(_serverName);

				var databases = t.Databases.Select(s => s.Name);

				Console.Write("\n");

				foreach (string database in databases)
				{
					Console.WriteLine("   Database - {0}", database);
				}

				Console.WriteLine("...Complete");
			}
		}

		private static async Task ListFirewallRulesAsync(SqlManagementControllerParameters parameters, int step)
		{
			using (var controller = new SqlManagementController(parameters.PublishSettingsFilePath))
			{
				Console.WriteLine("\n{1}. Listing Firewall Rules for Server {0}", _serverName, step);
				ConsoleContinuePrompt("List");

				var t = await controller.ListFirewallRulesAsync(_serverName);

				var rules = from r in t.FirewallRules
							select new
							{
								Name = r.Name,
								StartIP = r.StartIPAddress,
								EndIP = r.EndIPAddress
							};

				Console.Write("\n");

				foreach (var rule in rules)
				{
					Console.WriteLine("   Rule - {0}\tStart IP - {1}\tEnd IP - {2}", rule.Name, rule.StartIP, rule.EndIP);
				}

				Console.WriteLine("...Complete");
			}
		}

		private static async Task ListServersAsync(SqlManagementControllerParameters parameters, int step)
		{
			using (var controller = new SqlManagementController(parameters.PublishSettingsFilePath))
			{
				Console.WriteLine("\n{1}. Listing servers on server {0}", _serverName, step);
				ConsoleContinuePrompt("List");

				var t = await controller.ListServersAsync();
				var servers = t.Servers.Select(s => s.Name);

				Console.Write("\n");

				foreach (string server in servers)
				{
					Console.WriteLine("   Server - {0}", server);
				}

				Console.WriteLine("...Complete");
			}
		}

		private static async Task TearDownServerAsync(SqlManagementControllerParameters parameters, int step)
		{
			using (var controller = new SqlManagementController(parameters.PublishSettingsFilePath))
			{
				Console.WriteLine("\n{1}. Dropping Server {0}", _serverName, step);
				ConsoleContinuePrompt("Drop");

				await controller.DeleteServerAsync(_serverName);

				Console.WriteLine("\n...Complete");
			}
		}

		private static async Task TearDownDatabaseAsync(SqlManagementControllerParameters parameters, int step)
		{
			using (var controller = new SqlManagementController(parameters.PublishSettingsFilePath))
			{
				Console.WriteLine("\n{2}. Dropping Database {1} on Server {0}", _serverName, parameters.DatabaseName, step);
				ConsoleContinuePrompt("Drop");

				await controller.DropDatabaseAsync(_serverName, parameters.DatabaseName);

				Console.WriteLine("\n...Complete");
			}
		}

		private static async Task CreateDatabaseAsync(SqlManagementControllerParameters parameters, int step)
		{
			using (var controller = new SqlManagementController(parameters.PublishSettingsFilePath))
			{
				Console.WriteLine("\n{2}. Creating Database {1} on Server {0}", _serverName, parameters.DatabaseName, step);
				ConsoleContinuePrompt("Create");

				await controller.CreateDatabaseAsync(_serverName, parameters.DatabaseName, parameters.DatabaseCollation, parameters.DatabaseEdition, parameters.DatabaseMaxSizeInGB);

				Console.WriteLine("\n...Complete");
			}
		}

		private static async Task ConfigureFirewallAsync(SqlManagementControllerParameters parameters, int step)
		{
			using (var controller = new SqlManagementController(parameters.PublishSettingsFilePath))
			{
				Console.WriteLine("\n{1}. Adding Firewall rules for server {0}", _serverName, step);
				ConsoleContinuePrompt("Create");

				await controller.ConfigureFirewallAsync(_serverName, parameters.FirewallRuleName, parameters.FirewallRuleStartIP, parameters.FirewallRuleEndIP);

				Console.WriteLine("\n...Complete");
			}
		}

		private static async Task SetupServerAsync(SqlManagementControllerParameters parameters, int step)
		{
			using (var controller = new SqlManagementController(parameters.PublishSettingsFilePath))
			{
				Console.WriteLine("\n{1}. Create logical Server in Region {0}", parameters.ServerRegion, step);
				ConsoleContinuePrompt("Create");

				var t = await controller.CreateServerAsync(parameters.ServerRegion, parameters.ServerAdminUsername, parameters.ServerAdminPassword);

				// now that the task is done, save the returned Server Name in to a global variable
				// so that we can use it again later when creating the Firewall Rules and Database etc. 
				_serverName = t.ServerName;

				if (parameters.FirewallRuleAllowAzureServices)
				{
					Console.WriteLine("\n{1}. Adding Firewall rules for Azure Services on server {0}", _serverName, step);

					await controller.ConfigureFirewallAsync(_serverName, "Azure Services", parameters.FirewallRuleStartIP, parameters.FirewallRuleEndIP);
				}

				Console.WriteLine("\n...Complete");
			}
		}

		private static void ConsoleContinuePrompt(string prompt)
		{
			Console.WriteLine("\t > Press Enter to {0}", prompt);
			Console.ReadKey();
			Console.WriteLine("\t\t Starting, view progress in the management portal....");
		}
	}
}