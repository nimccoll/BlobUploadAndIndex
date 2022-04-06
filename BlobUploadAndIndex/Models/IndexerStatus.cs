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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BlobUploadAndIndex.Models
{
    // IndexerStatus myDeserializedClass = JsonConvert.DeserializeObject<IndexerStatus>(myJsonResponse);
    public class Warning
    {
        public string key { get; set; }
        public string name { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public string documentationLink { get; set; }
    }

    public class LastResult
    {
        public string status { get; set; }
        public object errorMessage { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public int itemsProcessed { get; set; }
        public int itemsFailed { get; set; }
        public string initialTrackingState { get; set; }
        public string finalTrackingState { get; set; }
        public List<object> errors { get; set; }
        public List<Warning> warnings { get; set; }
        public object metrics { get; set; }
    }

    public class ExecutionHistory
    {
        public string status { get; set; }
        public object errorMessage { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public int itemsProcessed { get; set; }
        public int itemsFailed { get; set; }
        public string initialTrackingState { get; set; }
        public string finalTrackingState { get; set; }
        public List<object> errors { get; set; }
        public List<Warning> warnings { get; set; }
        public object metrics { get; set; }
    }

    public class Limits
    {
        public string maxRunTime { get; set; }
        public int maxDocumentExtractionSize { get; set; }
        public int maxDocumentContentCharactersToExtract { get; set; }
    }

    public class IndexerStatus
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public LastResult lastResult { get; set; }
        public List<ExecutionHistory> executionHistory { get; set; }
        public Limits limits { get; set; }
    }
}
