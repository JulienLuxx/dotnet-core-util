using System;
using System.Collections.Generic;
using System.Text;
using Common.CoreUtil;

namespace Common.WeComCore
{
    public abstract class BaseHttpSvc
    {
        protected readonly IHttpClientUtil _httpUtil;

        public BaseHttpSvc(IHttpClientUtil httpClientUtil)
        {
            _httpUtil = httpClientUtil ?? throw new ArgumentNullException(nameof(httpClientUtil));
        }
    }
}

