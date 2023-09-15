﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Notion.Client
{
    public class EmbedBlock : Block, IColumnChildrenBlock, INonColumnBlock
    {
        [JsonProperty("embed")]
        public Info Embed { get; set; }

        public override BlockType Type => BlockType.Embed;

        public class Info
        {
            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("caption")]
            public IEnumerable<RichTextBase> Caption { get; set; }
        }
    }
}
