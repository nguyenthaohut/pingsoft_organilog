﻿using Newtonsoft.Json;
using System;

namespace Organilog.Models.Response
{
    public class CategoryTrackingResponse
    {
        [JsonProperty("0")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("1")]
        public int ServerId { get; set; }

        [JsonProperty("2")]
        public int Code { get; set; }

        [JsonProperty("3")]
        public string Title { get; set; }

        [JsonProperty("4")]
        public int IsActif { get; set; }

        [JsonProperty("5")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("k")]
        public string K { get; set; }
    }
}