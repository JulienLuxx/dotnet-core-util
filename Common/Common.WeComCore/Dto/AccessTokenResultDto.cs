using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.WeComCore
{
    public class AccessTokenResultDto : WeComBaseResultDto, IWeComResultDto
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
