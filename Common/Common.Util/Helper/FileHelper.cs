using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            else if (string.CompareOrdinal(extension, ".dll") == 0)
            {
                return FileType.WindowsDLL;
            }
            else
            {
                return FileType.UnSupport;
            }
        }

        public static string GetRuntimePath(string path)
        {
            if (RuntimeHelper.IsUnixRunTime())
            {
                return GetUnixPath(path);
            }
            if (RuntimeHelper.IsWindowRunTime())
            {
                return GetWindowPath(path);
            }
            return path;
        }

        public static string GetUnixPath(string path)
        {
            string pathTemp = Path.Combine(path);
            return pathTemp.Replace("\\", "/");
        }

        public static string GetWindowPath(string path)
        {
            string pathTemp = Path.Combine(path);
            return pathTemp.Replace("/", "\\");
        }

        public static bool VerifyFileExtesion(string extension)
        {
            var extensionList = new List<string>()
            {
                ".asp",
                ".aspx",
                ".ashx",
                ".asa",
                ".asax",
                ".asmx",
                ".dll",
                ".exe",
                ".js",
                ".jsp",
                ".htm",
                ".html",
                ".sql",
                ".php",
                ".py"
            };
            var flag = !extensionList.Where(x => string.CompareOrdinal(extension, x) == 0).Any();
            return flag;
        }
    }
}
