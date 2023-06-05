using Common.ImageUtil;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp.QrCode;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Common.XUnitTest
{
    public class ImageUtilUnitTest : BaseUnitTest
    {
        private IQrCodeUtil _qrCodeUtil { get; set; }
        private ICaptchaUtil _captchaUtil { get; set; }

        public ImageUtilUnitTest() : base()
        {
            _serviceCollection.AddScoped<IQrCodeUtil, QrCodeUtil>();
            _serviceCollection.AddScoped<ICaptchaUtil, CaptchaUtil>();
            BuilderServiceProvider();
            _qrCodeUtil = _serviceProvider.GetRequiredService<IQrCodeUtil>();
            _captchaUtil = _serviceProvider.GetRequiredService<ICaptchaUtil>();
        }

        [Fact]
        public async Task GenerateEmbeddedIconQrCodeTest()
        {
            var iconPath = @"C:\doc\v2.jpg";
            var icon = await File.ReadAllBytesAsync(iconPath);
            var value = "value";
            var bytesArray = _qrCodeUtil.GenerateEmbeddedIconQrCode(value, SKColor.Parse("000000"), ECCLevel.M, icon);
            using (var fs = new FileStream(@"C:\doc\qrcode.png", FileMode.CreateNew))
            {
                await fs.WriteAsync(bytesArray);
                fs.Flush();
            }
            Assert.True(File.Exists(@"C:\doc\qrcode.png"));
        }

        [Fact]
        public async Task GenerateCaptchaTest()
        {
            var filePath = @"C:\doc\captcha.png";
            var bytesArray = _captchaUtil.GenerateCaptcha("2333", 100, 50, 30);
            using (var img = SKImage.FromEncodedData(bytesArray))
            using (var skData = img.Encode(SKEncodedImageFormat.Png, 100))
            using (var fs = new FileStream(filePath, FileMode.CreateNew))
            {
                skData.SaveTo(fs);
                await fs.FlushAsync();
            }
            Assert.True(File.Exists(filePath));
        }
    }
}
