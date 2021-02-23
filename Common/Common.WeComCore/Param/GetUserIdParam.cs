using System;
using System.Collections.Generic;
using System.Text;

namespace Common.WeComCore
{
    public class GetUserIdParam : WeComAccessParam, IWeComAccesssParam
    {
        public string Code { get; set; }

        public GetUserIdParam(string accessToken, string code)
        {
            AccessToken = accessToken;
            Code = code;
        }
    }
}
