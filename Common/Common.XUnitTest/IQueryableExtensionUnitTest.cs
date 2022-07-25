using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Common.Util;

namespace Common.XUnitTest
{
    public class IQueryableExtensionUnitTest: BaseUnitTest
    {

        [Fact]
        public void JoinStringTest()
        {
            var list = new List<string>
            {
                "0","9","2"
            };
            var str = list.JoinString();
            Assert.Equal(5, str.Length);
        }
    }
}
