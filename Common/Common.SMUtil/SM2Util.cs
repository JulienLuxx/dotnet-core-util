using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math;

namespace Common.SMUtil
{
    public static class SM2Util
    {
        /// <summary>
        /// Uncompressed EC point encoding prefix (hex form)
        /// 未压缩椭圆曲线点编码的 04 前缀（十六进制）
        /// </summary>
        private const string UncompressedPointPrefix = "04";

        /// <summary>
        /// Hex char length of an SM2 coordinate (32 bytes)
        /// SM2 曲线坐标长度（32 字节）对应的十六进制字符数
        /// </summary>
        private const int CoordinateHexLength = 64;

        /// <summary>
        /// Byte length of an SM2 coordinate
        /// SM2 曲线坐标长度（字节）
        /// </summary>
        private const int CoordinateByteLength = CoordinateHexLength / 2;

        /// <summary>
        /// Hex char length of the SM3 digest (C3 part, 32 bytes)
        /// SM3 摘要（C3 部分，32 字节）对应的十六进制字符数
        /// </summary>
        private const int DigestHexLength = 64;

        /// <summary>
        /// Default SM2 user identifier defined by GB/T 32918
        /// 国标 GB/T 32918 定义的 SM2 默认用户标识
        /// </summary>
        private const string DefaultUserId = "1234567812345678";

        /// <summary>
        /// Byte length of each SM2 signature component (r or s)
        /// SM2 签名分量（r 或 s）的字节长度
        /// </summary>
        private const int SignatureComponentByteLength = 32;

        /// <summary>
        /// Generate an SM2 key pair, with the key pair encoded using Base64 or hexadecimal.
        /// 生成 SM2 密钥对，密钥对使用 Base64或十六进制进行编码
        /// </summary>
        /// <param name="isHex"></param>
        /// <param name="privateKey"></param>
        /// <param name="publicKey"></param>
        public static void GenerateSM2KeyPair(bool isHex, out string privateKey, out string publicKey)
        {
            // 获取 SM2 曲线参数
            X9ECParameters curve = ECNamedCurveTable.GetByName("sm2p256v1");
            KeyGenerationParameters parameters = new ECKeyGenerationParameters(new ECDomainParameters(curve), new SecureRandom());
            // 创建 SM2 密钥对生成器
            ECKeyPairGenerator generator = new ECKeyPairGenerator();
            generator.Init(parameters);
            // 创建密钥对
            var keyPair = generator.GenerateKeyPair();
            // 私钥
            ECPrivateKeyParameters privateKeyParameters = (ECPrivateKeyParameters)keyPair.Private;
            privateKey = isHex ? Hex.ToHexString(privateKeyParameters.D.ToByteArrayUnsigned()) : Base64.ToBase64String(privateKeyParameters.D.ToByteArrayUnsigned());
            // 公钥 - 手动拼接非压缩格式（0x04 + X(32B) + Y(32B)），补齐前导零避免缺位
            ECPublicKeyParameters publicKeyParameters = (ECPublicKeyParameters)keyPair.Public;
            var q = publicKeyParameters.Q;
            byte[] xBytes = q.AffineXCoord.ToBigInteger().ToByteArrayUnsigned();
            byte[] yBytes = q.AffineYCoord.ToBigInteger().ToByteArrayUnsigned();
            byte[] pubKeyBytes = new byte[65];
            pubKeyBytes[0] = 0x04;
            Array.Copy(xBytes, 0, pubKeyBytes, 33 - xBytes.Length, xBytes.Length);
            Array.Copy(yBytes, 0, pubKeyBytes, 65 - yBytes.Length, yBytes.Length);
            publicKey = isHex ? Hex.ToHexString(pubKeyBytes) : Base64.ToBase64String(pubKeyBytes);
        }

        /// <summary>
        /// SM2 Public Key Encrypt
        /// SM2 公钥加密
        /// </summary>
        /// <param name="message"></param>
        /// <param name="key"></param>
        /// <param name="mode"></param>
        /// <param name="isHex"></param>
        /// <returns></returns>
        private static byte[] Encrypt(string message, string key, SM2Engine.Mode mode = SM2Engine.Mode.C1C3C2, bool isHex = true)
        {
            // 获取 SM2 曲线参数
            X9ECParameters curve = GMNamedCurves.GetByName("sm2p256v1");
            var keyBytes = isHex ? Hex.Decode(key) : Base64.Decode(key);
            ECPoint q = curve.Curve.DecodePoint(keyBytes);
            ECDomainParameters domain = new ECDomainParameters(curve);
            ECPublicKeyParameters pubk = new ECPublicKeyParameters("EC", q, domain);
            // 创建SM2加密器
            SM2Engine sm2Engine = new SM2Engine(mode);
            sm2Engine.Init(true, new ParametersWithRandom(pubk, new SecureRandom()));
            // 将原始数据转换为字节数组
            byte[] dataBytes = Encoding.UTF8.GetBytes(message);
            // 执行加密操作
            byte[] encryptedData = sm2Engine.ProcessBlock(dataBytes, 0, dataBytes.Length);
            return encryptedData;
        }

        private static byte[] Encrypt(byte[] dataBytes, byte[] keyBytes, SM2Engine.Mode mode)
        {
            X9ECParameters curve = GMNamedCurves.GetByName("sm2p256v1");
            ECPoint q = curve.Curve.DecodePoint(keyBytes);
            ECDomainParameters domain = new ECDomainParameters(curve);
            ECPublicKeyParameters publicKeyParam = new ECPublicKeyParameters("EC", q, domain);
            SM2Engine sm2Engine = new SM2Engine(mode);
            sm2Engine.Init(true, new ParametersWithRandom(publicKeyParam, new SecureRandom()));
            byte[] encryptedData = sm2Engine.ProcessBlock(dataBytes, 0, dataBytes.Length);
            return encryptedData;
        }

        /// <summary>
        ///  SM2 Public Key Encrypt
        /// SM2 公钥加密
        /// </summary>
        /// <param name="message"></param>
        /// <param name="publicKey"></param>
        /// <param name="encoding"></param>
        /// <param name="mode"></param>
        /// <param name="isPublicKeyCompressed"></param>
        /// <param name="isBase64Key"></param>
        /// <param name="isBase64Data"></param>
        /// <returns></returns>
        public static string Encrypt(string message, string publicKey, Encoding encoding, SM2Engine.Mode mode = SM2Engine.Mode.C1C3C2, bool isPublicKeyCompressed = false, bool isBase64Key = false, bool isBase64Data = false)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "加密消息不能为空 / Message cannot be null");
            }
            if (publicKey == null)
            {
                throw new ArgumentNullException(nameof(publicKey), "公钥不能为空 / Public key cannot be null");
            }
            if (string.IsNullOrWhiteSpace(publicKey))
            {
                throw new ArgumentException("公钥不能为空串或纯空白字符 / Public key cannot be empty or whitespace", nameof(publicKey));
            }
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding), "文本编码不能为空 / Encoding cannot be null");
            }
            if (isPublicKeyCompressed && !isBase64Key && publicKey.StartsWith("04"))
            {
                publicKey = publicKey.Remove(0, 2);
            }
            var dataBytes = isBase64Data ? Base64.Decode(message) : encoding.GetBytes(message);
            var keyBytes = isBase64Key ? Base64.Decode(publicKey) : Hex.Decode(publicKey);
            byte[] encryptedData = Encrypt(dataBytes, keyBytes, mode);
            var encrypted = isBase64Data ? Base64.ToBase64String(encryptedData) : Hex.ToHexString(encryptedData);
            return encrypted;
        }

        /// <summary>
        /// SM2 Private Key Decrypt
        /// SM2 私钥解密
        /// </summary>
        /// <param name="message"></param>
        /// <param name="key"></param>
        /// <param name="mode"></param>
        /// <param name="isHex"></param>
        /// <returns></returns>
        private static string Decrypt(string message, string key, SM2Engine.Mode mode= SM2Engine.Mode.C1C3C2 , bool isHex = true)
        {
            // 获取 SM2 曲线参数
            X9ECParameters curve = GMNamedCurves.GetByName("sm2p256v1");
            ECDomainParameters domain = new ECDomainParameters(curve);
            var keyBytes =Hex.Decode(key) /*isHex ? Hex.Decode(key) : Base64.Decode(key)*/;
            BigInteger d = new BigInteger(1, keyBytes);
            ECPrivateKeyParameters prik = new ECPrivateKeyParameters(d, domain);
            // 创建SM2加密器
            SM2Engine sm2Engine = new SM2Engine(mode);
            sm2Engine.Init(false, prik);
            byte[] encryptedData =isHex ? Hex.Decode(message) : Base64.Decode(message);
            // 执行解密操作
            byte[] decryptedData = sm2Engine.ProcessBlock(encryptedData, 0, encryptedData.Length);
            // 将解密结果转换为字符串
            return Encoding.UTF8.GetString(decryptedData);
        }

        private static byte[] Decrypt(byte[] encryptData, byte[] keyBytes, SM2Engine.Mode mode)
        {
            X9ECParameters curve = GMNamedCurves.GetByName("sm2p256v1");
            ECDomainParameters domain = new ECDomainParameters(curve);
            BigInteger d = new BigInteger(1, keyBytes);
            ECPrivateKeyParameters privateKeyParam = new ECPrivateKeyParameters(d, domain);
            SM2Engine sm2Engine = new SM2Engine(mode);
            sm2Engine.Init(false, privateKeyParam);
            byte[] decryptedData = sm2Engine.ProcessBlock(encryptData, 0, encryptData.Length);
            return decryptedData;
        }

        /// <summary>
        /// SM2 Private Key Decrypt
        /// SM2 私钥解密
        /// <para>兼容不带 04 前缀的密文：按 X/Y 两侧补零长度二维试探，恢复被剥离的 C1 前缀及两侧坐标前导零</para>
        /// </summary>
        /// <param name="encrypted"></param>
        /// <param name="key"></param>
        /// <param name="encoding"></param>
        /// <param name="mode"></param>
        /// <param name="isBase64Key"></param>
        /// <param name="IsBase64Data"></param>
        /// <returns></returns>
        public static string Decrypt(string encrypted, string key, Encoding encoding, SM2Engine.Mode mode = SM2Engine.Mode.C1C3C2, bool isBase64Key = false, bool IsBase64Data = false)
        {
            if (encrypted == null)
            {
                throw new ArgumentNullException(nameof(encrypted), "密文不能为空 / Cipher text cannot be null");
            }
            if (string.IsNullOrWhiteSpace(encrypted))
            {
                throw new ArgumentException("密文不能为空串或纯空白字符 / Cipher text cannot be empty or whitespace", nameof(encrypted));
            }
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "私钥不能为空 / Private key cannot be null");
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("私钥不能为空串或纯空白字符 / Private key cannot be empty or whitespace", nameof(key));
            }
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding), "文本编码不能为空 / Encoding cannot be null");
            }
            var keyBytes = isBase64Key ? Base64.Decode(key) : Hex.Decode(key);

            // 密文自带 04 前缀，或密文为 Base64 格式时，直接按原格式解密
            if (IsBase64Data || encrypted.StartsWith(UncompressedPointPrefix))
            {
                byte[] encryptData = IsBase64Data ? Base64.Decode(encrypted) : Hex.Decode(encrypted);
                byte[] decryptedBytes = Decrypt(encryptData, keyBytes, mode);
                return encoding.GetString(decryptedBytes);
            }

            // 非 04 开头的 hex 密文：对端已剥离 C1 的 04 前缀（定长剥离），
            // 甚至可能按 BigInteger 最短形式连同 X / Y 坐标的前导零字节一并剥离（变长剥离）。
            // 由于 X/Y 的分割位置与两侧剥离字节数均无法从语法上判断，此处做二维试探：
            // 对（X 侧补零数, Y 侧补零数）的每种组合重构 C1 并尝试解密，
            // 错误候选会因点不在曲线上或 C3 哈希校验失败被排除（误通过概率约 2^-128，可忽略），
            // 首个解密成功的组合即为正确密文。
            // 遍历顺序：Y 侧补零数在外层递增，确保「未剥离 / 仅 X 侧剥离」的常见场景最先命中。
            if (encrypted.Length % 2 != 0)
            {
                throw new ArgumentException("密文十六进制长度必须为偶数 / Cipher hex length must be even", nameof(encrypted));
            }
            for (int yPadBytes = 0; yPadBytes <= CoordinateByteLength; yPadBytes++)
            {
                int yPartHexLength = CoordinateHexLength - 2 * yPadBytes;
                for (int xPadBytes = 0; xPadBytes <= CoordinateByteLength; xPadBytes++)
                {
                    int xPartHexLength = CoordinateHexLength - 2 * xPadBytes;
                    if (encrypted.Length < xPartHexLength + yPartHexLength + DigestHexLength)
                    {
                        // 剥离坐标后剩余部分不足以容纳 C3，该组合不可能
                        continue;
                    }
                    var candidate = string.Concat(
                        UncompressedPointPrefix,
                        new string('0', 2 * xPadBytes),
                        encrypted.Substring(0, xPartHexLength),
                        new string('0', 2 * yPadBytes),
                        encrypted.Substring(xPartHexLength, yPartHexLength),
                        encrypted.Substring(xPartHexLength + yPartHexLength));
                    byte[] encryptData = Hex.Decode(candidate);
                    try
                    {
                        byte[] decryptedBytes = Decrypt(encryptData, keyBytes, mode);
                        return encoding.GetString(decryptedBytes);
                    }
                    catch (Exception ex) when (ex is InvalidCipherTextException || ex is ArgumentException)
                    {
                        // 当前补零组合不正确，继续试探下一档
                    }
                }
            }

            throw new InvalidCipherTextException("invalid cipher text / 补零试探解密均失败，密文无效或与私钥不配对");
        }

        /// <summary>
        ///  SM2 Private Key Sign
        ///  <para>使用私钥对消息签名；默认输出裸 r||s 十六进制签名（128 个字符，与 hutool / sm-crypto 等主流实现一致），
        ///  置 isDerSignature=true 时输出 ASN.1 DER 格式（与 BouncyCastle 原生输出一致）；
        ///  未指定 userId 时使用国标默认标识 1234567812345678（按 UTF-8 编码）</para>
        /// </summary>
        /// <param name="message">Message / 待签名消息</param>
        /// <param name="privateKey">PrivateKey / 私钥字符串（十六进制或 Base64，与 isBase64Key 对应）</param>
        /// <param name="encoding">TextEncoding / 消息文本编码，验签时需使用相同编码</param>
        /// <param name="userId">UserId / 用户标识（按 UTF-8 编码），为 null 时使用国标默认 1234567812345678</param>
        /// <param name="isBase64Key">IsBase64Key / 私钥是否为 Base64 格式</param>
        /// <param name="isDerSignature">IsDerSignature / 是否输出 ASN.1 DER 格式签名</param>
        /// <returns>Signature / 十六进制签名字符串</returns>
        public static string Sign(string message, string privateKey, Encoding encoding, string userId = null, bool isBase64Key = false, bool isDerSignature = false)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "签名消息不能为空 / Message cannot be null");
            }
            if (privateKey == null)
            {
                throw new ArgumentNullException(nameof(privateKey), "私钥不能为空 / Private key cannot be null");
            }
            if (string.IsNullOrWhiteSpace(privateKey))
            {
                throw new ArgumentException("私钥不能为空串或纯空白字符 / Private key cannot be empty or whitespace", nameof(privateKey));
            }
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding), "文本编码不能为空 / Encoding cannot be null");
            }

            byte[] keyBytes = isBase64Key ? Base64.Decode(privateKey) : Hex.Decode(privateKey);
            X9ECParameters curve = GMNamedCurves.GetByName("sm2p256v1");
            ECDomainParameters domain = new ECDomainParameters(curve);
            ECPrivateKeyParameters privateKeyParameters = new ECPrivateKeyParameters(new BigInteger(1, keyBytes), domain);

            SM2Signer signer = new SM2Signer();
            signer.Init(true, new ParametersWithID(new ParametersWithRandom(privateKeyParameters), Encoding.UTF8.GetBytes(userId ?? DefaultUserId)));
            byte[] messageBytes = encoding.GetBytes(message);
            signer.BlockUpdate(messageBytes, 0, messageBytes.Length);
            byte[] derSignature = signer.GenerateSignature();
            return isDerSignature ? Hex.ToHexString(derSignature) : Hex.ToHexString(ConvertDerSignatureToPlain(derSignature));
        }

        /// <summary>
        ///  SM2 Public Key Verify Sign
        ///  <para>使用公钥验证签名；自动兼容裸 r||s 与 ASN.1 DER 两种签名格式；
        ///  签名或公钥数据非法、校验不通过均返回 false；未指定 userId 时使用国标默认标识 1234567812345678</para>
        /// </summary>
        /// <param name="message">Message / 待验签消息，需与签名时使用的消息一致</param>
        /// <param name="signature">Signature / 签名字符串（裸 r||s 或 DER 的十六进制，或 Base64，与 isBase64Signature 对应）</param>
        /// <param name="publicKey">PublicKey / 公钥字符串（十六进制或 Base64，与 isBase64Key 对应）</param>
        /// <param name="encoding">TextEncoding / 消息文本编码，需与签名时使用的编码一致</param>
        /// <param name="userId">UserId / 用户标识（按 UTF-8 编码），需与签名时一致</param>
        /// <param name="isBase64Key">IsBase64Key / 公钥是否为 Base64 格式</param>
        /// <param name="isBase64Signature">IsBase64Signature / 签名是否为 Base64 格式</param>
        /// <returns>IsVerified / 验签是否通过</returns>
        public static bool VerifySign(string message, string signature, string publicKey, Encoding encoding, string userId = null, bool isBase64Key = false, bool isBase64Signature = false)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "验签消息不能为空 / Message cannot be null");
            }
            if (signature == null)
            {
                throw new ArgumentNullException(nameof(signature), "签名不能为空 / Signature cannot be null");
            }
            if (string.IsNullOrWhiteSpace(signature))
            {
                throw new ArgumentException("签名不能为空串或纯空白字符 / Signature cannot be empty or whitespace", nameof(signature));
            }
            if (publicKey == null)
            {
                throw new ArgumentNullException(nameof(publicKey), "公钥不能为空 / Public key cannot be null");
            }
            if (string.IsNullOrWhiteSpace(publicKey))
            {
                throw new ArgumentException("公钥不能为空串或纯空白字符 / Public key cannot be empty or whitespace", nameof(publicKey));
            }
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding), "文本编码不能为空 / Encoding cannot be null");
            }

            try
            {
                byte[] keyBytes = isBase64Key ? Base64.Decode(publicKey) : Hex.Decode(publicKey);
                X9ECParameters curve = GMNamedCurves.GetByName("sm2p256v1");
                ECDomainParameters domain = new ECDomainParameters(curve);
                ECPublicKeyParameters publicKeyParameters = new ECPublicKeyParameters("EC", curve.Curve.DecodePoint(keyBytes), domain);

                byte[] messageBytes = encoding.GetBytes(message);
                byte[] signatureBytes = isBase64Signature ? Base64.Decode(signature) : Hex.Decode(signature);

                // 64 字节签名优先按裸 r||s 格式校验
                if (signatureBytes.Length == 2 * SignatureComponentByteLength
                    && VerifySignCore(messageBytes, ConvertPlainSignatureToDer(signatureBytes), publicKeyParameters, userId))
                {
                    return true;
                }
                // 回退按 DER 格式校验（覆盖 DER 签名长度恰为 64 字节的罕见场景）
                return VerifySignCore(messageBytes, signatureBytes, publicKeyParameters, userId);
            }
            catch (Exception ex) when (ex is System.IO.IOException || ex is ArgumentException || ex is InvalidCastException || ex is FormatException)
            {
                // 签名或公钥数据格式非法，视为验签失败
                return false;
            }
        }

        /// <summary>
        /// Verify signature core with DER format signature
        /// 使用 DER 格式签名执行验签核心逻辑
        /// </summary>
        private static bool VerifySignCore(byte[] messageBytes, byte[] derSignature, ECPublicKeyParameters publicKeyParameters, string userId)
        {
            SM2Signer signer = new SM2Signer();
            signer.Init(false, new ParametersWithID(publicKeyParameters, Encoding.UTF8.GetBytes(userId ?? DefaultUserId)));
            signer.BlockUpdate(messageBytes, 0, messageBytes.Length);
            return signer.VerifySignature(derSignature);
        }

        /// <summary>
        /// Convert DER signature to plain r||s format (each component padded to 32 bytes)
        /// 将 DER 格式签名转换为裸 r||s 格式（各分量补齐 32 字节）
        /// </summary>
        private static byte[] ConvertDerSignatureToPlain(byte[] derSignature)
        {
            Asn1Sequence sequence = (Asn1Sequence)Asn1Object.FromByteArray(derSignature);
            BigInteger r = ((DerInteger)sequence[0]).Value;
            BigInteger s = ((DerInteger)sequence[1]).Value;
            byte[] plainSignature = new byte[2 * SignatureComponentByteLength];
            Array.Copy(ToUnsignedByteArrayPadded(r, SignatureComponentByteLength), 0, plainSignature, 0, SignatureComponentByteLength);
            Array.Copy(ToUnsignedByteArrayPadded(s, SignatureComponentByteLength), 0, plainSignature, SignatureComponentByteLength, SignatureComponentByteLength);
            return plainSignature;
        }

        /// <summary>
        /// Convert BigInteger to unsigned byte array padded to the given length
        /// 将 BigInteger 转换为补齐至指定长度的无符号字节数组
        /// </summary>
        private static byte[] ToUnsignedByteArrayPadded(BigInteger value, int length)
        {
            byte[] bytes = value.ToByteArrayUnsigned();
            byte[] padded = new byte[length];
            Array.Copy(bytes, 0, padded, length - bytes.Length, bytes.Length);
            return padded;
        }

        /// <summary>
        /// Convert plain r||s signature to DER format
        /// 将裸 r||s 格式签名转换为 DER 格式
        /// </summary>
        private static byte[] ConvertPlainSignatureToDer(byte[] plainSignature)
        {
            BigInteger r = new BigInteger(1, plainSignature, 0, SignatureComponentByteLength);
            BigInteger s = new BigInteger(1, plainSignature, SignatureComponentByteLength, SignatureComponentByteLength);
            return new DerSequence(new DerInteger(r), new DerInteger(s)).GetDerEncoded();
        }
    }
}
