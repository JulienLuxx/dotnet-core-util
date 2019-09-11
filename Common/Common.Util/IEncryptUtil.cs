using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Util
{
    public interface IEncryptUtil : IDependency
    {
        string GetMd5By16(string value);

        string GetMd5By16(string value, Encoding encoding);

        string GetMd5By32(string value);

        string GetMd5By32(string value, Encoding encoding);
    }
}
