﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Common.Util
{
    public class MapUtil : IMapUtil
    {
        /// <summary>
        /// EntityObjectConvertToEntityDictionary(Suppot Use DescriptionAttribute)
        /// </summary>
        /// <param name="obj">EntityObject</param>
        /// <returns></returns>
        public IDictionary<string, string> ObjectToDictionary(object obj)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();

            var type = obj.GetType();
            var propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in propertys)
            {
                var method = property.GetGetMethod();
                if (null != method && method.IsPublic)
                {
                    if (null != property.GetValue(obj))
                    {
                        var description = property.CustomAttributes.Where(x => x.AttributeType.Equals(typeof(DescriptionAttribute))).Select(s => s.ConstructorArguments.FirstOrDefault()).FirstOrDefault();
                        if (null != description.Value)
                        {
                            dict.Add(description.Value.ToString(), property.GetValue(obj).ToString());
                        }
                        else
                        {
                            dict.Add(property.Name, property.GetValue(obj).ToString());
                        }
                    }
                }
            }
            return dict;
        }

        /// <summary>
        /// DynamicEntityObjectConvertToEntityDictionary(Not Suppot Use DescriptionAttribute)
        /// </summary>
        /// <param name="obj">DynamicEntity</param>
        /// <returns></returns>
        public IDictionary<string, string> DynamicToDictionary(dynamic obj)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();

            var type = obj.GetType();
            var propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in propertys)
            {
                var method = property.GetGetMethod();
                if (null != method && method.IsPublic)
                {
                    if (null != property.GetValue(obj))
                    {
                        dict.Add(property.Name, property.GetValue(obj).ToString());
                    }
                }
            }
            return dict;
        }

        /// <summary>
        /// EntityObjectConvertToEntityDictionary(Suppot Use DescriptionAttribute)
        /// </summary>
        /// <typeparam name="T">EntityObjectType</typeparam>
        /// <param name="obj">EntityObject</param>
        /// <returns></returns>
        public IDictionary<string, string> EntityToDictionary<T>(T obj) where T : class
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            if (null == obj)
            {
                return dict;
            }
            var type = obj.GetType();
            var propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in propertys)
            {
                var method = property.GetGetMethod();
                if (null != method && method.IsPublic)
                {
                    if (null != property.GetValue(obj))
                    {
                        var description = property.CustomAttributes.Where(x => x.AttributeType.Equals(typeof(DescriptionAttribute))).Select(s => s.ConstructorArguments.FirstOrDefault()).FirstOrDefault();
                        if (null != description.Value)
                        {
                            dict.Add(description.Value.ToString(), property.GetValue(obj).ToString());
                        }
                        else
                        {
                            dict.Add(property.Name, property.GetValue(obj).ToString());
                        }
                    }
                }
            }
            return dict;
        }

        /// <summary>
        /// EntityObjectConvertToEntityDictionary(Suppot Use DescriptionAttribute)
        /// </summary>
        /// <typeparam name="T">EntityClassOrStructType</typeparam>
        /// <param name="obj">EntityClassOrStruct</param>
        /// <returns></returns>
        public IDictionary<string, string> DynamicToDictionary<T>(T obj)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            if (null == obj)
            {
                return dict;
            }
            var type = obj.GetType();
            var propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in propertys)
            {
                var method = property.GetGetMethod();
                if (null != method && method.IsPublic)
                {
                    if (null != property.GetValue(obj))
                    {
                        var description = property.CustomAttributes.Where(x => x.AttributeType.Equals(typeof(DescriptionAttribute))).Select(s => s.ConstructorArguments.FirstOrDefault()).FirstOrDefault();
                        if (null != description.Value)
                        {
                            dict.Add(description.Value.ToString(), property.GetValue(obj).ToString());
                        }
                        else
                        {
                            dict.Add(property.Name, property.GetValue(obj).ToString());
                        }
                    }
                }
            }
            return dict;
        }

        public Span<string> GetAllPropertyNames(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => null != x.GetGetMethod() & x.GetGetMethod().IsPublic);
            var list = new List<string>();
            foreach (var property in properties)
            {
                var description = property.CustomAttributes.Where(x => x.AttributeType.Equals(typeof(DescriptionAttribute))).Select(s => s.ConstructorArguments.FirstOrDefault()).FirstOrDefault();
                if (null != description.Value)
                {
                    list.Add(description.Value.ToString());
                }
                else
                {
                    list.Add(property.Name);
                }
            }
            return list.ToArray().AsSpan();
        }

        public Memory<string> GetAllPropertyName(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => null != x.GetGetMethod() & x.GetGetMethod().IsPublic);
            var list = new List<string>();
            foreach (var property in properties)
            {
                var description = property.CustomAttributes.Where(x => x.AttributeType.Equals(typeof(DescriptionAttribute))).Select(s => s.ConstructorArguments.FirstOrDefault()).FirstOrDefault();
                if (null != description.Value)
                {
                    list.Add(description.Value.ToString());
                }
                else
                {
                    list.Add(property.Name);
                }
            }
            return list.ToArray().AsMemory();
        }

        /// <summary>
        /// DynamicEntityObjectConvertToCookieStringList
        /// </summary>
        /// <param name="obj">DynamicEntityObject</param>
        /// <returns></returns>
        [Obsolete]
        public List<string> DynamicToStringList(dynamic obj)
        {
            var list = new List<string>();
            var type = obj.GetType();
            var propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in propertys)
            {
                var method = property.GetGetMethod();
                if (null != method && method.IsPublic)
                {
                    if (null != property.GetValue(obj))
                    {
                        var str = property.Name + "=" + property.GetValue(obj).ToString();
                        list.Add(str);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// DynamicEntityObjectConvertToCookieStringList
        /// </summary>
        /// <param name="obj">DynamicEntityObject</param>
        /// <returns></returns>
        public List<string> DynamicToCookieStrList(dynamic obj)
        {
            var list = new List<string>();
            var type = obj.GetType();
            var propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in propertys)
            {
                var method = property.GetGetMethod();
                if (null != method && method.IsPublic)
                {
                    if (null != property.GetValue(obj))
                    {
                        var str = property.Name + "=" + property.GetValue(obj).ToString();
                        list.Add(str);
                    }
                }
            }
            return list;
        }

        public List<string> EntityToCookieStrList<T>(T obj) where T : class
        {
            var list = new List<string>();
            var type = obj.GetType();
            var propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in propertys)
            {
                var method = property.GetGetMethod();
                if (null != method && method.IsPublic)
                {
                    if (null != property.GetValue(obj))
                    {
                        var description = property.CustomAttributes.Where(x => x.AttributeType.Equals(typeof(DescriptionAttribute))).Select(s => s.ConstructorArguments.FirstOrDefault()).FirstOrDefault();
                        if (null != description.Value) 
                        {
                            var str = description.Value.ToString() + "=" + property.GetValue(obj).ToString();
                            list.Add(str);
                        }
                        else
                        {
                            var str = property.Name + "=" + property.GetValue(obj).ToString();
                            list.Add(str);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// CookieDictionaryConvertToCookieStringList
        /// </summary>
        /// <param name="dict">CookieDictionary</param>
        /// <returns></returns>
        [Obsolete]
        public List<string> DictionaryToStringList(IDictionary<string, string> dict)
        {
            var list = new List<string>();
            foreach (var item in dict)
            {
                var str = item.Key + "=" + item.Value;
                list.Add(str);
            }
            return list;
        }

        /// <summary>
        /// CookieDictionaryConvertToCookieStringList
        /// </summary>
        /// <param name="dict">CookieDictionary</param>
        /// <returns></returns>
        public List<string> CookieDictToCookieStrList(IDictionary<string, string> dict)
        {
            var list = new List<string>();
            foreach (var item in dict)
            {
                var str = item.Key + "=" + item.Value;
                list.Add(str);
            }
            return list;
        }
    }
}
