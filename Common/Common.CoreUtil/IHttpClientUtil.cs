using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.CoreUtil
{
    public interface IHttpClientUtil
    {
        Task<HttpResult> SendAsync(dynamic param, string url, HttpMethod httpMethod, MediaTypeEnum mediaType, List<string> cookieList = null, string userAgent = null);

        Task<HttpResult> SendAsync(dynamic param, string url, string httpMethodStr, MediaTypeEnum mediaType, List<string> cookieList = null, string userAgent = null);
    }
}
