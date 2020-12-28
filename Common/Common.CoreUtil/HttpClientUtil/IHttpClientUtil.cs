using Common.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.CoreUtil
{
    public interface IHttpClientUtil: IDependency
    {
        Task<IHttpResult<string>> PostFileAsync<T>(Stream stream, string fileName, T param, string url, string contentName = "files", CancellationToken cancellationToken = default, Encoding encoding = null);

        /// <summary>
        /// AsyncSendPackage(Suppot use DescriptionAttribute in param)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="mediaType"></param>
        /// <param name="url"></param>
        /// <param name="httpMethodStr"></param>
        /// <param name="jsonConvertOption"></param>
        /// <param name="encoding"></param>
        /// <param name="cookiesArray"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        Task<IHttpResult<string>> SendAsync<T>(T param, MediaTypeEnum mediaType, string url, string httpMethodStr, JsonConvertOptionEnum jsonConvertOption = JsonConvertOptionEnum.NewtonSoftJson, CancellationToken cancellationToken = default, Encoding encoding = null, string[] cookiesArray = null, string userAgent = null);

        /// <summary>
        /// AsyncSendPackage(Not suppot use DescriptionAttribute in param)
        /// </summary>
        /// <param name="param">RequestParam</param>
        /// <param name="url">RequestUrl</param>
        /// <param name="httpMethod">RequestHttpMethod</param>
        /// <param name="mediaType">RequestParamMediaType</param>
        /// <param name="cookieList">RequestCookieStringList</param>
        /// <param name="userAgent">RequestHeaderUserAgent</param>
        /// <returns></returns>
        Task<HttpResultDto> SendAsync(dynamic param, string url, HttpMethod httpMethod, MediaTypeEnum mediaType, List<string> cookieList = null, string userAgent = null);

        /// <summary>
        /// AsyncSendPackage(Not suppot use DescriptionAttribute in param)
        /// </summary>
        /// <param name="param">RequestParam</param>
        /// <param name="url">RequestUrl</param>
        /// <param name="httpMethodStr">RequestHttpMethodString(get/post)</param>
        /// <param name="mediaType">RequestParamMediaType</param>
        /// <param name="cookieList">RequestCookieStringList</param>
        /// <param name="userAgent">RequestHeaderUserAgent</param>
        /// <returns></returns>
        Task<HttpResultDto> SendAsync(dynamic param, string url, string httpMethodStr, MediaTypeEnum mediaType, List<string> cookieList = null, string userAgent = null);

        /// <summary>
        /// AsyncSendPackage(Suppot use DescriptionAttribute in param)
        /// </summary>
        /// <typeparam name="T">RequestParamType</typeparam>
        /// <param name="param">RequestParam</param>
        /// <param name="url">RequestUrl</param>
        /// <param name="httpMethodStr">RequestHttpMethodString(get/post)</param>
        /// <param name="mediaType">RequestParamMediaType</param>
        /// <param name="encoding">RequestResultStringEncoding(DefaultValueNull),If null default use ResponseHeader Charset</param>
        /// <param name="cookieList">RequestCookieStringList</param>
        /// <param name="userAgent">RequestHeaderUserAgent</param>
        /// <returns></returns>
        Task<HttpResultDto> SendAsync<T>(T param, string url, string httpMethodStr, MediaTypeEnum mediaType, Encoding encoding = null, List<string> cookieList = null, string userAgent = null) where T : class;
        
        /// <summary>
        /// AsyncSendPackage(Not suppot use DescriptionAttribute in param)
        /// </summary>
        /// <param name="param">RequestParam</param>
        /// <param name="url">RequestUrl</param>
        /// <param name="httpMethod">RequestHttpMethod</param>
        /// <param name="mediaType">RequestParamMediaType</param>
        /// <param name="isParamConvertCookies">ConvertRequestToCookies</param>
        /// <param name="cookieList">RequestCookieStringList</param>
        /// <param name="userAgent">RequestHeaderUserAgent</param>
        /// <returns></returns>
        Task<HttpResultDto> SendAsync(dynamic param, string url, HttpMethod httpMethod, MediaTypeEnum mediaType, bool isParamConvertCookies, List<string> cookieList = null, string userAgent = null);

        /// <summary>
        /// AsyncSendPackage(Not suppot use DescriptionAttribute in param)
        /// </summary>
        /// <param name="param">RequestParam</param>
        /// <param name="url">RequestUrl</param>
        /// <param name="httpMethodStr">RequestHttpMethodString(get/post)</param>
        /// <param name="mediaType">RequestParamMediaType</param>
        /// <param name="isParamConvertCookies">ConvertRequestToCookies</param>
        /// <param name="cookieList">RequestCookieStringList</param>
        /// <param name="userAgent">RequestHeaderUserAgent</param>
        /// <returns></returns>
        Task<HttpResultDto> SendAsync(dynamic param, string url, string httpMethodStr, MediaTypeEnum mediaType, bool isParamConvertCookies, List<string> cookieList = null, string userAgent = null);

        /// <summary>
        /// AsyncSendPackage(Suppot use DescriptionAttribute in param)
        /// </summary>
        /// <typeparam name="T">RequestParamType</typeparam>
        /// <param name="param">RequestParam</param>
        /// <param name="url">RequestUrl</param>
        /// <param name="httpMethodStr">RequestHttpMethodString(get/post)</param>
        /// <param name="mediaType">RequestParamMediaType</param>
        /// <param name="isParamConvertCookies">ConvertRequestToCookies</param>
        /// <param name="cookieList">RequestCookieStringList</param>
        /// <param name="userAgent">RequestHeaderUserAgent</param>
        /// <returns></returns>
        [Obsolete]
        Task<HttpResultDto> SendAsync<T>(T param, string url, string httpMethodStr, MediaTypeEnum mediaType, bool isParamConvertCookies, List<string> cookieList = null, string userAgent = null) where T : class;

        /// <summary>
        /// AsyncSendPackage,GetResultInStream(Not suppot use DescriptionAttribute in param)
        /// </summary>
        /// <param name="param">RequestParam</param>
        /// <param name="url">RequestUrl</param>
        /// <param name="httpMethodStr">RequestHttpMethodString(get/post)</param>
        /// <param name="mediaType">RequestParamMediaType</param>
        /// <param name="cancellationToken"></param>
        /// <param name="userAgent">RequestHeaderUserAgent</param>
        /// <returns></returns>
        Task<HttpStreamResultDto> GetStreamAsync(dynamic param, string url, string httpMethodStr, MediaTypeEnum mediaType, CancellationToken cancellationToken = default, string userAgent = null);

        /// <summary>
        /// AsyncSendPackage,GetResultInStream(Suppot use DescriptionAttribute in param)
        /// </summary>
        /// <typeparam name="T">RequestParamType</typeparam>
        /// <param name="param">RequestParam</param>
        /// <param name="url">RequestUrl</param>
        /// <param name="httpMethodStr">RequestHttpMethodString(get/post)</param>
        /// <param name="mediaType">RequestParamMediaType</param>
        /// <param name="cancellationToken"></param>
        /// <param name="userAgent">RequestHeaderUserAgent</param>
        /// <returns></returns>
        Task<HttpStreamResultDto> GetStreamAsync<T>(T param, string url, string httpMethodStr, MediaTypeEnum mediaType, CancellationToken cancellationToken = default, string userAgent = null) where T : class;
    }
}
