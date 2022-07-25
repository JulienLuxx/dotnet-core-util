using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Common.BaiduAICore
{
    public interface IHttpClientResult
    {
        dynamic Result { get; }

        bool IsSuccess { get; set; }

        HttpStatusCode ResultCode { get; set; }
    }

    public interface IHttpClientResult<T> : IHttpClientResult
    {
        new T Result { get; set; }
    }

    public struct HttpClientResult : IHttpClientResult
    {
        public dynamic Result { get; set; }

        public bool IsSuccess { get; set; }

        public HttpStatusCode ResultCode { get; set; }

        public HttpClientResult(dynamic result, bool isSuccess, HttpStatusCode resultCode)
        {
            Result = result;
            IsSuccess = isSuccess;
            ResultCode = resultCode;
        }
    }

    public struct HttpClientResult<T> : IHttpClientResult<T>
    {
        public T Result { get; set; }
        dynamic IHttpClientResult.Result { get => Result; }

        public bool IsSuccess { get; set; }

        public HttpStatusCode ResultCode { get; set; }

        public HttpClientResult(T result, bool isSuccess, HttpStatusCode resultCode)
        {
            Result = result;
            IsSuccess = isSuccess;
            ResultCode = resultCode;
        }
    }
}
