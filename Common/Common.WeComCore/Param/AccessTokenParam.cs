using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Common.WeComCore
{
    public class AccessTokenParam
    {
        [Description("corpid")]
        public string CorpId { get; set; }

        [Description("corpsecret")]
        public string CorpSecret { get; set; }
    }
}
