using System;
using System.Collections.Generic;
using System.Text;

namespace Common.AliyunCSBCore
{
    public interface ISignUtil
    {
        string Sign(IDictionary<string, string> dict, string key);
    }
}
