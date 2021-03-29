using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.WeComCore
{
    public class JsApiTicketResultDto : WeComBaseResultDto, IWeComResultDto
    {
        [JsonProperty("ticket")]
        public string Ticket { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
