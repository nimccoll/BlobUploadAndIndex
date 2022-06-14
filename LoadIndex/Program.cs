using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LoadIndex
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string environmentName = Environment.GetEnvironmentVariable("ENVIRONMENT");
            string currentDirectory = Directory.GetCurrentDirectory();

            // Setup console application to read settings from appsettings.json
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true);

            IConfigurationRoot configuration = configurationBuilder.Build();

            // Configure default dependency injection container
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddLogging(configure => configure.AddConsole())
                .AddSingleton<IConfigurationRoot>(configuration)
                .BuildServiceProvider();

            ILogger logger = serviceProvider.GetService<ILogger<Program>>();

            // Load JSON file of documents to upload
            logger.LogInformation("Reading JSON file of documents to load...");
            Documents documents = null;
            try
            {
                string jsonDocuments = await File.ReadAllTextAsync(Path.Combine(currentDirectory, "json_test_index.json"));
                documents = JsonConvert.DeserializeObject<Documents>(jsonDocuments);
                logger.LogInformation("Successfully read JSON file of documents to load.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to read JSON file of documents. Exception message is {ex.Message}");
            }

            if (documents != null)
            {
                // Update the Azure Search Index
                logger.LogInformation("Loading documents into search index...");
                try
                {
                    SearchClient searchClient = new SearchClient(new Uri(configuration["AZURE_SEARCH_URI"]), configuration["AZURE_SEARCH_INDEX"], new AzureKeyCredential(configuration["AZURE_SEARCH_KEY"]));
                    IndexDocumentsResult indexResults = await searchClient.MergeOrUploadDocumentsAsync<Document>(documents.value);
                    logger.LogInformation("Documents successfully loaded into search index. Run complete.");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Failed to load documents into search index. Exception message is {ex.Message}");
                }
            }
        }
    }
}
