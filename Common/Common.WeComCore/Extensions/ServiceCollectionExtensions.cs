using Common.CoreUtil;
using Common.Util;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Common.WeComCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWeComCore(this IServiceCollection services)
        {
            if (!services.Where(x => x.ServiceType == typeof(IHttpClientFactory)).Any())
            {
                services.AddHttpClient();
            }
            if (!services.Where(x => x.ServiceType == typeof(IMapUtil)).Any())
            {
                services.AddMapUtil();
            }
            if (!services.Where(x => x.ServiceType == typeof(IHttpClientUtil)).Any())
            {
                services.AddHttpClientUtil();
            }
            services.AddScoped<IWeComCoreSvc, WeComCoreSvc>();
            return services;
        }

    }
}
