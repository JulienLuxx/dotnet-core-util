using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Common.WeComCore
{
    public interface IWeComAccesssParam
    {
        string AccessToken { get; set; }
    }

    public class WeComAccessParam : IWeComAccesssParam
    {
        [Description("access_token")]
        public string AccessToken { get; set; }
    }
}
