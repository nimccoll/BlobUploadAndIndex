//===============================================================================
// Microsoft FastTrack for Azure
// Upload Blob and Index with Azure Search Example
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobUploadAndIndex.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BlobUploadAndIndex.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UploadBlob()
        {
            return View();
        }

        [HttpPost]
        [ActionName("UploadBlob")]
        public async Task<ActionResult> UploadBlobPost(IFormFile uploadedFile)
        {
            Random random = new Random();
            string[] categories = { "Technical", "Certification", "Leisure" };
            string[] subCategories = { "ASP.Net", "Azure", "Science Fiction" };

            if (uploadedFile.Length > 0)
            {
                string connectionString = _configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");

                // Create a BlobServiceClient object which will be used to create a container client
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                // Create a unique name for the container
                string containerName = "fileuploads";

                // Create the container and return a container client object
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Get the file name
                string fileName = Path.GetFileName(uploadedFile.FileName);

                // Get a reference to a blob
                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                
                // Generate some metadata
                Dictionary<string, string> metadata = new Dictionary<string, string>();
                string category = categories[random.Next(0, 2)];
                string subCategory = subCategories[random.Next(0, 2)];
                metadata.Add("Category", category);
                metadata.Add("SubCategory", subCategory);
                BlobUploadOptions blobUploadOptions = new BlobUploadOptions()
                {
                    Metadata = metadata
                };

                // Open the file and upload its data
                await blobClient.UploadAsync(uploadedFile.OpenReadStream(), blobUploadOptions);

                // Retrieve the blob's properties
                BlobProperties blobProperties = await blobClient.GetPropertiesAsync();

                // Update the Azure Search Index
                SearchClient searchClient = new SearchClient(new Uri(_configuration.GetValue<string>("AZURE_SEARCH_URI")), _configuration.GetValue<string>("AZURE_SEARCH_INDEX"), new AzureKeyCredential(_configuration.GetValue<string>("AZURE_SEARCH_KEY")));
                Document document = new Document()
                {
                    Category = category,
                    SubCategory = subCategory,
                    metadata_storage_content_type = blobProperties.ContentType,
                    metadata_storage_size = (int)blobProperties.ContentLength,
                    metadata_storage_last_modified = blobProperties.LastModified,
                    metadata_storage_name = blobClient.Name,
                    metadata_storage_path = Convert.ToBase64String(Encoding.UTF8.GetBytes(blobClient.Uri.ToString())),
                    metadata_storage_file_extension = Path.GetExtension(uploadedFile.FileName),
                    metadata_creation_date = blobProperties.CreatedOn
                };
                List<Document> documents = new List<Document>() { document };
                IndexDocumentsResult indexResults = await searchClient.MergeOrUploadDocumentsAsync(documents);

                ViewBag.Message = $"File {fileName} uploaded successfully";
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchDocuments(string searchText, int startRow = 1, int numberOfRows = 5, string orderBy = "")
        {
            long? numberOfDocuments = 0;
            List<Document> documents = new List<Document>();

            if (!string.IsNullOrEmpty(searchText))
            {
                // Create a search client and set the search options
                SearchClient searchClient = new SearchClient(new Uri(_configuration.GetValue<string>("AZURE_SEARCH_URI")), _configuration.GetValue<string>("AZURE_SEARCH_INDEX"), new AzureKeyCredential(_configuration.GetValue<string>("AZURE_SEARCH_KEY")));
                SearchOptions searchOptions = new SearchOptions();
                searchOptions.IncludeTotalCount = true;
                searchOptions.SearchFields.Add("Category");
                searchOptions.SearchFields.Add("SubCategory");
                searchOptions.Skip = startRow - 1;
                searchOptions.Size = numberOfRows;
                if (!string.IsNullOrEmpty(orderBy))
                {
                    searchOptions.OrderBy.Add(orderBy);
                }
                // Retrieve the matching documents
                SearchResults<Document> searchResults = await searchClient.SearchAsync<Document>(searchText, searchOptions);
                numberOfDocuments = searchResults.TotalCount;
                AsyncPageable<SearchResult<Document>> results = searchResults.GetResultsAsync();
               
                await foreach (SearchResult<Document> result in results)
                {
                    // Decode the URL of the blob
                    string url = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(result.Document.metadata_storage_path.Substring(0, result.Document.metadata_storage_path.Length - 1)));
                    result.Document.metadata_storage_path = url;
                    documents.Add(result.Document);
                }
            }

            // Configure paging controls
            if (startRow > 1)
            {
                ViewBag.PreviousClass = "page-item";
                ViewBag.PreviousRow = startRow - numberOfRows;
            }
            else
            {
                ViewBag.PreviousClass = "page-item disabled";
                ViewBag.PreviousRow = startRow;
            }
            if (startRow + numberOfRows > numberOfDocuments.Value)
            {
                ViewBag.NextClass = "page-item disabled";
                ViewBag.NextRow = numberOfDocuments;
            }
            else
            {
                ViewBag.NextClass = "page-item";
                ViewBag.NextRow = startRow + numberOfRows;
            }
            ViewBag.NumberOfRows = numberOfRows;
            ViewBag.SearchText = searchText;
            ViewBag.OrderBy = orderBy;

            return View(documents);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
