﻿using System;
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
        /// GetEntityPropertyNameStringList
        /// </summary>
        /// <typeparam name="T">EntityObjectType</typeparam>
        /// <param name="obj">EntityObject</param>
        /// <param name="list"></param>
        void GetEntityPropertyNames<T>(T obj, ref List<string> list) where T : class;

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
