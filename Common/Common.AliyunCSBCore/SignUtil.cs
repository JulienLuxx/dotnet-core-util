using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Common.AliyunCSBCore
{
    public class SignUtil : ISignUtil 
    {
        public string Sign(IDictionary<string, string> dict, string key)
        {
            var list = dict.DictionaryToParamNodeList();
            return Sign(list, key);
        }

        public string Sign(List<ParamNode> paramNodes, string key)
        {
            var paramList = new SortedParamList();
            paramList.AddAll(paramNodes);
            var dataStr=paramList.ToString();
            return SignAndBase64Encode(dataStr, key);
        }

        public string SignAndBase64Encode(string data, string key)
        {
            using (var sha1 = new HMACSHA1(Encoding.UTF8.GetBytes(key)))
            {
                var hash= sha1.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hash);
            }
            
        }
    }
}
