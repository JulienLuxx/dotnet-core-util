using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Util
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMapUtil(this IServiceCollection services)
        {
            services.AddScoped<IMapUtil, MapUtil>();
            return services;
        }

        public static IServiceCollection AddEncryptUtil(this IServiceCollection services)
        {
            services.AddScoped<IEncryptUtil, EncryptUtil>();
            return services;
        }
    }
}
