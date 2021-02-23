using Common.CoreUtil;
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
        public WeComCoreSvc(IHttpClientUtil httpClientUtil) : base(httpClientUtil)
        {
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
    }
}
