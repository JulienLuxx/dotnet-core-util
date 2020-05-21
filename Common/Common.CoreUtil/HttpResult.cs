using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Common.CoreUtil
{
    public interface IHttpResult
    { 
        dynamic Result { get; }

        bool IsSuccess { get; }

        string[] Cookies { get; }

        bool HasCookies { get; }

        HttpStatusCode ResultCode { get; }
    }

    public interface IHttpResult<T> : IHttpResult
    {
        new T Result { get; set; }
    }

    public class HttpResult : IHttpResult
    {
         public dynamic Result { get; }

         public bool IsSuccess { get; }

         public string[] Cookies { get; }

        public bool HasCookies { get; }

        public HttpStatusCode ResultCode { get; }

        public HttpResult(HttpStatusCode resultCode, bool isSuccess = false) 
        {
            Result = resultCode.ToString();
            IsSuccess = isSuccess;
            Cookies = null;
            HasCookies = false;
            ResultCode = resultCode;
        }
    }

    public class HttpResult<T> : IHttpResult<T>
    {
        public T Result { get; set; }

        dynamic IHttpResult.Result { get => Result; }

        public bool IsSuccess { get; }

        public string[] Cookies { get; }

        public bool HasCookies { get; }

        public HttpStatusCode ResultCode { get; }

        public HttpResult(T result, HttpStatusCode resultCode, bool isSuccess = false, string[] cookies = null)
        {
            Result = result;
            IsSuccess = isSuccess;
            Cookies = cookies;
            HasCookies = null != cookies && cookies.Any();
            ResultCode = resultCode;
        }
    }

    public class HttpResultDto
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public HttpResultDto()
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cookies">ResponseCookies</param>
        public HttpResultDto(List<string> cookies)
        {
            Cookies = cookies;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="result">ResponseContentString</param>
        public HttpResultDto(string result)
        {
            Result = result;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="isSuccess">IsResponseSuccess</param>
        public HttpResultDto(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="result">ResponseContentString</param>
        /// <param name="cookies">ResponseCookies</param>
        /// <param name="isSuccess">IsResponseSuccess</param>
        public HttpResultDto(string result, List<string> cookies, bool isSuccess)
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
        public HttpResultDto(string result, List<string> cookies, HttpStatusCode resultCode, bool isSuccess) 
        {
            Result = result;
            Cookies = cookies;
            ResultCode = resultCode;
            IsSuccess = isSuccess;
        }

        /// <summary>
        ///  Ctor
        /// </summary>
        /// <param name="result">ResponseContentString</param>
        /// <param name="cookieArray">ResponseCookies</param>
        /// <param name="resultCode">ResultStatusCode</param>
        /// <param name="isSuccess">IsResponseSuccess</param>
        public HttpResultDto(string result, string[] cookieArray, HttpStatusCode resultCode, bool isSuccess)
        {
            Result = result;
            Cookies = cookieArray.ToList();
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

    public class HttpStreamResultDto
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public HttpStreamResultDto()
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="stream">ResultStream</param>
        /// <param name="isSuccess">IsResponseSuccess</param>
        public HttpStreamResultDto(MemoryStream stream,bool isSuccess)
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
        public HttpStreamResultDto(MemoryStream stream, HttpStatusCode resultCode, bool isSuccess) 
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
