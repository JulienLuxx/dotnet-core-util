using Common.CoreUtil;
using Common.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.WeComCore
{
    public class WeComCoreSvc : BaseHttpSvc, IWeComCoreSvc
    {
        private readonly IEncryptUtil _encryptUtil;
        public WeComCoreSvc(IEncryptUtil encryptUtil, IHttpClientUtil httpClientUtil) : base(httpClientUtil) 
        {
            _encryptUtil = encryptUtil;
        }

        public IWeComResultDto CalculatorSignature(CalculateSignatureParam param)
        {
            if (string.IsNullOrEmpty(param.Url) || string.IsNullOrWhiteSpace(param.Url))
            {
                return new WeComBaseResultDto(-99, "NullUrl");
            }
            var resultDto = new SignatureResultDto
            {
                ErrCode = 0,
                ErrMsg = string.Empty,
                Signature = new SignatureDto()
                {
                    AgentId = param.AgentId,
                    CorpId = param.CorpId,
                    NonceStr = Guid.NewGuid().ToString(),
                    Ticket = param.Ticket,
                    Timestamp = _encryptUtil.GetNowTimestamp()
                }
            };
            var url = param.Url.IndexOf('#') > 0 ? param.Url.Remove(param.Url.IndexOf('#')) : param.Url;
            var JmData = "jsapi_ticket=" + resultDto.Signature.Ticket + "&noncestr=" + resultDto.Signature.NonceStr + "&timestamp=" + resultDto.Signature.Timestamp + "&url=" + url;
            resultDto.Signature.Signature = _encryptUtil.GetSHA1(JmData, Encoding.ASCII);
            return resultDto;
        }

        public virtual async Task<IWeComResultDto> GetAccessTokenAsync(AccessTokenParam param, string url, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url))
            {
                return new WeComBaseResultDto(-99, "NullUrl");
            }
            var result = await _httpUtil.SendAsync(param, MediaTypeEnum.UrlQuery, url, "get", cancellationToken: cancellationToken);
            if (result.IsSuccess)
            {
                var dto = JsonConvert.DeserializeObject<AccessTokenResultDto>(result.Result);
                return dto;
            }
            else
            {
                return new WeComBaseResultDto(result.ResultCode.GetHashCode(), "Internet Error");
            }
        }

        public async Task<IWeComResultDto> GetAppJsApiTicketAsync(JsApiTicketAppParam param, string url, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url))
            {
                return new WeComBaseResultDto(-99, "NullUrl");
            }
            var result = await _httpUtil.SendAsync(param, MediaTypeEnum.UrlQuery, url, "get", cancellationToken: cancellationToken);
            if (result.IsSuccess)
            {
                var dto = JsonConvert.DeserializeObject<JsApiTicketResultDto>(result.Result);
                return dto;
            }
            else
            {
                return new WeComBaseResultDto(result.ResultCode.GetHashCode(), "Internet Error");
            }
        }

        public async Task<IWeComResultDto> GetEnterpriseJsApiTicketAsync(JsApiTicketEnterpriseParam param, string url, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url))
            {
                return new WeComBaseResultDto(-99, "NullUrl");
            }
            var result = await _httpUtil.SendAsync(param, MediaTypeEnum.UrlQuery, url, "get", cancellationToken: cancellationToken);
            if (result.IsSuccess)
            {
                var dto = JsonConvert.DeserializeObject<JsApiTicketResultDto>(result.Result);
                return dto;
            }
            else
            {
                return new WeComBaseResultDto(result.ResultCode.GetHashCode(), "Internet Error");
            }
        }

        public virtual async Task<IWeComResultDto> GetUserIdAsync(GetUserIdParam param, string url, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url))
            {
                return new WeComBaseResultDto(-99, "NullUrl");
            }
            var result = await _httpUtil.SendAsync(param, MediaTypeEnum.UrlQuery, url, "get", cancellationToken: cancellationToken);
            if (result.IsSuccess)
            {
                var baseDto = JsonConvert.DeserializeObject<WeComBaseResultDto>(result.Result);
                if (baseDto.ErrCode == 0)
                {
                    var dto = JsonConvert.DeserializeObject<UserInfoResultDto>(result.Result);
                    return dto;
                }
                return baseDto;
            }
            else
            {
                return new WeComBaseResultDto(result.ResultCode.GetHashCode(), "Internet Error");
            }
        }

        public virtual async Task<IWeComResultDto> PushMessageAsync(CardMessageParam param, string accessToken, string url, CancellationToken cancellationToken = default) 
        {
            if (null == param) 
            {
                return new WeComBaseResultDto(-99, "NullParam");
            }
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrWhiteSpace(accessToken)) 
            {
                return new WeComBaseResultDto(-99, "NullAccessToken");
            }
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url))
            {
                return new WeComBaseResultDto(-99, "NullUrl");
            }
            //url = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + accessToken + "";//微信推送接口地址
            url = url + accessToken;

            var result = await _httpUtil.SendAsync(param, MediaTypeEnum.ApplicationJson, url, "post", cancellationToken: cancellationToken);
            if (result.IsSuccess)
            {
                var baseDto = JsonConvert.DeserializeObject<WeComBaseResultDto>(result.Result);
                if (baseDto.ErrCode == 0)
                {
                    var dto = JsonConvert.DeserializeObject<PushMessageResultDto>(result.Result);
                    return dto;
                }
                return baseDto;
            }
            else
            {
                return new WeComBaseResultDto(result.ResultCode.GetHashCode(), "网络异常");
            }
        }

        public virtual async Task<IWeComResultDto> UploadMediaAsync(UploadMediaParam param, Stream stream, string fileName, string url, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url))
            {
                return new WeComBaseResultDto(-99, "NullUrl");
            }
            var result = await _httpUtil.PostFileAsync(stream, fileName, param, url, "media", cancellationToken);
            if (result.IsSuccess)
            {
                var baseDto = JsonConvert.DeserializeObject<WeComBaseResultDto>(result.Result);
                if (baseDto.ErrCode == 0)
                {
                    var dto = JsonConvert.DeserializeObject<UploadMediaResultDto>(result.Result);
                    return dto;
                }
                return baseDto;
            }
            else
            {
                return new WeComBaseResultDto(result.ResultCode.GetHashCode(), "Internet Error");
            }
        }
    }
}
