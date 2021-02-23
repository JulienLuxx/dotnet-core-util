using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.WeComCore
{
    public class UserInfoResultDto : WeComBaseResultDto
    {
        public string UserId { get; set; }

        public string OpenId { get; set; }

        public string DeviceId { get; set; }

        [JsonProperty("external_userid")]
        public string ExternalUserId { get; set; }
    }
}
