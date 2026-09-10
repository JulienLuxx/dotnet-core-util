using Common.SMUtil;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Common.XUnitTest
{
    public class SM2UnitTest
    {
        const string generatePrivateKey = @"00C6210A6F323B173555C629B9DFC9EECA113A9513D0826890D7E03974CC649D07";
        const string generatePublicKey = @"04C7F3967640F9907EEEABF46EB45201B13F9D3DF7351E7223E777CBA9EC9427F771A006CC8C4023C28474E2D08A06BA5439479927F01D4CC33CAD25C84F7F6108";

        // 已删除 4 个基于 SM2CryptoUtil 的死测试（DecryptTest/EncryptAndDecryptTest/SignTest/VerifySign）：
        // 该类依赖损坏的 SM3Digest，运行时抛 TypeLoadException，且已标注 [Obsolete(error)]，
        // 对应能力由 SM2Util 系列测试覆盖。

        [Theory]
        [InlineData()]
        public void GenerateKeyPairTest()
        {
            SM2Util.GenerateSM2KeyPair(true, out var privateKey, out var publicKey);
            Assert.NotEmpty(privateKey);
            Assert.NotEmpty(publicKey);
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void UtilEncryptAndDecryptTest(string plainTxt)
        {
            var base64Str=Convert.ToBase64String(Encoding.UTF8.GetBytes(plainTxt));
            var encrypted = SM2Util.Encrypt(plainTxt, generatePublicKey, Encoding.UTF8);
            var hexTxt=Hex.ToHexString(Encoding.UTF8.GetBytes(plainTxt));
            Assert.NotEmpty(encrypted);

            //var textBytes = ByteUtils.GetBytesByHexString(encryptedTxt);
            //var originalBytes = handle.Decrypt(textBytes);
            //var original = Encoding.UTF8.GetString(originalBytes);

            ////assert
            //Assert.Equal("{\"aac002\":\"45012119930940463X\",\"aac003\":\"潘某某\",\"aac045\":\"18476927841\"}", original);
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void UtilDecryptStrippedLeadingZeroTest(string plainTxt)
        {
            // SM2 加密含随机数，寻找 X 坐标带前导零字节的密文（单次命中概率约 1/256）；
            // 排除 X 第二字节为 04 的样本：剥离后与自带 04 前缀存在语法歧义，不在支持范围内
            string withLeadingZero = null;
            for (int i = 0; i < 5000 && withLeadingZero == null; i++)
            {
                var encrypted = SM2Util.Encrypt(plainTxt, generatePublicKey, Encoding.UTF8);
                if (encrypted.StartsWith("0400", StringComparison.OrdinalIgnoreCase) && !encrypted.StartsWith("040004", StringComparison.OrdinalIgnoreCase))
                {
                    withLeadingZero = encrypted;
                }
            }
            // 5000 次内未命中的概率约 (1 - 1/257)^5000 ≈ 2.7e-8，可视为必然命中
            Assert.NotNull(withLeadingZero);

            // 场景一：对端定长剥离（仅去掉 04 前缀），开头的 00 是 X 坐标的真实字节，必须保留
            Assert.Equal(plainTxt, SM2Util.Decrypt(withLeadingZero.Substring(2), generatePrivateKey, Encoding.UTF8));

            // 场景二：对端变长剥离（04 前缀连同 X 的一个前导零字节一并去掉），需经补零试探恢复
            Assert.Equal(plainTxt, SM2Util.Decrypt(withLeadingZero.Substring(4), generatePrivateKey, Encoding.UTF8));
        }

        [Fact]
        public void UtilDecryptInvalidStrippedCipherTest()
        {
            // 长度合法但内容无效的 hex 密文：所有补零候选均失败后，应抛出 InvalidCipherTextException
            var invalidCipher = new string('A', 216);
            Assert.Throws<InvalidCipherTextException>(() => SM2Util.Decrypt(invalidCipher, generatePrivateKey, Encoding.UTF8));
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void UtilDecryptStrippedYLeadingZeroTest(string plainTxt)
        {
            // 寻找 Y 坐标带前导零字节的密文（C1 布局：04 + X(64) + Y(64)，Y 从第 66 个 hex 字符开始）；
            // 排除 X 以 04 开头的样本：剥离后与自带 04 前缀存在语法歧义，不在支持范围内
            string withYLeadingZero = null;
            for (int i = 0; i < 5000 && withYLeadingZero == null; i++)
            {
                var encrypted = SM2Util.Encrypt(plainTxt, generatePublicKey, Encoding.UTF8);
                if (!encrypted.StartsWith("0404", StringComparison.OrdinalIgnoreCase)
                    && encrypted.Substring(2 + 64, 2).Equals("00", StringComparison.OrdinalIgnoreCase))
                {
                    withYLeadingZero = encrypted;
                }
            }
            // 单次命中概率约 1/257，5000 次内未命中的概率约 2.7e-8，可视为必然命中
            Assert.NotNull(withYLeadingZero);

            // 模拟对端变长剥离：去掉 04 前缀，并去掉 Y 坐标的一个前导零字节
            // （X 与 Y 的分割位置随之丢失，需靠二维补零试探恢复）
            // stripped = X(64) || Yr(62) || C3 || C2
            var stripped = withYLeadingZero.Substring(2, 64) + withYLeadingZero.Substring(2 + 64 + 2);
            var decrypted = SM2Util.Decrypt(stripped, generatePrivateKey, Encoding.UTF8);
            Assert.Equal(plainTxt, decrypted);
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void UtilRoundTripTest(string plainTxt)
        {
            var encrypted = SM2Util.Encrypt(plainTxt, generatePublicKey, Encoding.UTF8);
            var decrypted = SM2Util.Decrypt(encrypted, generatePrivateKey, Encoding.UTF8);
            Assert.Equal(plainTxt, decrypted);
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void UtilRoundTripWithGeneratedKeyTest(string plainTxt)
        {
            SM2Util.GenerateSM2KeyPair(true, out var privateKey, out var publicKey);
            var encrypted = SM2Util.Encrypt(plainTxt, publicKey, Encoding.UTF8);
            var decrypted = SM2Util.Decrypt(encrypted, privateKey, Encoding.UTF8);
            Assert.Equal(plainTxt, decrypted);
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void UtilRoundTripC1C2C3Test(string plainTxt)
        {
            var encrypted = SM2Util.Encrypt(plainTxt, generatePublicKey, Encoding.UTF8, Org.BouncyCastle.Crypto.Engines.SM2Engine.Mode.C1C2C3);
            var decrypted = SM2Util.Decrypt(encrypted, generatePrivateKey, Encoding.UTF8, Org.BouncyCastle.Crypto.Engines.SM2Engine.Mode.C1C2C3);
            Assert.Equal(plainTxt, decrypted);
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void UtilRoundTripBase64Test(string plainTxt)
        {
            SM2Util.GenerateSM2KeyPair(false, out var privateKey, out var publicKey);
            var message = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainTxt)); // isBase64Data=true 要求消息本身为 Base64
            var encrypted = SM2Util.Encrypt(message, publicKey, Encoding.UTF8, isBase64Key: true, isBase64Data: true);
            var decrypted = SM2Util.Decrypt(encrypted, privateKey, Encoding.UTF8, isBase64Key: true, IsBase64Data: true);
            Assert.Equal(plainTxt, decrypted);
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void UtilDecryptStrippedPrefixTest(string plainTxt)
        {
            // 模拟外部系统定长剥离 C1 的 04 前缀；
            // 跳过 X 坐标恰好以 04 开头的样本：剥离后与自带前缀存在语法歧义，不在支持范围内
            string encrypted;
            do
            {
                encrypted = SM2Util.Encrypt(plainTxt, generatePublicKey, Encoding.UTF8);
            } while (encrypted.StartsWith("0404", StringComparison.OrdinalIgnoreCase));

            var stripped = encrypted.Substring(2);
            var decrypted = SM2Util.Decrypt(stripped, generatePrivateKey, Encoding.UTF8);
            Assert.Equal(plainTxt, decrypted);
        }

        [Fact]
        public void UtilEncryptCompressedKeyDemoTest()
        {
            // 实证休眠 Bug：isPublicKeyCompressed=true 会剥离 04 前缀导致 DecodePoint 失败
            Assert.ThrowsAny<Exception>(() => SM2Util.Encrypt("test", generatePublicKey, Encoding.UTF8, isPublicKeyCompressed: true));
        }

        [Theory]
        [InlineData("e58d6c48d1c7b930a75be19f4e89d5632c6e630889727681a3376488146b1600f63e3945b4890a833f50710761e44069f21c2b1225614438eaa982c9e5266d10af29360a3a3e0e3b2922620c1dcb472c32f5b19fd62db04d58c2aa09e79ba870dbbbbc329f")]
        public void UtilDecryptTest(string encryptedTxt)
        {
            var plainTxt = SM2Util.Decrypt(encryptedTxt, "CAA77B5BAA6BAC42E3015C004DCC839A56C94D9F8965D0110FCE76E973F9EFE5", Encoding.UTF8);
            Assert.NotEmpty(plainTxt);
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void UtilSignAndVerifyTest(string message)
        {
            var signature = SM2Util.Sign(message, generatePrivateKey, Encoding.UTF8);
            Assert.NotEmpty(signature);
            Assert.Equal(128, signature.Length); // 裸 r||s 格式：32 + 32 字节 = 128 个十六进制字符
            Assert.True(SM2Util.VerifySign(message, signature, generatePublicKey, Encoding.UTF8));
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void UtilSignAndVerifyWithGeneratedKeyTest(string message)
        {
            SM2Util.GenerateSM2KeyPair(true, out var privateKey, out var publicKey);
            var signature = SM2Util.Sign(message, privateKey, Encoding.UTF8);
            Assert.True(SM2Util.VerifySign(message, signature, publicKey, Encoding.UTF8));
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void UtilSignAndVerifyWithUserIdTest(string message)
        {
            var signature = SM2Util.Sign(message, generatePrivateKey, Encoding.UTF8, userId: "custom-user-id");
            // 相同 userId 验签通过
            Assert.True(SM2Util.VerifySign(message, signature, generatePublicKey, Encoding.UTF8, userId: "custom-user-id"));
            // 缺省 userId（1234567812345678）与签名时不一致，验签必须失败
            Assert.False(SM2Util.VerifySign(message, signature, generatePublicKey, Encoding.UTF8));
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void UtilSignDerAndVerifyTest(string message)
        {
            var signature = SM2Util.Sign(message, generatePrivateKey, Encoding.UTF8, isDerSignature: true);
            Assert.StartsWith("30", signature); // DER SEQUENCE 起始标记
            Assert.True(SM2Util.VerifySign(message, signature, generatePublicKey, Encoding.UTF8));
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void UtilSignAndVerifyBase64Test(string message)
        {
            SM2Util.GenerateSM2KeyPair(false, out var privateKey, out var publicKey);
            var signature = SM2Util.Sign(message, privateKey, Encoding.UTF8, isBase64Key: true);
            var base64Signature = Convert.ToBase64String(Hex.Decode(signature));
            Assert.True(SM2Util.VerifySign(message, base64Signature, publicKey, Encoding.UTF8, isBase64Key: true, isBase64Signature: true));
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void UtilVerifyTamperedMessageTest(string message)
        {
            var signature = SM2Util.Sign(message, generatePrivateKey, Encoding.UTF8);
            // 篡改消息后验签必须失败
            Assert.False(SM2Util.VerifySign(message + "x", signature, generatePublicKey, Encoding.UTF8));
        }

        [Fact]
        public void UtilVerifyInvalidSignatureTest()
        {
            // 长度合法但内容无效的裸格式签名：验签应返回 false 而非抛异常
            var invalidSignature = new string('A', 128);
            Assert.False(SM2Util.VerifySign("Gxrc@666", invalidSignature, generatePublicKey, Encoding.UTF8));
        }

        [Fact]
        public void UtilVerifyMalformedSignatureTest()
        {
            // 非 hex 签名数据：按验签失败处理，返回 false 而非抛异常
            Assert.False(SM2Util.VerifySign("Gxrc@666", "not-hex-signature", generatePublicKey, Encoding.UTF8));
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void UtilSignVerifiedByBouncyCastleTest(string message)
        {
            // 交叉验证一：SM2Util 的 DER 输出由 BouncyCastle 原生 SM2Signer 验证（显式国标默认 userId）
            var derSignature = Hex.Decode(SM2Util.Sign(message, generatePrivateKey, Encoding.UTF8, isDerSignature: true));
            var curve = GMNamedCurves.GetByName("sm2p256v1");
            var publicKeyParameters = new ECPublicKeyParameters("EC", curve.Curve.DecodePoint(Hex.Decode(generatePublicKey)), new ECDomainParameters(curve));
            var verifier = new SM2Signer();
            verifier.Init(false, new ParametersWithID(publicKeyParameters, Encoding.UTF8.GetBytes("1234567812345678")));
            var messageBytes = Encoding.UTF8.GetBytes(message);
            verifier.BlockUpdate(messageBytes, 0, messageBytes.Length);
            Assert.True(verifier.VerifySignature(derSignature));
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void UtilVerifyBouncyCastleSignatureTest(string message)
        {
            // 交叉验证二：BouncyCastle 原生签名（缺省 userId、DER 输出）由 SM2Util 验签（自动识别 DER 格式）
            var curve = GMNamedCurves.GetByName("sm2p256v1");
            var domain = new ECDomainParameters(curve);
            var privateKeyParameters = new ECPrivateKeyParameters(new Org.BouncyCastle.Math.BigInteger(1, Hex.Decode(generatePrivateKey)), domain);
            var signer = new SM2Signer();
            signer.Init(true, new ParametersWithRandom(privateKeyParameters));
            var messageBytes = Encoding.UTF8.GetBytes(message);
            signer.BlockUpdate(messageBytes, 0, messageBytes.Length);
            var signatureHex = Hex.ToHexString(signer.GenerateSignature());
            Assert.True(SM2Util.VerifySign(message, signatureHex, generatePublicKey, Encoding.UTF8));
        }

        [Theory]
        [InlineData("3046022100cce97958385426fa8528d7d39bb75f98259ca24bcace4c3c1b1fd3ad631af371022100cafbf4f4c9d01bdd76c5305fff80d2d39a24fe11c59dbd6bf757e3b7a884c839")]
        public void UtilVerifySignWithExistingDataTest(string signature)
        {
            // 既有 SM2CryptoUtil 生成的真实 DER 签名数据（缺省 userId、国标曲线），经 SM2Util 验签应通过
            var message = "{\"aac002\":\"45012119930940463X\",\"aac003\":\"潘某某\",\"aac045\":\"18476927841\"}";
            Assert.True(SM2Util.VerifySign(message, signature, generatePublicKey, Encoding.UTF8));
        }

        [Fact]
        public void UtilEncryptArgumentValidationTest()
        {
            // message=null → ArgumentNullException（带参数名）
            var ex1 = Assert.Throws<ArgumentNullException>(() => SM2Util.Encrypt(null, generatePublicKey, Encoding.UTF8));
            Assert.Equal("message", ex1.ParamName);
            // publicKey=null → ArgumentNullException（此前为 NullReferenceException）
            var ex2 = Assert.Throws<ArgumentNullException>(() => SM2Util.Encrypt("msg", null, Encoding.UTF8));
            Assert.Equal("publicKey", ex2.ParamName);
            // publicKey=空白 → ArgumentException
            var ex3 = Assert.Throws<ArgumentException>(() => SM2Util.Encrypt("msg", "   ", Encoding.UTF8));
            Assert.Equal("publicKey", ex3.ParamName);
            // encoding=null → ArgumentNullException（此前为 NullReferenceException）
            var ex4 = Assert.Throws<ArgumentNullException>(() => SM2Util.Encrypt("msg", generatePublicKey, null));
            Assert.Equal("encoding", ex4.ParamName);
        }

        [Fact]
        public void UtilDecryptArgumentValidationTest()
        {
            // encrypted=null → ArgumentNullException（此前为 NullReferenceException）
            var ex1 = Assert.Throws<ArgumentNullException>(() => SM2Util.Decrypt(null, generatePrivateKey, Encoding.UTF8));
            Assert.Equal("encrypted", ex1.ParamName);
            // encrypted=空白 → ArgumentException
            var ex2 = Assert.Throws<ArgumentException>(() => SM2Util.Decrypt("   ", generatePrivateKey, Encoding.UTF8));
            Assert.Equal("encrypted", ex2.ParamName);
            // key=null → ArgumentNullException
            var ex3 = Assert.Throws<ArgumentNullException>(() => SM2Util.Decrypt("04AB", null, Encoding.UTF8));
            Assert.Equal("key", ex3.ParamName);
            // key=空白 → ArgumentException
            var ex4 = Assert.Throws<ArgumentException>(() => SM2Util.Decrypt("04AB", " ", Encoding.UTF8));
            Assert.Equal("key", ex4.ParamName);
            // encoding=null → ArgumentNullException（此前为 NullReferenceException）
            var ex5 = Assert.Throws<ArgumentNullException>(() => SM2Util.Decrypt("04AB", generatePrivateKey, null));
            Assert.Equal("encoding", ex5.ParamName);
        }

        [Fact]
        public void UtilSignArgumentValidationTest()
        {
            var ex1 = Assert.Throws<ArgumentNullException>(() => SM2Util.Sign(null, generatePrivateKey, Encoding.UTF8));
            Assert.Equal("message", ex1.ParamName);
            var ex2 = Assert.Throws<ArgumentNullException>(() => SM2Util.Sign("msg", null, Encoding.UTF8));
            Assert.Equal("privateKey", ex2.ParamName);
            var ex3 = Assert.Throws<ArgumentException>(() => SM2Util.Sign("msg", " ", Encoding.UTF8));
            Assert.Equal("privateKey", ex3.ParamName);
            var ex4 = Assert.Throws<ArgumentNullException>(() => SM2Util.Sign("msg", generatePrivateKey, null));
            Assert.Equal("encoding", ex4.ParamName);
        }

        [Fact]
        public void UtilVerifySignArgumentValidationTest()
        {
            var ex1 = Assert.Throws<ArgumentNullException>(() => SM2Util.VerifySign(null, "sig", generatePublicKey, Encoding.UTF8));
            Assert.Equal("message", ex1.ParamName);
            var ex2 = Assert.Throws<ArgumentNullException>(() => SM2Util.VerifySign("msg", null, generatePublicKey, Encoding.UTF8));
            Assert.Equal("signature", ex2.ParamName);
            var ex3 = Assert.Throws<ArgumentException>(() => SM2Util.VerifySign("msg", " ", generatePublicKey, Encoding.UTF8));
            Assert.Equal("signature", ex3.ParamName);
            var ex4 = Assert.Throws<ArgumentNullException>(() => SM2Util.VerifySign("msg", "sig", null, Encoding.UTF8));
            Assert.Equal("publicKey", ex4.ParamName);
            var ex5 = Assert.Throws<ArgumentNullException>(() => SM2Util.VerifySign("msg", "sig", generatePublicKey, null));
            Assert.Equal("encoding", ex5.ParamName);
        }

    }
}
