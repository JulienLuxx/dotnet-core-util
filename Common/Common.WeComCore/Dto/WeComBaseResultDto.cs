using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.WeComCore
{
    public interface IWeComResultDto
    {
        int ErrCode { get; set; }

        string ErrMsg { get; set; }
    }

    public class WeComBaseResultDto : IWeComResultDto
    {
        [JsonProperty("errcode")]
        public int ErrCode { get; set; }

        [JsonProperty("errmsg")]
        public string ErrMsg { get; set; }

        public WeComBaseResultDto() { }

        public WeComBaseResultDto(int errCode, string errMsg)
        {
            ErrCode = errCode;
            ErrMsg = errMsg;
        }
    }
}
