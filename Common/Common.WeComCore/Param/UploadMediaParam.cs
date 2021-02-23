using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Common.WeComCore
{
    public class UploadMediaParam : WeComAccessParam
    {
        [Description("type")]
        public string Type { get; set; }
    }
}
