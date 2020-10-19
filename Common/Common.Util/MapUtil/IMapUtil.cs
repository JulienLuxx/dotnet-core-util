using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Util
{
    public interface IMapUtil : IDependency
    {
        /// <summary>
        /// EntityObjectConvertToEntityDictionary(Suppot Use DescriptionAttribute)
        /// </summary>
        /// <param name="obj">EntityObject</param>
        /// <returns></returns>
        IDictionary<string, string> ObjectToDictionary(object obj);

        /// <summary>
        /// DynamicEntityObjectConvertToEntityDictionary(Not Suppot Use DescriptionAttribute)
        /// </summary>
        /// <param name="obj">DynamicEntity</param>
        /// <returns></returns>
        IDictionary<string, string> DynamicToDictionary(dynamic obj);

        /// <summary>
        /// EntityObjectConvertToEntityDictionary(Suppot Use DescriptionAttribute)
        /// </summary>
        /// <typeparam name="T">EntityObjectType</typeparam>
        /// <param name="obj">EntityObject</param>
        /// <returns></returns>
        IDictionary<string, string> EntityToDictionary<T>(T obj) where T : class;

        /// <summary>
        /// EntityObjectConvertToEntityDictionary(Suppot Use DescriptionAttribute)
        /// </summary>
        /// <typeparam name="T">EntityClassOrStructType</typeparam>
        /// <param name="obj">EntityClassOrStruct</param>
        /// <returns></returns>
        IDictionary<string, string> DynamicToDictionary<T>(T obj);

        /// <summary>
        /// GetEntityClassAllPropertyNames(Not Support Async/Await)
        /// </summary>
        /// <param name="type">EntityClassType</param>
        /// <returns></returns>
        Span<string> GetAllPropertyNames(Type type);

        /// <summary>
        /// GetEntityClassAllPropertyNames(Support Async/Await)
        /// </summary>
        /// <param name="type">EntityClassType</param>
        /// <returns></returns>
        Memory<string> GetAllPropertyName(Type type);

        /// <summary>
        /// DynamicEntityObjectConvertToCookieStringList
        /// </summary>
        /// <param name="obj">DynamicEntityObject</param>
        /// <returns></returns>
        [Obsolete]
        List<string> DynamicToStringList(dynamic obj);

        /// <summary>
        /// DynamicEntityObjectConvertToCookieStringList(Not Suppot Use DescriptionAttribute)
        /// </summary>
        /// <param name="obj">DynamicEntityObject</param>
        /// <returns></returns>
        List<string> DynamicToCookieStrList(dynamic obj);

        /// <summary>
        /// EntityObjectConvertToCookieStringList(Suppot Use DescriptionAttribute)
        /// </summary>
        /// <typeparam name="T">EntityObjectType</typeparam>
        /// <param name="obj">EntityObject</param>
        /// <returns></returns>
        List<string> EntityToCookieStrList<T>(T obj) where T : class;

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
