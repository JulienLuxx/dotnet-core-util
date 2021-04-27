using Microsoft.Extensions.DependencyInjection;
using Common.Util;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Common.CoreUtil;

namespace Common.XUnitTest
{
    public class EncryptUtilUnitTest: BaseUnitTest
    {
        private IEncryptUtil _encryptUtil { get; set; }

        public EncryptUtilUnitTest() : base()
        {
            _serviceCollection.AddEncryptUtil();
            BuilderServiceProvider();
            _encryptUtil = _serviceProvider.GetService<IEncryptUtil>();
        }

        [Fact]
        public void GetMd5By32Test()
        {
            var value = _encryptUtil.GetMd5By32(@"123456{1#2$3%4(5)6@7!poeeww$3%4(5)djjkkldss}").ToLower();
            Assert.True(!string.IsNullOrEmpty(value));
        }

        [Fact]
        public void CreateRandomCode()
        {
            var result = _encryptUtil.CreateRandomCode(6, false);
            Assert.Equal(6, result.Length);
        }

        [Fact]
        public void GetLongByGuidTest()
        {
            var flag= _encryptUtil.GetLongByGuid(out var num);
            Assert.True(flag);
        }

        [Fact]
        public void GetLongByGuidTest2()
        {
            var flag = _encryptUtil.GetLongByGuid(Guid.NewGuid(), out var num);
            Assert.True(flag);
        }

        [Fact]
        public void GetRuntimePath()
        {
            var path = FileHelper.GetLinuxPath("D:\\WebSite\\MobileServerFri /upload /20210423 /Common.WeComCore.0.0.3.nupkg");
            Assert.True(false);
        }
    }
}
