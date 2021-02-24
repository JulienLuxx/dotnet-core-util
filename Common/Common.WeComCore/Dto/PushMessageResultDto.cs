using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.WeComCore
{
    public class PushMessageResultDto: WeComBaseResultDto
    {
        [JsonProperty("invaliduser")]
        public string InvalidUser { get; set; }

        [JsonProperty("invalidparty")]
        public string InvalidParty { get; set; }

        [JsonProperty("invalidtag")]
        public string InvalidTag { get; set; }
    }
}
