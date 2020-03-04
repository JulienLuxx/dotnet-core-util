using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Util
{
    public static class EnumExtensions
    {
        /// <summary>
        /// GetEnumDescriptionAttributeString
        /// </summary>
        /// <param name="instance">EnumObject</param>
        /// <returns></returns>
        public static string GetDescription(this Enum instance)
        {
            var type = instance.GetType();            
            if (null == type)
            {
                return string.Empty;
            }
            var name = GetName(type, instance);
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }
            var member = type.GetTypeInfo().GetMember(name).FirstOrDefault();
            if (null == member)
            {
                return string.Empty;
            }
            return member.GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute attribute ? attribute.Description : member.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">EnumType</param>
        /// <param name="obj">Member,Value,ObjectInstance All Can</param>
        /// <returns></returns>
        public static string GetName(Type type, dynamic obj)
        {
            if (null == type)
            {
                return string.Empty;
            }
            if (null == obj)
            {
                return string.Empty;
            }
            if (obj is string)
            {
                obj.ToString();
            }
            if (type.GetTypeInfo().IsEnum == false)
            {
                return string.Empty;
            }
            return Enum.GetName(type, obj);
        }
    }
}
