using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;

namespace Common.WeComCore
{
    public static class ServiceProviderExtensions
    {
        public static IWeComCoreSvc GetWeComCore(this IServiceProvider provider)
        {
            return provider.GetService<IWeComCoreSvc>();
        }

    }
}

