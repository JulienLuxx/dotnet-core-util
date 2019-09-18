using Common.Util;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            var request = new HttpRequestMessage(httpMethod, @url);
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
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var cookieFlag = response.Headers.TryGetValues("Set-Cookie", out var setCookies);
                if (cookieFlag)
                {
                    return new HttpResult(result, setCookies.ToList(), true);
                }
                else
                {
                    return new HttpResult(result, new List<string>(), true);
                }
            }
            else
            {
                return new HttpResult(false);
            }
        }

        public async Task<HttpResult> SendAsync(dynamic param, string url, string httpMethodStr, MediaTypeEnum mediaType, List<string> cookieList = null, string userAgent = null)
        {
            var httpMethod = new HttpMethod(httpMethodStr.ToUpper());
            var request = new HttpRequestMessage(httpMethod, @url);
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
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var cookieFlag = response.Headers.TryGetValues("Set-Cookie", out var setCookies);
                if (cookieFlag)
                {
                    return new HttpResult(result, setCookies.ToList(), true);
                }
                else
                {
                    return new HttpResult(result, new List<string>(), true);
                }
            }
            else
            {
                return new HttpResult(false);
            }
        }

        public async Task<HttpResult> SendAsync(dynamic param, string url, HttpMethod httpMethod, MediaTypeEnum mediaType, List<string> cookieList = null, string userAgent = null, bool isParamConvertCookies = false)
        {
            var request = new HttpRequestMessage(httpMethod, @url);
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
                List<string> list = _mapUtil.DynamicToStringList(param);
                request.Headers.Add("Set-Cookie", list);
            }

            if (!string.IsNullOrEmpty(userAgent) && !string.IsNullOrWhiteSpace(userAgent))
            {
                request.Headers.UserAgent.ParseAdd(userAgent);
            }
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var cookieFlag = response.Headers.TryGetValues("Set-Cookie", out var setCookies);
                if (cookieFlag)
                {
                    return new HttpResult(result, setCookies.ToList(), true);
                }
                else
                {
                    return new HttpResult(result, new List<string>(), true);
                }
            }
            else
            {
                return new HttpResult(false);
            }
        }

        public async Task<HttpResult> SendAsync(dynamic param, string url, string httpMethodStr, MediaTypeEnum mediaType, List<string> cookieList = null, string userAgent = null, bool isParamConvertCookies = false) 
        {
            var httpMethod = new HttpMethod(httpMethodStr.ToUpper());
            var request = new HttpRequestMessage(httpMethod, @url);
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
                List<string> list = _mapUtil.DynamicToStringList(param);
                request.Headers.Add("Set-Cookie", list);
            }


            if (!string.IsNullOrEmpty(userAgent) && !string.IsNullOrWhiteSpace(userAgent))
            {
                request.Headers.UserAgent.ParseAdd(userAgent);
            }
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var cookieFlag = response.Headers.TryGetValues("Set-Cookie", out var setCookies);
                if (cookieFlag)
                {
                    return new HttpResult(result, setCookies.ToList(), true);
                }
                else
                {
                    return new HttpResult(result, new List<string>(), true);
                }
            }
            else
            {
                return new HttpResult(false);
            }
        }
    }
}
