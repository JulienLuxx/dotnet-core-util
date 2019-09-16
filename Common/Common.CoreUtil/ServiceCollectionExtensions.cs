using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.CoreUtil
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpClientUtil(this IServiceCollection services)
        {
            services.AddScoped<IHttpClientUtil, HttpClientUtil>();
            return services;
        }
    }
}
