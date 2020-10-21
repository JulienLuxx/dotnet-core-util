using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Util
{
    public static class FileHelper
    {
        public static FileType GetFileType(string extension)
        {
            if (string.CompareOrdinal(extension, ".jpg") == 0
                || string.CompareOrdinal(extension, "jpeg") == 0
                || string.CompareOrdinal(extension, "png") == 0
                || string.CompareOrdinal(extension, "gif") == 0)
            {
                return FileType.Image;
            }
            else if (string.CompareOrdinal(extension, ".txt") == 0)
            {
                return FileType.Text;
            }
            else if (string.CompareOrdinal(extension, ".zip") == 0 || string.CompareOrdinal(extension, ".rar") == 0)
            {
                return FileType.Zip;
            }
            else if (string.CompareOrdinal(extension, ".xls") == 0 || string.CompareOrdinal(extension, "xlsx") == 0)
            {
                return FileType.Excel;
            }
            else if (string.CompareOrdinal(extension, ".pdf") == 0)
            {
                return FileType.PDF;
            }
            else if (string.CompareOrdinal(extension, "doc") == 0 || string.CompareOrdinal(extension, "docx") == 0)
            {
                return FileType.Word;
            }
            else if (string.CompareOrdinal(extension, ".ppt") == 0 || string.CompareOrdinal(extension, ".pptx") == 0)
            {
                return FileType.PPT;
            }
            else if (string.CompareOrdinal(extension, ".mp3") == 0
                || string.CompareOrdinal(extension, ".wav") == 0
                || string.CompareOrdinal(extension, ".m4a") == 0)
            {
                return FileType.Audio;
            }
            else if (string.CompareOrdinal(extension, ".mp4") == 0
                || string.CompareOrdinal(extension, ".avi") == 0
                || string.CompareOrdinal(extension, "rm") == 0
                || string.CompareOrdinal(extension, "mkv") == 0
                || string.CompareOrdinal(extension, "rmvb") == 0)
            {
                return FileType.Video;
            }
            else if (string.CompareOrdinal(extension, ".exe") == 0 || string.CompareOrdinal(extension, "mis") == 0)
            {
                return FileType.WindowsExecutable;
            }
            else
            {
                return FileType.UnSupport;
            }
        }
    }
}
