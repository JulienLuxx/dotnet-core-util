using System;
using System.Collections.Generic;
using System.Text;

namespace Common.CoreUtil
{
    public class HttpResult
    {
        public HttpResult()
        { }

        public HttpResult(List<string> cookies)
        {
            Cookies = cookies;
        }

        public HttpResult(string result)
        {
            Result = result;
        }

        public HttpResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public HttpResult(string result, List<string> cookies, bool isSuccess)
        {
            Result = result;
            Cookies = cookies;
            IsSuccess = isSuccess;
        }

        public string Result { get; set; }

        public List<string> Cookies { get; set; }

        public bool IsSuccess { get; set; }
    }
}
