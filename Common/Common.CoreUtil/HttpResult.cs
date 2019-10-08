using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Common.CoreUtil
{
    public class HttpResult
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public HttpResult()
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cookies">ResponseCookies</param>
        public HttpResult(List<string> cookies)
        {
            Cookies = cookies;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="result">ResponseContentString</param>
        public HttpResult(string result)
        {
            Result = result;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="isSuccess">IsResponseSuccess</param>
        public HttpResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="result">ResponseContentString</param>
        /// <param name="cookies">ResponseCookies</param>
        /// <param name="isSuccess">IsResponseSuccess</param>
        public HttpResult(string result, List<string> cookies, bool isSuccess)
        {
            Result = result;
            Cookies = cookies;
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="result">ResponseContentString</param>
        /// <param name="cookies">ResponseCookies</param>
        /// <param name="resultCode">ResultStatusCode</param>
        /// <param name="isSuccess">IsResponseSuccess</param>
        public HttpResult(string result, List<string> cookies, HttpStatusCode resultCode, bool isSuccess) 
        {
            Result = result;
            Cookies = cookies;
            ResultCode = resultCode;
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// ResponseContentString
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// ResponseCookies
        /// </summary>
        public List<string> Cookies { get; set; }

        /// <summary>
        /// IsResponseSuccess
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// ResultStatusCode
        /// </summary>
        public HttpStatusCode ResultCode { get; set; }
    }

    public class HttpStreamResult
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public HttpStreamResult()
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="stream">ResultStream</param>
        /// <param name="isSuccess">IsResponseSuccess</param>
        public HttpStreamResult(MemoryStream stream,bool isSuccess)
        {
            Stream = stream;
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="stream">ResultStream</param>
        /// <param name="resultCode">ResultStatusCode</param>
        /// <param name="isSuccess">IsResponseSuccess</param>
        public HttpStreamResult(MemoryStream stream, HttpStatusCode resultCode, bool isSuccess) 
        {
            Stream = stream;
            ResultCode = resultCode;
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// ResultStream
        /// </summary>
        public MemoryStream Stream { get; set; }

        /// <summary>
        /// ResultStatusCode
        /// </summary>
        public HttpStatusCode ResultCode { get; set; }

        /// <summary>
        /// IsResponseSuccess
        /// </summary>
        public bool IsSuccess { get; set; }
    }
}
