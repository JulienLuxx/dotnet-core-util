using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Util
{
    public interface IMapUtil : IDependency
    {
        /// <summary>
        /// EntityObjectConvertToEntityDictionary
        /// </summary>
        /// <param name="obj">EntityObject</param>
        /// <returns></returns>
        IDictionary<string, string> ObjectToDictionary(object obj);

        /// <summary>
        /// DynamicEntityObjectConvertToEntityDictionary
        /// </summary>
        /// <param name="obj">DynamicEntityObject</param>
        /// <returns></returns>
        IDictionary<string, string> DynamicToDictionary(dynamic obj);

        /// <summary>
        /// DynamicEntityObjectConvertToCookieStringList
        /// </summary>
        /// <param name="obj">DynamicEntityObject</param>
        /// <returns></returns>
        [Obsolete]
        List<string> DynamicToStringList(dynamic obj);

        /// <summary>
        /// DynamicEntityObjectConvertToCookieStringList
        /// </summary>
        /// <param name="obj">DynamicEntityObject</param>
        /// <returns></returns>
        List<string> DynamicToCookieStrList(dynamic obj);

        /// <summary>
        /// CookieDictionaryConvertToCookieStringList
        /// </summary>
        /// <param name="dict">CookieDictionary</param>
        /// <returns></returns>
        [Obsolete]
        List<string> DictionaryToStringList(IDictionary<string, string> dict);

        /// <summary>
        /// CookieDictionaryConvertToCookieStringList
        /// </summary>
        /// <param name="dict">CookieDictionary</param>
        /// <returns></returns>
        List<string> CookieDictToCookieStrList(IDictionary<string, string> dict);
    }
}
