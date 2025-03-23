using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
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
            // 公钥
            ECPublicKeyParameters publicKeyParameters = (ECPublicKeyParameters)keyPair.Public;
            publicKey = isHex ? Hex.ToHexString(publicKeyParameters.Q.GetEncoded()) : Base64.ToBase64String(publicKeyParameters.Q.GetEncoded());
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
            if (!encrypted.StartsWith("04") && !IsBase64Data)
            {
                if (encrypted.StartsWith("00"))
                {
                    encrypted.Remove(0, 2);
                }
                encrypted = $"04{encrypted}";
            }
            var keyBytes = isBase64Key ? Base64.Decode(key) : Hex.Decode(key);
            byte[] encryptData = IsBase64Data ? Base64.Decode(encrypted) : Hex.Decode(encrypted);
            byte[] decryptedBytes = Decrypt(encryptData, keyBytes, mode);
            var decrypted = encoding.GetString(decryptedBytes);
            return decrypted;
        }
    }
}
