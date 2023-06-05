using SkiaSharp.QrCode.Models;
using SkiaSharp.QrCode;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ImageUtil
{
    public class QrCodeUtil : IQrCodeUtil
    {
        public byte[] GenerateEmbeddedIconQrCode(string content, SKColor? codeColor, ECCLevel level, byte[] iconData, int iconSize = 15, int width = 512, int height = 512, bool useRect = false)
        {
            var icon = new IconData
            {
                Icon = SKBitmap.Decode(iconData),
                IconSizePercent = iconSize
            };
            // Generate QrCode
            using (var generator = new QRCodeGenerator())
            {
                var qr = generator.CreateQrCode(content, level);
                // Render to canvas
                var info = new SKImageInfo(width, height);
                using (var surface = SKSurface.Create(info))
                {
                    var canvas = surface.Canvas;
                    if (useRect)
                    {
                        canvas.Render(qr, new SKRect(0, 0, info.Width, info.Height), SKColor.Empty, codeColor.Value, icon);
                    }
                    else
                    {
                        canvas.Render(qr, info.Width, info.Height, SKColor.Empty, codeColor.Value, icon);
                    }

                    using (var image = surface.Snapshot())
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        return data.ToArray();
                    }
                }

            }
        }
    }
}
