using Common.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.CoreUtil
{
    public interface IHttpClientUtil: IDependency
    {
        /// <summary>
        /// AsyncSendPackage
        /// </summary>
        /// <param name="param">RequestParam</param>
        /// <param name="url">RequestUrl</param>
        /// <param name="httpMethod">RequestHttpMethod</param>
        /// <param name="mediaType">RequestParamMediaType</param>
        /// <param name="cookieList">RequestCookieStringList</param>
        /// <param name="userAgent">RequestHeaderUserAgent</param>
        /// <returns></returns>
        Task<HttpResult> SendAsync(dynamic param, string url, HttpMethod httpMethod, MediaTypeEnum mediaType, List<string> cookieList = null, string userAgent = null);

        /// <summary>
        /// AsyncSendPackage
        /// </summary>
        /// <param name="param">RequestParam</param>
        /// <param name="url">RequestUrl</param>
        /// <param name="httpMethodStr">RequestHttpMethodString(get/post)</param>
        /// <param name="mediaType">RequestParamMediaType</param>
        /// <param name="cookieList">RequestCookieStringList</param>
        /// <param name="userAgent">RequestHeaderUserAgent</param>
        /// <returns></returns>
        Task<HttpResult> SendAsync(dynamic param, string url, string httpMethodStr, MediaTypeEnum mediaType, List<string> cookieList = null, string userAgent = null);

        /// <summary>
        /// AsyncSendPackage
        /// </summary>
        /// <param name="param">RequestParam</param>
        /// <param name="url">RequestUrl</param>
        /// <param name="httpMethod">RequestHttpMethod</param>
        /// <param name="mediaType">RequestParamMediaType</param>
        /// <param name="isParamConvertCookies">ConvertRequestToCookies</param>
        /// <param name="cookieList">RequestCookieStringList</param>
        /// <param name="userAgent">RequestHeaderUserAgent</param>
        /// <returns></returns>
        Task<HttpResult> SendAsync(dynamic param, string url, HttpMethod httpMethod, MediaTypeEnum mediaType, bool isParamConvertCookies, List<string> cookieList = null, string userAgent = null);

        /// <summary>
        /// AsyncSendPackage
        /// </summary>
        /// <param name="param">RequestParam</param>
        /// <param name="url">RequestUrl</param>
        /// <param name="httpMethodStr">RequestHttpMethodString(get/post)</param>
        /// <param name="mediaType">RequestParamMediaType</param>
        /// <param name="isParamConvertCookies">ConvertRequestToCookies</param>
        /// <param name="cookieList">RequestCookieStringList</param>
        /// <param name="userAgent">RequestHeaderUserAgent</param>
        /// <returns></returns>
        Task<HttpResult> SendAsync(dynamic param, string url, string httpMethodStr, MediaTypeEnum mediaType, bool isParamConvertCookies, List<string> cookieList = null, string userAgent = null);

        /// <summary>
        /// AsyncSendPackage,GetResultInStream
        /// </summary>
        /// <param name="param">RequestParam</param>
        /// <param name="url">RequestUrl</param>
        /// <param name="httpMethodStr">RequestHttpMethodString(get)(Temporarily only supported Get)</param>
        /// <param name="mediaType">RequestParamMediaType</param>
        /// <param name="userAgent">RequestHeaderUserAgent</param>
        /// <returns></returns>
        Task<HttpStreamResult> GetStreamAsync(dynamic param, string url, string httpMethodStr, MediaTypeEnum mediaType, string userAgent = null);
    }
}
