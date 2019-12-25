using Common.CoreUtil;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using System.IO;

namespace Common.XUnitTest
{
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

        public async Task<HttpResult> SendAsyncGeneric()
        {
            string json = @GetJson();
            var httpResult = await _httpClientUtil.SendAsync(new { c = "Writes", t = string.Empty, d = json }, @"http://192.168.118.27:8000/Log", "POST", MediaTypeEnum.ApplicationFormUrlencoded);
            return httpResult;
        }

        [Fact]
        public async Task SendAsyncGenericTest()
        {
            var result = await SendAsyncGeneric();
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
