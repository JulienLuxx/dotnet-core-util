using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Util
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add MapUtil to IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMapUtil(this IServiceCollection services)
        {
            services.AddScoped<IMapUtil, MapUtil>();
            return services;
        }

        /// <summary>
        /// Add EncryptUtil to IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEncryptUtil(this IServiceCollection services)
        {
            services.AddScoped<IEncryptUtil, EncryptUtil>();
            return services;
        }
    }
}
