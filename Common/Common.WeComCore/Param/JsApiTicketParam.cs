using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Common.WeComCore
{
    public interface IJsApiTicketParam
    {
        string AccessToken { get; set; }
    }

    public class JsApiTicketEnterpriseParam: IJsApiTicketParam
    {
        [Description("access_token")]
        public string AccessToken { get; set; }

        public JsApiTicketEnterpriseParam() { }

        public JsApiTicketEnterpriseParam(string accessToken)
        {
            AccessToken = accessToken;
        }
    }

    public class JsApiTicketAppParam: JsApiTicketEnterpriseParam, IJsApiTicketParam
    {
        [Description("type")]
        public string Type { get; set; }

        public JsApiTicketAppParam() : base() 
        {
            Type = "agent_config";
        }

        public JsApiTicketAppParam(string accessToken) : base(accessToken)
        {
            Type = "agent_config";
        }
    }
}
