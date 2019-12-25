using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Common.XUnitTest
{
    public class BaseUnitTest
    {
        protected ServiceCollection _serviceCollection { get; set; }

        protected ServiceProvider _serviceProvider { get; set; }

        protected BaseUnitTest()
        {
            _serviceCollection = new ServiceCollection();
        }

        protected virtual void BuilderServiceProvider()
        {
            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }
    }
}
