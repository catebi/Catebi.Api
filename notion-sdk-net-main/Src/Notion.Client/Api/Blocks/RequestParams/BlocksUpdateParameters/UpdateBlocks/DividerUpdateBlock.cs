﻿using Newtonsoft.Json;

namespace Notion.Client
{
    public class DividerUpdateBlock : IUpdateBlock
    {
        public DividerUpdateBlock()
        {
            Divider = new Info();
        }

        [JsonProperty("divider")]
        public Info Divider { get; set; }

        public bool Archived { get; set; }

        public class Info
        {
        }
    }
}
