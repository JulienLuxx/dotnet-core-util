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

        /// <summary>
        /// CookieDictionaryConvertToCookieStringList
        /// </summary>
        /// <param name="dict">CookieDictionary</param>
        /// <returns></returns>
        List<string> DictionaryToStringList(IDictionary<string, string> dict);

        /// <summary>
        /// CookieDictionaryConvertToCookieStringList
        /// </summary>
        /// <param name="dict">CookieDictionary</param>
        /// <returns></returns>
        List<string> CookieDictToCookieStrList(IDictionary<string, string> dict);
    }
}
