using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Common.BaiduAICore
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOcrUtil(this IServiceCollection services)
        {
            if (!services.Where(x => x.ServiceType == typeof(IHttpClientFactory)).Any())
            {
                services.AddHttpClient();
            }
            services.AddScoped<IOcrUtil, OcrUtil>();
            return services;
        }
    }
}
