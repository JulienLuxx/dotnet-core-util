using Common.SMUtil;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
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
            handle.SetIv("0000000000000000");
            handle.SetSecretKey("p7CorxBtvdSaKoGt");

            //act
            var encrypted = handle.EncryptECB(text, Encoding.UTF8);
            Assert.NotEmpty(encrypted);
            var original=handle.DecryptECB(encrypted, Encoding.UTF8);

            //assert
            Assert.Equal(text, original);
        }

                [Theory]
        [InlineData("gxrc@601")]
        public void EncryptAndDecryptCBCTest(string text)
        {
            //arrange
            //var handle = new SM4CryptoUtil(@"Pxb6zhoARp-PrlE5", "3120120458946774");
            //var handle = new SM4CryptoUtil(@"e1uQjzqp84nfp_kt", "3120120458946774");
            var handle = new SM4CryptoUtil();
            handle.SetIv("8056518210210740");
            handle.SetSecretKey("wIHsRqkB_mSQQd1V");

            ////handle.SetIv("4230073116883844");
            ////handle.SetSecretKey("q59o6XP_VLTf4fBo");


            //handle.hexString = true;
            //handle.iv = "6692921803744201";
            //handle.secretKey = "p7CorxBtvdSaKoGt";
            //handle.iv = "6689827651079013";
            //handle.iv = "0000000000000000";
            //handle.secretKey = "SK_LhSmCmQ6su3Ct";

            //act
            var encrypted = handle.EncryptECB(text, Encoding.UTF8);
            Assert.NotEmpty(encrypted);
            var original = handle.DecryptECB(encrypted, Encoding.UTF8);

            //assert
            Assert.Equal(text, original);
        }

        [Theory]
        //[InlineData("E379A7C601CC829D30EE01791A492780")]
        //[InlineData("65CA9EE0DC812A392543B3D16E654F51")]
        //[InlineData("BFD5C791257D0CF5EC8A1959B1709762")]

        //[InlineData("C7A42D07C3955FB3428231595478EE53")]
        //[InlineData("620961CC2B1E677146AAB6FC1F2F0E4C")]
        [InlineData("61566fa163df102049fbde6e88e3e13b")]
        public void DecryptCBCTest(string text)
        {
            var handle = new SM4CryptoUtil();
            handle.SetIv("3120120458946774");
            handle.SetSecretKey("Pxb6zhoARp-PrlE5");
            //handle.SetSecretKey("SK_LhSmCmQ6su3Ct");
            //handle.SetIv("6692921803744201");
            //handle.SetSecretKey ("YbBh8ygcPBa24knb");
            //handle.SetSecretKey("lTcF5hKQh-fuG2ef");

            //handle.SetIv("3120120458946774");
            //handle.SetSecretKey("Pxb6zhoARp-PrlE5");

            var original = handle.DecryptECB(text, Encoding.UTF8);
            Assert.NotEmpty(original);
        }


        [Theory]
        [InlineData("gxrc@666")]                                  // 单块（< 16 字节）
        [InlineData("0123456789abcdef")]                          // 整 16 字节边界（填充后 2 块）
        [InlineData("Hello SM4 CBC 多块解密验证测试123")]           // 多块（33 字节 → 3 块）
        public void UtilEncryptCBCTest(string text)
        {
            var handle = new SM4CryptoUtil();
            handle.SetIv("3120120458946774");
            handle.SetSecretKey("Pxb6zhoARp-PrlE5");

            var encrypted = handle.EncryptCBC(text, Encoding.UTF8);
            Assert.NotEmpty(encrypted);
            var original = handle.DecryptCBC(encrypted, Encoding.UTF8);
            Assert.Equal(text, original);
        }

        [Fact]
        public void EncryptCBCMatchesBouncyCastleTest()
        {
            //arrange
            const string keyHex = "0123456789abcdeffedcba9876543210";
            const string ivHex = "0123456789abcdeffedcba9876543210";
            var handle = new SM4CryptoUtil(keyHex, ivHex, true);
            var plainText = "Hello SM4 CBC 多块解密验证测试123";

            //act：本实现加密
            var encrypted = handle.EncryptCBC(plainText, Encoding.UTF8);

            //act：BouncyCastle 标准实现加密（SM4/CBC/PKCS7Padding）
            var key = Hex.Decode(keyHex);
            var iv = Hex.Decode(ivHex);
            var bcCipher = CipherUtilities.GetCipher("SM4/CBC/PKCS7Padding");
            var keyParam = ParameterUtilities.CreateKeyParameter("SM4", key);
            bcCipher.Init(true, new ParametersWithIV(keyParam, iv));
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var output = new byte[bcCipher.GetOutputSize(plainBytes.Length)];
            var length = bcCipher.ProcessBytes(plainBytes, 0, plainBytes.Length, output, 0);
            length += bcCipher.DoFinal(output, length);
            var bcResult = Hex.ToHexString(output, 0, length);

            //assert：两实现密文逐字节一致（Hex 大小写不影响字节等价，忽略大小写比较）
            Assert.Equal(bcResult, encrypted, ignoreCase: true);
        }

        [Fact]
        public void CbcIvLengthValidationTest()
        {
            //arrange：默认 iv = ""，编码后 0 字节；以及超长 IV
            var emptyIvHandle = new SM4CryptoUtil("0123456789abcdef", "", false);
            var longIvHandle = new SM4CryptoUtil("0123456789abcdef", "0123456789abcdef00", false);

            //act + assert：IV 非 16 字节时抛 ArgumentOutOfRangeException
            Assert.Throws<ArgumentOutOfRangeException>(() => emptyIvHandle.EncryptCBC("test", Encoding.UTF8));
            Assert.Throws<ArgumentOutOfRangeException>(() => emptyIvHandle.DecryptCBC("00", Encoding.UTF8));
            Assert.Throws<ArgumentOutOfRangeException>(() => longIvHandle.EncryptCBC("test", Encoding.UTF8));
            Assert.Throws<ArgumentOutOfRangeException>(() => longIvHandle.DecryptCBC("00", Encoding.UTF8));
        }
    }
}
