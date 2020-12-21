using Common.CoreUtil;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using System.IO;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Reflection;

namespace Common.XUnitTest
{
    public class OrganParam
    {
        //[JsonPropertyName("organ_code")]
        [JsonProperty("data.organ_code")]
        [Description("data.organ_code")]
        public string OrganCode { get; set; }

        //[JsonPropertyName("organ_name")]
        [JsonProperty("data.organ_name")]
        [Description("data.organ_name")]
        public string OrganName { get; set; }

        //[JsonPropertyName("short_name")]
        [JsonProperty("data.short_name")]
        [Description("data.short_name")]
        public string ShortName { get; set; }

        //[JsonPropertyName("parent_organ_code")]
        [JsonProperty("data.parent_organ_code")]
        [Description("data.parent_organ_code")]
        public string ParentOrganCode { get; set; }

        //public OrganParam(OrganArg arg, IMapper mapper)
        //{
        //    mapper.Map(arg, this);
        //}

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class UserCenterSendParam
    {

        [Description("data.organ_code")]
        public string OrganCode { get; set; }

        [Description("data.organ_name")]
        public string OrganName { get; set; }

        [Description("data.short_name")]
        public string ShortName { get; set; }

        [Description("data.parent_organ_code")]
        public string ParentOrganCode { get; set; }

        [Description("appCode")]
        public string AppCode { get; set; }

        [Description("appToken")]
        public string AppToken { get; set; }
    }

    public class BasePageQueryModel
    {
        private int _pageIndex = 0;
        //[Description("pageIndex")]
        public int PageIndex
        {
            get => _pageIndex <= 0 ? 1 : _pageIndex;
            set => _pageIndex = value;
        }

        private int _pageSize = 0;
        //[Description("pageSize")]
        public int PageSize
        {
            get => _pageSize <= 0 ? 20 : _pageSize;
            set => _pageSize = value;
        }

        public int TotalCount { get; set; }


        private string _orderByColumn = string.Empty;
        public string OrderByColumn
        {
            get => string.IsNullOrEmpty(_orderByColumn) || string.IsNullOrWhiteSpace(_orderByColumn) ? string.Empty : _orderByColumn.Trim();
            set => _orderByColumn = value;
        }

        public bool IsDesc { get; set; }
    }

    public class TestDto
    {
        public string Name { get; set; }

        public string EditerName { get; set; }

        public int Status { get; set; }
    }

    public class WeComAccessTokenParam
    {
        [Description("corpid")]
        public string CorpId { get; set; }

        [Description("corpsecret")]
        public string CorpSecret { get; set; }
    }

    public class HttpUtilUnitTest : BaseUnitTest
    {
        private IHttpClientUtil _httpClientUtil { get; set; }

        public HttpUtilUnitTest() : base()
        {
            _serviceCollection.AddHttpClientUtil();
            BuilderServiceProvider();
            _httpClientUtil = _serviceProvider.GetService<IHttpClientUtil>();
        }

        public dynamic GetJson()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return File.ReadAllText("C:/Users/Julien/Desktop/esjson.txt", Encoding.GetEncoding("GB2312"));
        }

        public dynamic GetJson2()
        {
            return File.ReadAllText("C:/Users/Julien/Desktop/t.txt");
        }

        public async Task<HttpResultDto> SendAsyncGeneric()
        {
            string json = @GetJson();
            var httpResult = await _httpClientUtil.SendAsync(new { c = "Writes", t = string.Empty, d = json }, @"http://192.168.118.27:8000/Log", "POST", MediaTypeEnum.ApplicationFormUrlencoded);
            return httpResult;
        }

        public async Task<HttpResultDto> SendAsync2()
        {
            var json = GetJson2();
            var httpResult = await _httpClientUtil.SendAsync(new { d = json }, @"http://localhost:5000/DAQManage?c=UpdateUserCenterData", "POST", MediaTypeEnum.ApplicationFormUrlencoded);
            return httpResult;
        }

        public async Task<HttpResultDto> SendAsync3()
        {
            var param = new UserCenterSendParam()
            {
                OrganCode = "636",
                OrganName = "Test",
                ParentOrganCode = "456",
                AppCode = "233",
                AppToken = "2333"
            };
            var httpResult = await _httpClientUtil.SendAsync(param, @"http://localhost:5000/API/Organ/AddOrgan", "POST", MediaTypeEnum.ApplicationFormUrlencoded);
            return httpResult;
        }

        [Fact]
        public async Task SendAsyncGenericTest()
        {
            var result = await SendAsync3();
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetStreamAsync()
        {
            var httpResult = await _httpClientUtil.GetStreamAsync(new {q="release" }, @"https://cn.bing.com/dict/search", "GET", MediaTypeEnum.UrlQuery);
            var fileStream = new FileStream(@"D:\doc\233.txt", FileMode.Create);
            await httpResult.Stream.CopyToAsync(fileStream);
            httpResult.Stream.Dispose();
            fileStream.Close();
            Assert.True(true);
        }

        public async Task<HttpResult<string>> GetAsync()
        {
            var param = new BasePageQueryModel()
            {
                PageSize=500,
                OrderByColumn="Id",
                IsDesc=true
            };
            var httpResult = await _httpClientUtil.SendAsync(param, MediaTypeEnum.UrlQuery, "http://localhost:5010/Log/Page", "get", JsonConvertOptionEnum.SystemJson) as HttpResult<string>;
            return httpResult;
        }

        public async Task<HttpResult<string>> PostAsync()
        {
            var param = new TestDto()
            {
                Name="UtilTest",
                EditerName="Util",
                Status=0
            };
            var httpResult = await _httpClientUtil.SendAsync(param, MediaTypeEnum.ApplicationJson, "http://localhost:5010/ArticleType/Add", "post", JsonConvertOptionEnum.SystemJson) as HttpResult<string>;
            return httpResult;
        }

        [Fact]
        public async Task SendAsyncTest()
        {
            var result = await PostAsync();
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task WeComSendTest()
        {
            var param = new WeComAccessTokenParam()
            {
                CorpId = "ww906ab183effcd12b",
                CorpSecret = "iCoggU4TtZsme87i4x6vuryxK2yeBrHmCef9ZvbTwNA"
            };
            var result = await _httpClientUtil.SendAsync(param, MediaTypeEnum.UrlQuery, "https://qyapi.weixin.qq.com/cgi-bin/gettoken", "get") as HttpResult<string>;
            Assert.True(result.IsSuccess);
        }
    }
}
