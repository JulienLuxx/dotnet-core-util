using Microsoft.Extensions.DependencyInjection;
using System;

namespace Common.CoreUtil
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Get HttpClientUtil from IServiceProvider
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IHttpClientUtil GetHttpClientUtilService(this IServiceProvider provider)
        {
            return provider.GetService<IHttpClientUtil>();
        }
    }
}
