using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.WeComCore
{
    public class UploadMediaResultDto : WeComBaseResultDto
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("media_id")]
        public string MediaId { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
    }
}
