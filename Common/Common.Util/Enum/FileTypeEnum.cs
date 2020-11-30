using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Common.Util
{
    public enum FileType
    {
        [Description("Audio")]
        Audio,

        [Description("Image")]
        Image,

        [Description("Excel")]
        Excel,

        [Description("PDF")]
        PDF,

        [Description("PPT")]
        PPT,

        [Description("Text")]
        Text,

        [Description("Video")]
        Video,

        [Description("WindowsExecutable")]
        WindowsExecutable,

        [Description("WindowsDLL")]
        WindowsDLL,

        [Description("Word")]
        Word,

        [Description("Zip")]
        Zip,

        [Description("UnSupport")]
        UnSupport=-1
    }
}
