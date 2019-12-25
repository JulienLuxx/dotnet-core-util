using Common.Util;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.CoreUtil
{
    public class HttpClientUtil : IHttpClientUtil
    {
        private IHttpClientFactory _clientFactory { get; set; }

        private IMapUtil _mapUtil { get; set; }

        public HttpClientUtil(IHttpClientFactory clientFactory, IMapUtil mapUtil)
        {
            _clientFactory = clientFactory;
            _mapUtil = mapUtil;
        }

        public async Task<HttpResult> SendAsync(dynamic param, string url, HttpMethod httpMethod, MediaTypeEnum mediaType, List<string> cookieList = null, string userAgent = null)
        {
            using (var request = new HttpRequestMessage(httpMethod, @url))
            {
                if ((HttpMethod.Get.Equals(httpMethod)))
                {
                    var dict = _mapUtil.DynamicToDictionary(param);
                    switch (mediaType)
                    {
                        case MediaTypeEnum.UrlQuery:
                            var paramUrl = QueryHelpers.AddQueryString(@url, dict);
                            request.RequestUri = new Uri(paramUrl);
                            break;
                        case MediaTypeEnum.ApplicationFormUrlencoded:
                            request.Content = new FormUrlEncodedContent(dict);
                            break;
                    }

                }
                else if (HttpMethod.Post.Equals(httpMethod))
                {
                    var dict = _mapUtil.DynamicToDictionary(param);
                    switch (mediaType)
                    {
                        case MediaTypeEnum.ApplicationFormUrlencoded:
                            request.Content = new FormUrlEncodedContent(dict);
                            break;
                        case MediaTypeEnum.ApplicationJson:
                            var jsonParam = JsonConvert.SerializeObject(param);
                            request.Content = new StringContent(jsonParam, Encoding.UTF8, "application/json");
                            break;
                        case MediaTypeEnum.MultipartFormData:
                            var content = new MultipartFormDataContent();
                            foreach (var item in dict)
                            {
                                content.Add(new StringContent(item.Value), item.Key);
                            }
                            request.Content = content;
                            break;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                if (null != cookieList && cookieList.Any())
                {
                    request.Headers.Add("Set-Cookie", cookieList);
                }
                if (!string.IsNullOrEmpty(userAgent) && !string.IsNullOrWhiteSpace(userAgent))
                {
                    request.Headers.UserAgent.ParseAdd(userAgent);
                }
                using (var client = _clientFactory.CreateClient())
                {
                    using (var response = await client.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            var cookieFlag = response.Headers.TryGetValues("Set-Cookie", out var setCookies);
                            if (cookieFlag)
                            {
                                return new HttpResult(result, setCookies.ToList(), response.StatusCode, true);
                            }
                            else
                            {
                                return new HttpResult(result, new List<string>(), response.StatusCode, true);
                            }
                        }
                        else
                        {
                            return new HttpResult(response.StatusCode.ToString(), new List<string>(), response.StatusCode, false);
                        }
                    }

                }
            }
        }

        public async Task<HttpResult> SendAsync(dynamic param, string url, string httpMethodStr, MediaTypeEnum mediaType, List<string> cookieList = null, string userAgent = null)
        {
            var httpMethod = new HttpMethod(httpMethodStr.ToUpper());
            using (var request = new HttpRequestMessage(httpMethod, @url))
            {
                if ((HttpMethod.Get.Equals(httpMethod)))
                {
                    var dict = _mapUtil.DynamicToDictionary(param);
                    switch (mediaType)
                    {
                        case MediaTypeEnum.UrlQuery:
                            var paramUrl = QueryHelpers.AddQueryString(@url, dict);
                            request.RequestUri = new Uri(paramUrl);
                            break;
                        case MediaTypeEnum.ApplicationFormUrlencoded:
                            request.Content = new FormUrlEncodedContent(dict);
                            break;
                    }

                }
                else if (HttpMethod.Post.Equals(httpMethod))
                {
                    var dict = _mapUtil.DynamicToDictionary(param);
                    switch (mediaType)
                    {
                        case MediaTypeEnum.ApplicationFormUrlencoded:
                            request.Content = new FormUrlEncodedContent(dict);
                            break;
                        case MediaTypeEnum.ApplicationJson:
                            var jsonParam = JsonConvert.SerializeObject(param);
                            request.Content = new StringContent(jsonParam, Encoding.UTF8, "application/json");
                            break;
                        case MediaTypeEnum.MultipartFormData:
                            var content = new MultipartFormDataContent();
                            foreach (var item in dict)
                            {
                                content.Add(new StringContent(item.Value), item.Key);
                            }
                            request.Content = content;
                            break;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                if (null != cookieList && cookieList.Any())
                {
                    request.Headers.Add("Set-Cookie", cookieList);
                }
                if (!string.IsNullOrEmpty(userAgent) && !string.IsNullOrWhiteSpace(userAgent))
                {
                    request.Headers.UserAgent.ParseAdd(userAgent);
                }
                using (var client = _clientFactory.CreateClient())
                {
                    using (var response = await client.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            var cookieFlag = response.Headers.TryGetValues("Set-Cookie", out var setCookies);
                            if (cookieFlag)
                            {
                                return new HttpResult(result, setCookies.ToList(), response.StatusCode, true);
                            }
                            else
                            {
                                return new HttpResult(result, new List<string>(), response.StatusCode, true);
                            }
                        }
                        else
                        {
                            return new HttpResult(response.StatusCode.ToString(), new List<string>(), response.StatusCode, false);
                        }
                    }
                }
            }
        }

        public async Task<HttpResult> SendAsync<T>(T param, string url, string httpMethodStr, MediaTypeEnum mediaType, Encoding encoding = null, List<string> cookieList = null, string userAgent = null) where T : class 
        {
            var httpMethod = new HttpMethod(httpMethodStr.ToUpper());
            using(var request = new HttpRequestMessage(httpMethod, @url))
            {
                if ((HttpMethod.Get.Equals(httpMethod)))
                {
                    var dict = _mapUtil.EntityToDictionary(param);
                    switch (mediaType)
                    {
                        case MediaTypeEnum.UrlQuery:
                            var paramUrl = QueryHelpers.AddQueryString(@url, dict);
                            request.RequestUri = new Uri(paramUrl);
                            break;
                        case MediaTypeEnum.ApplicationFormUrlencoded:
                            request.Content = new FormUrlEncodedContent(dict);
                            break;
                    }
                }
                else if (HttpMethod.Post.Equals(httpMethod))
                {
                    var dict = _mapUtil.EntityToDictionary(param);
                    switch (mediaType)
                    {
                        case MediaTypeEnum.ApplicationFormUrlencoded:
                            request.Content = new FormUrlEncodedContent(dict);
                            break;
                        case MediaTypeEnum.ApplicationJson:
                            var jsonParam = JsonConvert.SerializeObject(param);
                            request.Content = new StringContent(jsonParam, Encoding.UTF8, "application/json");
                            break;
                        case MediaTypeEnum.MultipartFormData:
                            var content = new MultipartFormDataContent();
                            foreach (var item in dict)
                            {
                                content.Add(new StringContent(item.Value), item.Key);
                            }
                            request.Content = content;
                            break;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                if (null != cookieList && cookieList.Any())
                {
                    request.Headers.Add("Set-Cookie", cookieList);
                }
                if (!string.IsNullOrEmpty(userAgent) && !string.IsNullOrWhiteSpace(userAgent))
                {
                    request.Headers.UserAgent.ParseAdd(userAgent);
                }
                using (var client = _clientFactory.CreateClient())
                {
                    using(var response = await client.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var cookieFlag = response.Headers.TryGetValues("Set-Cookie", out var setCookies);
                            if (response.Content.Headers.ContentLength.HasValue)
                            {
                                if (response.Content.Headers.ContentLength < 81920)
                                {
                                    var result = await response.Content.ReadAsStringAsync();
                                    return new HttpResult(result, (cookieFlag ? setCookies.ToList() : new List<string>()), true);
                                }
                                else
                                {
                                    using (var memoryStream = new MemoryStream())
                                    {
                                        var charSet = response.Content.Headers.ContentType.CharSet;
                                        if (null == encoding)
                                        {
                                            if (string.IsNullOrEmpty(charSet))
                                            {
                                                encoding = Encoding.Default;
                                            }
                                            else
                                            {
                                                encoding = Encoding.GetEncoding(charSet);
                                            }
                                        }
                                        await response.Content.CopyToAsync(memoryStream);
                                        memoryStream.Seek(0, SeekOrigin.Begin);
                                        //var byteArray = new byte[memoryStream.Length];
                                        //await memoryStream.ReadAsync(byteArray, 0, byteArray.Length);
                                        var byteArray = memoryStream.GetBuffer();
                                        var resultStr = encoding.GetString(byteArray);
                                        return new HttpResult(resultStr, (cookieFlag ? setCookies.ToList() : new List<string>()), true);
                                    }
                                }
                            }
                            else
                            {
                                return new HttpResult(string.Empty, (cookieFlag ? setCookies.ToList() : new List<string>()), true);
                            }

                        }
                        else
                        {
                            return new HttpResult(response.StatusCode.ToString(), new List<string>(), response.StatusCode, false);
                        }
                    }
                }
            }

        }

        public async Task<HttpResult> SendAsync(dynamic param, string url, HttpMethod httpMethod, MediaTypeEnum mediaType, bool isParamConvertCookies, List<string> cookieList = null, string userAgent = null) 
        {
            using (var request = new HttpRequestMessage(httpMethod, @url))
            {
                if ((HttpMethod.Get.Equals(httpMethod)))
                {
                    var dict = _mapUtil.DynamicToDictionary(param);
                    switch (mediaType)
                    {
                        case MediaTypeEnum.UrlQuery:
                            var paramUrl = QueryHelpers.AddQueryString(@url, dict);
                            request.RequestUri = new Uri(paramUrl);
                            break;
                        case MediaTypeEnum.ApplicationFormUrlencoded:
                            request.Content = new FormUrlEncodedContent(dict);
                            break;
                    }

                }
                else if (HttpMethod.Post.Equals(httpMethod))
                {
                    var dict = _mapUtil.DynamicToDictionary(param);
                    switch (mediaType)
                    {
                        case MediaTypeEnum.ApplicationFormUrlencoded:
                            request.Content = new FormUrlEncodedContent(dict);
                            break;
                        case MediaTypeEnum.ApplicationJson:
                            var jsonParam = JsonConvert.SerializeObject(param);
                            request.Content = new StringContent(jsonParam, Encoding.UTF8, "application/json");
                            break;
                        case MediaTypeEnum.MultipartFormData:
                            var content = new MultipartFormDataContent();
                            foreach (var item in dict)
                            {
                                content.Add(new StringContent(item.Value), item.Key);
                            }
                            request.Content = content;
                            break;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                if (null != cookieList && cookieList.Any())
                {
                    request.Headers.Add("Set-Cookie", cookieList);
                }
                else if (isParamConvertCookies)
                {
                    List<string> list = _mapUtil.DynamicToCookieStrList(param);
                    request.Headers.Add("Set-Cookie", list);
                }

                if (!string.IsNullOrEmpty(userAgent) && !string.IsNullOrWhiteSpace(userAgent))
                {
                    request.Headers.UserAgent.ParseAdd(userAgent);
                }
                using (var client = _clientFactory.CreateClient())
                {
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var cookieFlag = response.Headers.TryGetValues("Set-Cookie", out var setCookies);
                        if (cookieFlag)
                        {
                            return new HttpResult(result, setCookies.ToList(), response.StatusCode, true);
                        }
                        else
                        {
                            return new HttpResult(result, new List<string>(), response.StatusCode, true);
                        }
                    }
                    else
                    {
                        return new HttpResult(response.StatusCode.ToString(), new List<string>(), response.StatusCode, false);
                    }
                }
            }
        }

        public async Task<HttpResult> SendAsync(dynamic param, string url, string httpMethodStr, MediaTypeEnum mediaType, bool isParamConvertCookies, List<string> cookieList = null, string userAgent = null) 
        {
            var httpMethod = new HttpMethod(httpMethodStr.ToUpper());
            using (var request = new HttpRequestMessage(httpMethod, @url))
            {
                if ((HttpMethod.Get.Equals(httpMethod)))
                {
                    var dict = _mapUtil.DynamicToDictionary(param);
                    switch (mediaType)
                    {
                        case MediaTypeEnum.UrlQuery:
                            var paramUrl = QueryHelpers.AddQueryString(@url, dict);
                            request.RequestUri = new Uri(paramUrl);
                            break;
                        case MediaTypeEnum.ApplicationFormUrlencoded:
                            request.Content = new FormUrlEncodedContent(dict);
                            break;
                    }

                }
                else if (HttpMethod.Post.Equals(httpMethod))
                {
                    var dict = _mapUtil.DynamicToDictionary(param);
                    switch (mediaType)
                    {
                        case MediaTypeEnum.ApplicationFormUrlencoded:
                            request.Content = new FormUrlEncodedContent(dict);
                            break;
                        case MediaTypeEnum.ApplicationJson:
                            var jsonParam = JsonConvert.SerializeObject(param);
                            request.Content = new StringContent(jsonParam, Encoding.UTF8, "application/json");
                            break;
                        case MediaTypeEnum.MultipartFormData:
                            var content = new MultipartFormDataContent();
                            foreach (var item in dict)
                            {
                                content.Add(new StringContent(item.Value), item.Key);
                            }
                            request.Content = content;
                            break;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                if (null != cookieList && cookieList.Any())
                {
                    request.Headers.Add("Set-Cookie", cookieList);
                }
                else if (isParamConvertCookies)
                {
                    List<string> list = _mapUtil.DynamicToCookieStrList(param);
                    request.Headers.Add("Set-Cookie", list);
                }
                if (!string.IsNullOrEmpty(userAgent) && !string.IsNullOrWhiteSpace(userAgent))
                {
                    request.Headers.UserAgent.ParseAdd(userAgent);
                }
                using (var client = _clientFactory.CreateClient())
                {
                    using (var response = await client.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            var cookieFlag = response.Headers.TryGetValues("Set-Cookie", out var setCookies);
                            if (cookieFlag)
                            {
                                return new HttpResult(result, setCookies.ToList(), response.StatusCode, true);
                            }
                            else
                            {
                                return new HttpResult(result, new List<string>(), response.StatusCode, true);
                            }
                        }
                        else
                        {
                            return new HttpResult(response.StatusCode.ToString(), new List<string>(), response.StatusCode, false);
                        }
                    }
                }
            }
        }

        public async Task<HttpResult> SendAsync<T>(T param, string url, string httpMethodStr, MediaTypeEnum mediaType, bool isParamConvertCookies, List<string> cookieList = null, string userAgent = null) where T : class 
        {
            var httpMethod = new HttpMethod(httpMethodStr.ToUpper());
            using (var request = new HttpRequestMessage(httpMethod, @url))
            {
                if ((HttpMethod.Get.Equals(httpMethod)))
                {
                    var dict = _mapUtil.EntityToDictionary(param);
                    switch (mediaType)
                    {
                        case MediaTypeEnum.UrlQuery:
                            var paramUrl = QueryHelpers.AddQueryString(@url, dict);
                            request.RequestUri = new Uri(paramUrl);
                            break;
                        case MediaTypeEnum.ApplicationFormUrlencoded:
                            request.Content = new FormUrlEncodedContent(dict);
                            break;
                    }

                }
                else if (HttpMethod.Post.Equals(httpMethod))
                {
                    var dict = _mapUtil.EntityToDictionary(param);
                    switch (mediaType)
                    {
                        case MediaTypeEnum.ApplicationFormUrlencoded:
                            request.Content = new FormUrlEncodedContent(dict);
                            break;
                        case MediaTypeEnum.ApplicationJson:
                            var jsonParam = JsonConvert.SerializeObject(param);
                            request.Content = new StringContent(jsonParam, Encoding.UTF8, "application/json");
                            break;
                        case MediaTypeEnum.MultipartFormData:
                            var content = new MultipartFormDataContent();
                            foreach (var item in dict)
                            {
                                content.Add(new StringContent(item.Value), item.Key);
                            }
                            request.Content = content;
                            break;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                if (null != cookieList && cookieList.Any())
                {
                    request.Headers.Add("Set-Cookie", cookieList);
                }
                else if (isParamConvertCookies)
                {
                    List<string> list = _mapUtil.EntityToCookieStrList(param);
                    request.Headers.Add("Set-Cookie", list);
                }
                if (!string.IsNullOrEmpty(userAgent) && !string.IsNullOrWhiteSpace(userAgent))
                {
                    request.Headers.UserAgent.ParseAdd(userAgent);
                }
                using (var client = _clientFactory.CreateClient())
                {
                    using (var response = await client.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var cookieFlag = response.Headers.TryGetValues("Set-Cookie", out var setCookies);
                            var result = await response.Content.ReadAsStringAsync();

                            if (cookieFlag)
                            {
                                return new HttpResult(result, setCookies.ToList(), response.StatusCode, true);
                            }
                            else
                            {
                                return new HttpResult(result, new List<string>(), response.StatusCode, true);
                            }
                        }
                        else
                        {
                            return new HttpResult(response.StatusCode.ToString(), new List<string>(), response.StatusCode, false);
                        }
                    }
                }
            }

        }

        public async Task<HttpStreamResult> GetStreamAsync(dynamic param, string url, string httpMethodStr, MediaTypeEnum mediaType, string userAgent = null)
        {
            httpMethodStr = httpMethodStr.ToUpper();
            var httpMethod = new HttpMethod(httpMethodStr);
            var paramDict = _mapUtil.DynamicToDictionary(param);
            using (var request = new HttpRequestMessage(httpMethod, @url))
            {
                if (HttpMethod.Get.Equals(httpMethod))
                {
                    switch (mediaType)
                    {
                        case MediaTypeEnum.UrlQuery:
                            url = QueryHelpers.AddQueryString(url, paramDict);
                            request.RequestUri = new Uri(url);
                            break;
                        case MediaTypeEnum.ApplicationFormUrlencoded:
                            request.Content = new FormUrlEncodedContent(paramDict);
                            break;
                    }
                }
                else if (HttpMethod.Post.Equals(httpMethod))
                {
                    switch (mediaType)
                    {
                        case MediaTypeEnum.ApplicationFormUrlencoded:
                            request.Content = new FormUrlEncodedContent(paramDict);
                            break;
                        case MediaTypeEnum.ApplicationJson:
                            var jsonParam = JsonConvert.SerializeObject(param);
                            request.Content = new StringContent(jsonParam, Encoding.UTF8, "application/json");
                            break;
                        case MediaTypeEnum.MultipartFormData:
                            var content = new MultipartFormDataContent();
                            foreach (var item in paramDict)
                            {
                                content.Add(new StringContent(item.Value), item.Key);
                            }
                            request.Content = content;
                            break;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                if (!string.IsNullOrEmpty(userAgent) && !string.IsNullOrWhiteSpace(userAgent))
                {
                    request.Headers.UserAgent.ParseAdd(userAgent);
                }
                using (var client = _clientFactory.CreateClient())
                {
                    using (var response = await client.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var memoryStream = new MemoryStream();
                            await response.Content.CopyToAsync(memoryStream);
                            if (memoryStream.Length > 0)
                            {
                                return new HttpStreamResult(memoryStream, response.StatusCode, true);
                            }
                            else
                            {
                                return new HttpStreamResult(null, response.StatusCode, false);
                            }
                        }
                        else
                        {
                            return new HttpStreamResult(null, response.StatusCode, false);
                        }
                    }

                }
            }
        }

        public async Task<HttpStreamResult> GetStreamAsync<T>(T param, string url, string httpMethodStr, MediaTypeEnum mediaType, string userAgent = null) where T : class 
        {
            httpMethodStr = httpMethodStr.ToUpper();
            var httpMethod = new HttpMethod(httpMethodStr);
            var paramDict = _mapUtil.EntityToDictionary(param);
            using (var request = new HttpRequestMessage(httpMethod, url))
            {
                if (HttpMethod.Get.Equals(httpMethod))
                {
                    switch (mediaType)
                    {
                        case MediaTypeEnum.UrlQuery:
                            url = QueryHelpers.AddQueryString(url, paramDict);
                            request.RequestUri = new Uri(url);
                            break;
                        case MediaTypeEnum.ApplicationFormUrlencoded:
                            request.Content = new FormUrlEncodedContent(paramDict);
                            break;
                    }
                }
                else if (HttpMethod.Post.Equals(httpMethod))
                {
                    switch (mediaType)
                    {
                        case MediaTypeEnum.ApplicationFormUrlencoded:
                            request.Content = new FormUrlEncodedContent(paramDict);
                            break;
                        case MediaTypeEnum.ApplicationJson:
                            var jsonParam = JsonConvert.SerializeObject(param);
                            request.Content = new StringContent(jsonParam, Encoding.UTF8, "application/json");
                            break;
                        case MediaTypeEnum.MultipartFormData:
                            var content = new MultipartFormDataContent();
                            foreach (var item in paramDict)
                            {
                                content.Add(new StringContent(item.Value), item.Key);
                            }
                            request.Content = content;
                            break;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                if (!string.IsNullOrEmpty(userAgent) && !string.IsNullOrWhiteSpace(userAgent))
                {
                    request.Headers.UserAgent.ParseAdd(userAgent);
                }
                using (var client = _clientFactory.CreateClient())
                {
                    using (var response = await client.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var memoryStream = new MemoryStream();
                            await response.Content.CopyToAsync(memoryStream);
                            if (memoryStream.Length > 0)
                            {
                                return new HttpStreamResult(memoryStream, response.StatusCode, true);
                            }
                            else
                            {
                                return new HttpStreamResult(null, response.StatusCode, false);
                            }
                        }
                        else
                        {
                            return new HttpStreamResult(null, response.StatusCode, false);
                        }
                    }
                }
            }

        }
    }
}
