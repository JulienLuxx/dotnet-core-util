using Common.Util;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Common.CoreUtil
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Get IEncryptUtil from IServiceProvider
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IEncryptUtil GetEncryptUtilService(this IServiceProvider provider)
        {
            return provider.GetService<IEncryptUtil>();
        }

        /// <summary>
        /// Get IMapUtil from IServiceProvider
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IMapUtil GetMapUtilService(this IServiceProvider provider)
        {
            return provider.GetService<IMapUtil>();
        }

        /// <summary>
        /// Get IHttpClientUtil from IServiceProvider
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IHttpClientUtil GetHttpClientUtilService(this IServiceProvider provider)
        {
            return provider.GetService<IHttpClientUtil>();
        }
    }
}
