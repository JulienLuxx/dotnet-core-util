using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Common.Util;
using Xunit;

namespace Common.XUnitTest
{
    public enum TestEnum
    {
        [Description("Unable")]
        Unable=-1,

        [Description("Default")]
        Default,


        [Description("Activate")]
        Activate,
    }

    public class EnumExtensionsUnitTest
    {
        [Fact]
        public void GetDescriptionTest()
        {
            var name = TestEnum.Activate.GetDescription();
            Assert.Equal("Activate", name);
        }
    }
}
