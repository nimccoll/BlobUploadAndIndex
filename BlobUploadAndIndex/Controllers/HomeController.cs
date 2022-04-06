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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

        [HttpGet]
        public async Task<IActionResult> AdvancedSearch(string searchText, int startRow = 1, int numberOfRows = 5, string orderBy = "", string filter = "")
        {
            long? numberOfDocuments = 0;
            SearchResults model = new SearchResults()
            {
                value = new List<Document>(),
                Facets = new List<Facet>()
            };

            DateTime lastIndexUpdateDateTime = await GetLastIndexUpdateDateTime();

            if (!string.IsNullOrEmpty(searchText))
            {
                string filterString = string.Empty;
                if (!string.IsNullOrEmpty(filter))
                {
                    string[] filterCriteria = filter.Split("|");
                    filterString = $"{filterCriteria[0]} eq '{filterCriteria[1]}'";
                }

                // Create a search client and set the search options
                SearchClient searchClient = new SearchClient(new Uri(_configuration.GetValue<string>("AZURE_SEARCH_URI")), _configuration.GetValue<string>("AZURE_SEARCH_INDEX"), new AzureKeyCredential(_configuration.GetValue<string>("AZURE_SEARCH_KEY")));
                SearchOptions searchOptions = new SearchOptions();
                searchOptions.IncludeTotalCount = true;
                searchOptions.SearchFields.Add("Category");
                searchOptions.SearchFields.Add("SubCategory");
                searchOptions.SearchFields.Add("metadata_storage_name");
                searchOptions.Skip = startRow - 1;
                searchOptions.Size = numberOfRows;
                searchOptions.Facets.Add("Category");
                searchOptions.Facets.Add("SubCategory");
                searchOptions.Facets.Add("metadata_storage_file_extension");
                if (!string.IsNullOrEmpty(orderBy))
                {
                    searchOptions.OrderBy.Add(orderBy);
                }
                if (!string.IsNullOrEmpty(filterString))
                {
                    searchOptions.Filter = filterString;
                }

                // Retrieve the matching documents
                SearchResults<Document> searchResults = await searchClient.SearchAsync<Document>(searchText, searchOptions);
                numberOfDocuments = searchResults.TotalCount;

                // Process facets
                foreach (KeyValuePair<string, IList<FacetResult>> facet in searchResults.Facets)
                {
                    Facet facetOut = new Facet()
                    {
                        FacetName = facet.Key,
                        FacetValues = new Dictionary<string, long?>()
                    };
                    foreach (FacetResult facetResult in facet.Value)
                    {
                        facetOut.FacetValues.Add(facetResult.Value.ToString(), facetResult.Count);
                    }
                    model.Facets.Add(facetOut);
                }

                AsyncPageable<SearchResult<Document>> results = searchResults.GetResultsAsync();

                await foreach (SearchResult<Document> result in results)
                {
                    // Decode the URL of the blob
                    string url = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(result.Document.metadata_storage_path.Substring(0, result.Document.metadata_storage_path.Length - 1)));
                    result.Document.metadata_storage_path = url;
                    model.value.Add(result.Document);
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
            ViewBag.Filter = filter;
            ViewBag.LastIndexUpdateDateTime = lastIndexUpdateDateTime.ToString("g");

            return View(model);
        }

        public JsonResult AutoComplete(string searchText)
        {
            SearchClient searchClient = new SearchClient(new Uri(_configuration.GetValue<string>("AZURE_SEARCH_URI")), _configuration.GetValue<string>("AZURE_SEARCH_INDEX"), new AzureKeyCredential(_configuration.GetValue<string>("AZURE_SEARCH_KEY")));
            AutocompleteOptions autocompleteOptions = new AutocompleteOptions()
            {
                Mode = AutocompleteMode.OneTermWithContext
            };
            Response<AutocompleteResults> autocompleteResult = searchClient.Autocomplete(searchText, "documentSuggester", autocompleteOptions);

            // Convert the autocompleteResult results to a list that can be displayed in the client.
            List<string> searchResults = autocompleteResult.Value.Results.Select(x => x.Text).ToList();
            return Json(searchResults);
        }

        private async Task<DateTime> GetLastIndexUpdateDateTime()
        {
            DateTime lastUpdateDateTime = DateTime.MinValue;
            HttpClient httpClient = new HttpClient();
            string url = $"{_configuration.GetValue<string>("AZURE_SEARCH_URI")}/indexers/{_configuration.GetValue<string>("AZURE_SEARCH_INDEXER")}/status?api-version=2020-06-30";

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("api-key", _configuration.GetValue<string>("AZURE_SEARCH_KEY"));
            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string indexerStatusString = await response.Content.ReadAsStringAsync();
                IndexerStatus indexerStatus = JsonConvert.DeserializeObject<IndexerStatus>(indexerStatusString);
                if (indexerStatus.lastResult.status == "success")
                {
                    lastUpdateDateTime = indexerStatus.lastResult.endTime;
                }
            }

            return lastUpdateDateTime;
        }

    }
}
