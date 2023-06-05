using SkiaSharp;
using SkiaSharp.QrCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ImageUtil
{
    public interface IQrCodeUtil
    {
        /// <summary>
        /// GenerateQRCodeHasIconEmbedded
        /// </summary>
        /// <param name="content"></param>
        /// <param name="codeColor"></param>
        /// <param name="level"></param>
        /// <param name="iconData"></param>
        /// <param name="iconSize"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="useRect"></param>
        /// <returns></returns>
        byte[] GenerateEmbeddedIconQrCode(string content, SKColor? codeColor, ECCLevel level, byte[] iconData, int iconSize = 15, int width = 512, int height = 512, bool useRect = false);
    }
}
