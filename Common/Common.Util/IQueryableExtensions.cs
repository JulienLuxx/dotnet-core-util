using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Common.Util
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// OrderByColumnName
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">IQueryableSource</param>
        /// <param name="columnName">ColumnName</param>
        /// <param name="isDesc">DESCorASC</param>
        /// <returns></returns>
        public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string columnName, bool isDesc) where TSource : class
        {
            try
            {
                Type type = typeof(TSource);
                ParameterExpression param = Expression.Parameter(type, "x");
                PropertyInfo propertyInfo = type.GetProperty(columnName);
                MemberExpression selector = Expression.MakeMemberAccess(param, propertyInfo);
                LambdaExpression lambda = Expression.Lambda(selector, param);
                string methodName = (isDesc) ? "OrderByDescending" : "OrderBy";

                var typeArray = new Type[]
                {
                    type,
                    propertyInfo.PropertyType
                };
                MethodCallExpression callExpression = Expression.Call(typeof(Queryable), methodName, typeArray, source.Expression, lambda);

                return source.Provider.CreateQuery<TSource>(callExpression);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
