using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Polly;
using Polly.Extensions.Http;

namespace Common.BaiduAICore
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOcrUtil(this IServiceCollection services)
        {
            //if (!services.Where(x => x.ServiceType == typeof(IHttpClientFactory)).Any())
            //{                
            //}
            services.AddHttpClient("BaiduOcr");
            services.AddScoped<IOcrUtil, OcrUtil>();
            return services;
        }

        public static IServiceCollection AddOcrUtilWithPolly(this IServiceCollection services, int retryCount, int seconds)
        {
            services
                .AddHttpClient("BaiduOcr")
                .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(retryCount, _ => TimeSpan.FromSeconds(seconds)));
            return services;
        }
    }
}
