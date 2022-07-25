using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.BaiduAICore
{
    public interface IOcrUtil
    {
        Task<IHttpClientResult> GetAccessTokenAsync(string clientId, string clientSecret, string grantType = "client_credentials", string url = @"https://aip.baidubce.com/oauth/2.0/token", CancellationToken cancellationToken = default);

        Task<IHttpClientResult> PostHealthCodeAsync(string accessToken, byte[] imageBuffer, string url = @"https://aip.baidubce.com/rest/2.0/ocr/v1/health_code?access_token=", CancellationToken cancellationToken = default);

        Task<IHttpClientResult> PostPCRAsync(string accessToken, byte[] imageBuffer, string url = @"https://aip.baidubce.com/rest/2.0/ocr/v1/covid_test?access_token=", CancellationToken cancellationToken = default);

        Task<IHttpClientResult> PostTravelCardAsync(string accessToken, byte[] imageBuffer, string url = @"https://aip.baidubce.com/rest/2.0/ocr/v1/travel_card?access_token=", CancellationToken cancellationToken = default);        
    }
}
