using Common.Util;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

namespace Common.CoreUtil
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add HttpClientUtil to IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClientUtil(this IServiceCollection services)
        {
            if (services.Where(x => x.ServiceType == typeof(IHttpClientFactory)).Any())
            {
                services.AddHttpClient();
            }
            if (services.Where(x => x.ServiceType == typeof(IMapUtil)).Any())
            {
                services.AddScoped<IMapUtil, MapUtil>();
            }
            services.AddScoped<IHttpClientUtil, HttpClientUtil>();
            return services;
        }
    }
}
