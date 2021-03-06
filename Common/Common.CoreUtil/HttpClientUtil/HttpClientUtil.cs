﻿using Common.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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

        public async Task<IHttpResult> GetAsync<T>(T param,string url,MediaTypeEnum mediaType)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpResult<string>> PostFileAsync<T>(Stream stream, string fileName, T param, string url, string contentName = "files", CancellationToken cancellationToken = default, Encoding encoding = null) 
        {
            var boundary= string.Format("--{0}", DateTime.Now.Ticks.ToString("x"));
            var content = new MultipartFormDataContent(boundary);
            content.Add(new StreamContent(stream, (int)stream.Length), contentName, fileName);

            if (null != param)
            {
                var paramDict = _mapUtil.DynamicToDictionary(param);
                foreach (var item in paramDict)
                {
                    content.Add(new StringContent(item.Value), item.Key);
                }
            }

            using (var client=_clientFactory.CreateClient())
            {
                using (var response = await client.PostAsync(url, content, cancellationToken)) 
                {
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.Content.Headers.ContentLength.HasValue)
                        {
                            if (response.Content.Headers.ContentLength < 81920)
                            {
                                var result = await response.Content.ReadAsStringAsync();
                                return new HttpResult<string>(result, response.StatusCode, response.IsSuccessStatusCode);
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
                                    var byteArray = memoryStream.GetBuffer();
                                    var resultStr = encoding.GetString(byteArray);
                                    return new HttpResult<string>(resultStr, response.StatusCode, response.IsSuccessStatusCode);
                                }
                            }
                        }
                        else
                        {
                            return new HttpResult<string>(string.Empty, response.StatusCode, response.IsSuccessStatusCode);
                        }
                    }
                    else
                    {
                        return new HttpResult<string>(response.StatusCode);
                    }
                }
            }
        }

        public async Task<IHttpResult<string>> SendAsync<T>(T param, MediaTypeEnum mediaType, string url, string httpMethodStr, JsonConvertOptionEnum jsonConvertOption = JsonConvertOptionEnum.NewtonSoftJson, CancellationToken cancellationToken = default, Encoding encoding = null, string jwt = null, string[] cookiesArray = null, string userAgent = null) 
        {
            var httpMethod = new HttpMethod(httpMethodStr);
            using (var request = new HttpRequestMessage(httpMethod, @url))
            {
                if (null != param)
                {
                    var paramDict = _mapUtil.DynamicToDictionary(param);
                    if (HttpMethod.Get.Equals(httpMethod))
                    {
                        switch (mediaType)
                        {
                            case MediaTypeEnum.UrlQuery:
                                var paramUrl = QueryHelpers.AddQueryString(@url, paramDict);
                                request.RequestUri = new Uri(paramUrl);
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
                                var jsonParam = JsonConvertOptionEnum.NewtonSoftJson == jsonConvertOption ? JsonConvert.SerializeObject(param) : System.Text.Json.JsonSerializer.Serialize(param);
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
                }


                if (null != cookiesArray && cookiesArray.Any())
                {
                    request.Headers.Add("Set-Cookie", cookiesArray);
                }
                else if (!string.IsNullOrEmpty(jwt) && !string.IsNullOrWhiteSpace(jwt))
                {
                    //var tokenString = string.Format("Bearer {0}", jwt);
                    //request.Headers.Add("Authorization", tokenString);
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                }

                if (!string.IsNullOrEmpty(userAgent) && !string.IsNullOrWhiteSpace(userAgent))
                {
                    request.Headers.UserAgent.ParseAdd(userAgent);
                }

                using (var client = _clientFactory.CreateClient())
                {
                    using (var response = await client.SendAsync(request, cancellationToken)) 
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var cookieFlag = response.Headers.TryGetValues("Set-Cookie", out var setCookies);
                            if (response.Content.Headers.ContentLength.HasValue)
                            {
                                if (response.Content.Headers.ContentLength < 81920)
                                {
                                    var result = await response.Content.ReadAsStringAsync();
                                    //return new HttpResult(result, (cookieFlag ? setCookies.ToList() : new List<string>()), true);
                                    return new HttpResult<string>(result, response.StatusCode, response.IsSuccessStatusCode, cookieFlag ? setCookies.ToArray() : null);
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
                                        //return new HttpResult(resultStr, (cookieFlag ? setCookies.ToList() : new List<string>()), true);
                                        return new HttpResult<string>(resultStr, response.StatusCode, response.IsSuccessStatusCode, cookieFlag ? setCookies.ToArray() : null);
                                    }
                                }
                            }
                            else
                            {
                                //return new HttpResult(string.Empty, (cookieFlag ? setCookies.ToList() : new List<string>()), true);
                                return new HttpResult<string>(string.Empty, response.StatusCode, response.IsSuccessStatusCode, cookieFlag ? setCookies.ToArray() : null);
                            }
                        }
                        else
                        {
                            //return new HttpResult(response.StatusCode.ToString(), new List<string>(), response.StatusCode, false);
                            return new HttpResult<string>(response.StatusCode);
                        }
                    }
                }
            }
        }

        public async Task<HttpResultDto> SendAsync(dynamic param, string url, HttpMethod httpMethod, MediaTypeEnum mediaType, List<string> cookieList = null, string userAgent = null)
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
                                return new HttpResultDto(result, setCookies.ToList(), response.StatusCode, true);
                            }
                            else
                            {
                                return new HttpResultDto(result, new List<string>(), response.StatusCode, true);
                            }
                        }
                        else
                        {
                            return new HttpResultDto(response.StatusCode.ToString(), new List<string>(), response.StatusCode, false);
                        }
                    }

                }
            }
        }

        public async Task<HttpResultDto> SendAsync(dynamic param, string url, string httpMethodStr, MediaTypeEnum mediaType, List<string> cookieList = null, string userAgent = null)
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
                                return new HttpResultDto(result, setCookies.ToList(), response.StatusCode, true);
                            }
                            else
                            {
                                return new HttpResultDto(result, new List<string>(), response.StatusCode, true);
                            }
                        }
                        else
                        {
                            return new HttpResultDto(response.StatusCode.ToString(), new List<string>(), response.StatusCode, false);
                        }
                    }
                }
            }
        }

        public async Task<HttpResultDto> SendAsync<T>(T param, string url, string httpMethodStr, MediaTypeEnum mediaType, Encoding encoding = null, List<string> cookieList = null, string userAgent = null) where T : class 
        {
            var httpMethod = new HttpMethod(httpMethodStr.ToUpper());
            using(var request = new HttpRequestMessage(httpMethod, @url))
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
                                    return new HttpResultDto(result, (cookieFlag ? setCookies.ToList() : new List<string>()), true);
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
                                        return new HttpResultDto(resultStr, (cookieFlag ? setCookies.ToList() : new List<string>()), true);
                                    }
                                }
                            }
                            else
                            {
                                return new HttpResultDto(string.Empty, (cookieFlag ? setCookies.ToList() : new List<string>()), true);
                            }

                        }
                        else
                        {
                            return new HttpResultDto(response.StatusCode.ToString(), new List<string>(), response.StatusCode, false);
                        }
                    }
                }
            }

        }

        public async Task<HttpResultDto> SendAsync(dynamic param, string url, HttpMethod httpMethod, MediaTypeEnum mediaType, bool isParamConvertCookies, List<string> cookieList = null, string userAgent = null) 
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
                            return new HttpResultDto(result, setCookies.ToList(), response.StatusCode, true);
                        }
                        else
                        {
                            return new HttpResultDto(result, new List<string>(), response.StatusCode, true);
                        }
                    }
                    else
                    {
                        return new HttpResultDto(response.StatusCode.ToString(), new List<string>(), response.StatusCode, false);
                    }
                }
            }
        }

        public async Task<HttpResultDto> SendAsync(dynamic param, string url, string httpMethodStr, MediaTypeEnum mediaType, bool isParamConvertCookies, List<string> cookieList = null, string userAgent = null) 
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
                                return new HttpResultDto(result, setCookies.ToList(), response.StatusCode, true);
                            }
                            else
                            {
                                return new HttpResultDto(result, new List<string>(), response.StatusCode, true);
                            }
                        }
                        else
                        {
                            return new HttpResultDto(response.StatusCode.ToString(), new List<string>(), response.StatusCode, false);
                        }
                    }
                }
            }
        }

        public async Task<HttpResultDto> SendAsync<T>(T param, string url, string httpMethodStr, MediaTypeEnum mediaType, bool isParamConvertCookies, List<string> cookieList = null, string userAgent = null) where T : class 
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
                                return new HttpResultDto(result, setCookies.ToList(), response.StatusCode, true);
                            }
                            else
                            {
                                return new HttpResultDto(result, new List<string>(), response.StatusCode, true);
                            }
                        }
                        else
                        {
                            return new HttpResultDto(response.StatusCode.ToString(), new List<string>(), response.StatusCode, false);
                        }
                    }
                }
            }

        }

        public async Task<HttpStreamResultDto> GetStreamAsync(dynamic param, string url, string httpMethodStr, MediaTypeEnum mediaType, CancellationToken cancellationToken = default, string userAgent = null) 
        {
            httpMethodStr = httpMethodStr.ToUpper();
            var httpMethod = new HttpMethod(httpMethodStr);

            using (var request = new HttpRequestMessage(httpMethod, @url))
            {
                if (null != param)
                {
                    var paramDict = _mapUtil.DynamicToDictionary(param);
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
                }

                if (!string.IsNullOrEmpty(userAgent) && !string.IsNullOrWhiteSpace(userAgent))
                {
                    request.Headers.UserAgent.ParseAdd(userAgent);
                }
                using (var client = _clientFactory.CreateClient())
                {
                    using (var response = await client.SendAsync(request, cancellationToken)) 
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var memoryStream = new MemoryStream();
                            await response.Content.CopyToAsync(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            if (memoryStream.Length > 0)
                            {
                                return new HttpStreamResultDto(memoryStream, response.StatusCode, true);
                            }
                            else
                            {
                                return new HttpStreamResultDto(null, response.StatusCode, false);
                            }
                        }
                        else
                        {
                            return new HttpStreamResultDto(null, response.StatusCode, false);
                        }
                    }

                }
            }
        }

        public async Task<HttpStreamResultDto> GetStreamAsync<T>(T param, string url, string httpMethodStr, MediaTypeEnum mediaType, CancellationToken cancellationToken = default, string userAgent = null) where T : class 
        {
            httpMethodStr = httpMethodStr.ToUpper();
            var httpMethod = new HttpMethod(httpMethodStr);

            var memoryStream = new MemoryStream();
            using (var request = new HttpRequestMessage(httpMethod, url))
            {
                if (null!=param)
                {
                    var paramDict = _mapUtil.DynamicToDictionary(param);
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
                }

                if (!string.IsNullOrEmpty(userAgent) && !string.IsNullOrWhiteSpace(userAgent))
                {
                    request.Headers.UserAgent.ParseAdd(userAgent);
                }
                using (var client = _clientFactory.CreateClient())
                {
                    using (var response = await client.SendAsync(request, cancellationToken)) 
                    {
                        if (response.IsSuccessStatusCode)
                        {                            
                            await response.Content.CopyToAsync(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            if (memoryStream.Length > 0)
                            {                                
                                return new HttpStreamResultDto(memoryStream, response.StatusCode, true);
                            }
                            else
                            {
                                memoryStream.Dispose();
                                return new HttpStreamResultDto(null, response.StatusCode, false);
                            }
                        }
                        else
                        {
                            memoryStream.Dispose();
                            return new HttpStreamResultDto(null, response.StatusCode, false);
                        }
                    }
                }
            }

        }
    }
}
