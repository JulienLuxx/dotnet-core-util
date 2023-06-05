using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Common.ImageUtil
{
    public class CaptchaUtil : ICaptchaUtil
    {
        public byte[] GenerateCaptcha(string code, int width, int height, int textSize)
        {
            var random = new Random();
            //验证码颜色集合
            var colors = new[] { SKColors.Black, SKColors.Red, SKColors.DarkBlue, SKColors.Green, SKColors.Orange, SKColors.Brown, SKColors.DarkCyan, SKColors.Purple };
            var backcolors = new[] { SKColors.AntiqueWhite, SKColors.WhiteSmoke, SKColors.FloralWhite };
            //验证码字体集合
            var fonts = new[] { "Cantarell" };
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fonts = new[] { "宋体" };
            }
            //相当于js的 canvas.getContext('2d')
            using (var image2d = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul))
            //相当于前端的canvas
            using (var canvas = new SKCanvas(image2d))
            {
                //填充白色背景
                canvas.DrawColor(backcolors[random.Next(0, backcolors.Length - 1)]);
                //样式 跟xaml差不多
                using (var drawStyle = new SKPaint())
                {
                    //填充验证码到图片
                    for (int i = 0; i < code.Length; i++)
                    {
                        drawStyle.IsAntialias = true;
                        drawStyle.TextSize = textSize;
                        var font = SKTypeface.FromFamilyName(fonts[random.Next(0, fonts.Length - 1)], SKFontStyleWeight.SemiBold, SKFontStyleWidth.ExtraCondensed, SKFontStyleSlant.Upright);
                        drawStyle.Typeface = font;
                        drawStyle.Color = colors[random.Next(0, colors.Length - 1)];
                        //写字
                        canvas.DrawText(code[i].ToString(), (i + 1) * 16, 28, drawStyle);
                    }
                    //生成20条干扰线
                    for (int i = 0; i < 20; i++)
                    {
                        drawStyle.Color = colors[random.Next(colors.Length)];
                        drawStyle.StrokeWidth = 1;
                        canvas.DrawLine(random.Next(0, code.Length * 20), random.Next(0, 80), random.Next(0, code.Length * 21), random.Next(0, 40), drawStyle);
                    }
                    using (var image = SKImage.FromBitmap(image2d))
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        return data.ToArray();
                    }
                }

            }
        }
    }
}
