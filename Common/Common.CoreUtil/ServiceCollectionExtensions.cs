using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
            services.AddScoped<IHttpClientUtil, HttpClientUtil>();
            return services;
        }
    }
}
