using Common.SMUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Common.SMUtil.SM2CryptoUtil;

namespace Common.XUnitTest
{
    public class SM2UnitTest
    {
        const string generatePrivateKey = @"00C6210A6F323B173555C629B9DFC9EECA113A9513D0826890D7E03974CC649D07";
        const string generatePublicKey = @"04C7F3967640F9907EEEABF46EB45201B13F9D3DF7351E7223E777CBA9EC9427F771A006CC8C4023C28474E2D08A06BA5439479927F01D4CC33CAD25C84F7F6108";

        [Theory]
        [InlineData("0462D9E0B5E0E7CF6D10A81A8C3A1BAE3BF85B5970B9A7EF42734F3B1AA899DCD92F786C66092BA7FDD34D7A10313844D12AE1BA3443C7F770E68099F25B0DEBE0B7338E4D57032D724DA5A847CBB77062509AC6512E9F8C98DA3535761E11BF18565793EE6B84D591B04D86FE9CD99274C8C33E4B1B8DF608A997179CCC1A89164D8A2A80B708183D68416C93B02E91CE399713351BDF5D6B0896B5813BD34130385AB485F68A3F12911423")]
        public void DecryptTest(string text)
        {
            var str = "{\"aac002\":\"45012119930940463X\",\"aac003\":\"潘某某\",\"aac045\":\"18476927841\"}";
            var hexStr = ByteUtils.ToHexString(Encoding.UTF8.GetBytes(str));

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // arrange
            var handle = new SM2CryptoUtil(generatePublicKey, generatePrivateKey, SM2CryptoUtil.Mode.C1C3C2);
            // act
            var textBytes = ByteUtils.GetBytesByHexString(text);
            var originalBytes = handle.Decrypt(textBytes);
            var original = Encoding.UTF8.GetString(originalBytes);


            Assert.Equal("{\"aac002\":\"45012119930940463X\",\"aac003\":\"潘某某\",\"aac045\":\"18476927841\"}", original);
        }

        [Theory]
        [InlineData("{\"aac002\":\"45012119930940463X\",\"aac003\":\"潘某某\",\"aac045\":\"18476927841\"}")]
        public void EncryptAndDecryptTest(string plainTxt)
        {
            // arrange
            var handle = new SM2CryptoUtil(generatePublicKey, generatePrivateKey, SM2CryptoUtil.Mode.C1C3C2);

            // act
            var encrypted = handle.Encrypt(Encoding.UTF8.GetBytes(plainTxt));
            var encryptedTxt = ByteUtils.ToHexString(encrypted);
            Assert.NotEmpty(encrypted);

            var textBytes = ByteUtils.GetBytesByHexString(encryptedTxt);
            var originalBytes = handle.Decrypt(textBytes);
            var original = Encoding.UTF8.GetString(originalBytes);

            //assert
            Assert.Equal("{\"aac002\":\"45012119930940463X\",\"aac003\":\"潘某某\",\"aac045\":\"18476927841\"}", original);
        }

        [Theory]
        [InlineData("{\"aac002\":\"45012119930940463X\",\"aac003\":\"潘某某\",\"aac045\":\"18476927841\"}")]
        public void SignTest(string text)
        {
            var hexText = ByteUtils.ToHexString(Encoding.UTF8.GetBytes(text));
            // arrange
            var handle = new SM2CryptoUtil("", generatePrivateKey, Mode.C1C3C2);
            var signedBytes = handle.Sign(Encoding.UTF8.GetBytes(text));
            var signed = ByteUtils.ToHexString(signedBytes);

            Assert.NotEmpty(signed);
        }

        [Theory]
        [InlineData("3046022100cce97958385426fa8528d7d39bb75f98259ca24bcace4c3c1b1fd3ad631af371022100cafbf4f4c9d01bdd76c5305fff80d2d39a24fe11c59dbd6bf757e3b7a884c839")]
        public void VerifySign(string cipherText)
        {
            var hexText = ByteUtils.ToHexString(Encoding.UTF8.GetBytes("{\"aac002\":\"45012119930940463X\",\"aac003\":\"潘某某\",\"aac045\":\"18476927841\"}"));
            var handle = new SM2CryptoUtil(null, generatePrivateKey, Mode.C1C3C2);
            var textBytes = ByteUtils.GetBytesByHexString(cipherText);
            var flag = handle.VerifySign(ByteUtils.GetBytesByHexString(hexText), textBytes);
            Assert.True(flag);
        }
    }
}
