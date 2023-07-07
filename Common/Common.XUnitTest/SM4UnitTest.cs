using Common.SMUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Common.XUnitTest
{
    public class SM4UnitTest
    {

        [Theory]
        //[InlineData("{\"aac002\":\"45012119930940463X\",\"aac003\":\"潘某某\",\"aac045\":\"18476927841\"}")]
        [InlineData("gxrc@601")]
        public void EncryptAndDecryptECBTest(string text)
        {
            //arrange
            var handle = new SM4CryptoUtil();
            //handle.hexString = true;
            handle.iv = "0000000000000000";
            handle.secretKey = "p7CorxBtvdSaKoGt";

            //act
            var encrypted = handle.EncryptECB(text, Encoding.UTF8);
            Assert.NotEmpty(encrypted);
            var original=handle.DecryptECB(encrypted, Encoding.UTF8);

            //assert
            Assert.Equal(text, original);
        }
    }
}
