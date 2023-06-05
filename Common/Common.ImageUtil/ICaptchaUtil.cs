using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ImageUtil
{
    public interface ICaptchaUtil
    {
        byte[] GenerateCaptcha(string code, int width, int height, int textSize);
    }
}
