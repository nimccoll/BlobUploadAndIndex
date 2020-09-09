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
using System;
using System.Collections.Generic;

namespace BlobUploadAndIndex.Models
{
    public class Document
    {
        public string content { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string metadata_storage_content_type { get; set; }
        public int metadata_storage_size { get; set; }
        public DateTimeOffset metadata_storage_last_modified { get; set; }
        public string metadata_storage_name { get; set; }
        public string metadata_storage_path { get; set; }
        public string metadata_storage_file_extension { get; set; }
        public string metadata_content_type { get; set; }
        public string metadata_language { get; set; }
        public string metadata_author { get; set; }
        public string metadata_title { get; set; }
        public DateTimeOffset metadata_creation_date { get; set; }
    }

    public class SearchResults
    {
        public List<Document> value { get; set; }
    }
}
