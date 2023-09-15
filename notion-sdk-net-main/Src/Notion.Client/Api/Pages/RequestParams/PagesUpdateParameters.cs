﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Notion.Client
{
    public class PagesUpdateParameters : IPagesUpdateBodyParameters
    {
        [JsonProperty("icon")]
        public IPageIcon Icon { get; set; }

        [JsonProperty("cover")]
        public FileObject Cover { get; set; }

        [JsonProperty("archived")]
        public bool Archived { get; set; }

        [JsonProperty("properties")]
        public IDictionary<string, PropertyValue> Properties { get; set; }
    }
}
