using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Util
{
    public interface IMapUtil : IDependency
    {
        IDictionary<string, string> ObjectToDictionary(object obj);

        IDictionary<string, string> DynamicToDictionary(dynamic obj);

        List<string> DynamicToStringList(dynamic obj);

        List<string> DictionaryToStringList(IDictionary<string, string> dict);
    }
}
