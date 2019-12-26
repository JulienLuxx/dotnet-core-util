using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Util
{
    public static class ServiceProviderExtensions
    {
        public static IEncryptUtil GetEncryptUtilService(this IServiceProvider provider)
        {
            return provider.GetService<IEncryptUtil>();
        }

        public static IMapUtil GetMapUtilService(this IServiceProvider provider)
        {
            return provider.GetService<IMapUtil>();
        }
    }
}
