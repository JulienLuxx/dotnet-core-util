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

        public async Task<HttpResult> SendAsyncGeneric()
        {
            string json = @GetJson();
            var httpResult = await _httpClientUtil.SendAsync(new { c = "Writes", t = string.Empty, d = json }, @"http://192.168.118.27:8000/Log", "POST", MediaTypeEnum.ApplicationFormUrlencoded);
            return httpResult;
        }

        public async Task<HttpResult> SendAsync2()
        {
            var json = GetJson2();
            var httpResult = await _httpClientUtil.SendAsync(new { d = json }, @"http://localhost:5000/DAQManage?c=UpdateUserCenterData", "POST", MediaTypeEnum.ApplicationFormUrlencoded);
            return httpResult;
        }

        public async Task<HttpResult> SendAsync3()
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
    }
}
