using Common.Util;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net;

namespace Common.CoreUtil
{
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register Common.Util.IMapUtil to IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMapUtil(this IServiceCollection services)
        {
            services.AddScoped<IMapUtil, MapUtil>();
            return services;
        }

        /// <summary>
        /// Register Common.Util.IEncryptUtil to IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEncryptUtil(this IServiceCollection services)
        {
            services.AddScoped<IEncryptUtil, EncryptUtil>();
            return services;
        }

        /// <summary>
        /// Register Common.Util.IGPSUtil to IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddGPSUtil(this IServiceCollection services)
        {
            services.AddScoped<IGPSUtil, GPSUtil>();
            return services;
        }

        /// <summary>
        /// Register HttpClientUtil to IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClientUtil(this IServiceCollection services)
        {
            if (!services.Where(x => x.ServiceType == typeof(IHttpClientFactory)).Any())
            {
                services.AddHttpClient("default");
                services.AddHttpClient<HttpClient>("decompression").ConfigurePrimaryHttpMessageHandler(h =>
                {
                    var handler = new HttpClientHandler();
                    //handler.AutomaticDecompression = DecompressionMethods.GZip;
                    if (handler.SupportsAutomaticDecompression)
                    {
                        handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    }
                    return handler;
                });
            }
            if (!services.Where(x => x.ServiceType == typeof(IMapUtil)).Any())
            {
                services.AddScoped<IMapUtil, MapUtil>();
            }
            services.AddScoped<IHttpClientUtil, HttpClientUtil>();
            return services;
        }
    }
}
