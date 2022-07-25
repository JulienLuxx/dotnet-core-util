using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Common.BaiduAICore
{
    public class OcrUtil : IOcrUtil
    {
        public string _clientName { get; set; }

        private IHttpClientFactory _clientFactory { get; set; }

        public OcrUtil(IHttpClientFactory clientFactory, string clientName="BaiduOcr")
        {
            _clientFactory = clientFactory;
            _clientName = clientName;
        }

        public async Task<IHttpClientResult> GetAccessTokenAsync(string clientId, string clientSecret, string grantType = "client_credentials", string url = @"https://aip.baidubce.com/oauth/2.0/token", CancellationToken cancellationToken = default)
        {
            var paramDict = new Dictionary<string, string>();
            paramDict.Add("grant_type", grantType);
            paramDict.Add("client_id", clientId);
            paramDict.Add("client_secret", clientSecret);

            var content = new FormUrlEncodedContent(paramDict);
            using (var client = _clientFactory.CreateClient(_clientName))
            {
                using (var response = await client.PostAsync(url, content, cancellationToken))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.Content.Headers.ContentLength.HasValue)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            return new HttpClientResult<string>(result, response.IsSuccessStatusCode, response.StatusCode);
                        }
                    }
                    return new HttpClientResult<string>(string.Empty, response.IsSuccessStatusCode, response.StatusCode);
                }
            }
        }

        private async Task<IHttpClientResult> PostImageBase64Async(string accessToken, byte[] imageBuffer, string url, CancellationToken cancellationToken = default)
        {
            url = url + accessToken;
            var base64 = Convert.ToBase64String(imageBuffer);
            var imageBase64 = "image=" + HttpUtility.UrlEncode(base64);
            var imageBytes = Encoding.Default.GetBytes(imageBase64);
            var content = new ByteArrayContent(imageBytes, 0, imageBytes.Length);
            using (var client = _clientFactory.CreateClient(_clientName))
            {
                using (var response = await client.PostAsync(url, content, cancellationToken))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.Content.Headers.ContentLength.HasValue)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            return new HttpClientResult<string>(result, response.IsSuccessStatusCode, response.StatusCode);
                        }
                    }
                    return new HttpClientResult<string>(string.Empty, response.IsSuccessStatusCode, response.StatusCode);
                }
            }
        }

        public async Task<IHttpClientResult> PostHealthCodeAsync(string accessToken, byte[] imageBuffer, string url = @"https://aip.baidubce.com/rest/2.0/ocr/v1/health_code?access_token=", CancellationToken cancellationToken = default)
        {
            return await PostImageBase64Async(accessToken, imageBuffer, url, cancellationToken);
        }
        public async Task<IHttpClientResult> PostPCRAsync(string accessToken, byte[] imageBuffer, string url = @"https://aip.baidubce.com/rest/2.0/ocr/v1/covid_test?access_token=", CancellationToken cancellationToken = default)
        {
            return await PostImageBase64Async(accessToken, imageBuffer, url, cancellationToken);
        }

        public async Task<IHttpClientResult> PostTravelCardAsync(string accessToken, byte[] imageBuffer, string url = @"https://aip.baidubce.com/rest/2.0/ocr/v1/travel_card?access_token=", CancellationToken cancellationToken = default)
        {
            return await PostImageBase64Async(accessToken, imageBuffer, url, cancellationToken);
        }


    }
}
