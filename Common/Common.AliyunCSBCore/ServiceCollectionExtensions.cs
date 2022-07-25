using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.AliyunCSBCore
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSignUtil(this IServiceCollection services)
        {
            //if (!services.Where(x => x.ServiceType == typeof(IHttpClientFactory)).Any())
            //{
            //    services.AddHttpClient();
            //}
            services.AddSingleton<ISignUtil, SignUtil>();
            return services;
        }
    }
}
