using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.SMUtil
{
    internal class SM4_Context
    {
        public int mode;

        public long[] sk;

        public bool isPadding;

        public SM4_Context()
        {
            mode = 1;
            isPadding = true;
            sk = new long[32];
        }
    }

    internal class SM4
    {
        public const int SM4_ENCRYPT = 1;

        public const int SM4_DECRYPT = 0;

        public byte[] SboxTable = new byte[256]
        {
            214, 144, 233, 254, 204, 225, 61, 183, 22, 182,
            20, 194, 40, 251, 44, 5, 43, 103, 154, 118,
            42, 190, 4, 195, 170, 68, 19, 38, 73, 134,
            6, 153, 156, 66, 80, 244, 145, 239, 152, 122,
            51, 84, 11, 67, 237, 207, 172, 98, 228, 179,
            28, 169, 201, 8, 232, 149, 128, 223, 148, 250,
            117, 143, 63, 166, 71, 7, 167, 252, 243, 115,
            23, 186, 131, 89, 60, 25, 230, 133, 79, 168,
            104, 107, 129, 178, 113, 100, 218, 139, 248, 235,
            15, 75, 112, 86, 157, 53, 30, 36, 14, 94,
            99, 88, 209, 162, 37, 34, 124, 59, 1, 33,
            120, 135, 212, 0, 70, 87, 159, 211, 39, 82,
            76, 54, 2, 231, 160, 196, 200, 158, 234, 191,
            138, 210, 64, 199, 56, 181, 163, 247, 242, 206,
            249, 97, 21, 161, 224, 174, 93, 164, 155, 52,
            26, 85, 173, 147, 50, 48, 245, 140, 177, 227,
            29, 246, 226, 46, 130, 102, 202, 96, 192, 41,
            35, 171, 13, 83, 78, 111, 213, 219, 55, 69,
            222, 253, 142, 47, 3, 255, 106, 114, 109, 108,
            91, 81, 141, 27, 175, 146, 187, 221, 188, 127,
            17, 217, 92, 65, 31, 16, 90, 216, 10, 193,
            49, 136, 165, 205, 123, 189, 45, 116, 208, 18,
            184, 229, 180, 176, 137, 105, 151, 74, 12, 150,
            119, 126, 101, 185, 241, 9, 197, 110, 198, 132,
            24, 240, 125, 236, 58, 220, 77, 32, 121, 238,
            95, 62, 215, 203, 57, 72
        };

        public uint[] FK = new uint[4] { 2746333894u, 1453994832u, 1736282519u, 2993693404u };

        public uint[] CK = new uint[32]
        {
            462357u, 472066609u, 943670861u, 1415275113u, 1886879365u, 2358483617u, 2830087869u, 3301692121u, 3773296373u, 4228057617u,
            404694573u, 876298825u, 1347903077u, 1819507329u, 2291111581u, 2762715833u, 3234320085u, 3705924337u, 4177462797u, 337322537u,
            808926789u, 1280531041u, 1752135293u, 2223739545u, 2695343797u, 3166948049u, 3638552301u, 4110090761u, 269950501u, 741554753u,
            1213159005u, 1684763257u
        };

        private long GET_ULONG_BE(byte[] b, int i)
        {
            return ((long)(b[i] & 0xFF) << 24) | ((b[i + 1] & 0xFF) << 16) | ((b[i + 2] & 0xFF) << 8) | (b[i + 3] & 0xFF & 0xFFFFFFFFu);
        }

        private void PUT_ULONG_BE(long n, byte[] b, int i)
        {
            b[i] = (byte)(0xFF & (n >> 24));
            b[i + 1] = (byte)(0xFF & (n >> 16));
            b[i + 2] = (byte)(0xFF & (n >> 8));
            b[i + 3] = (byte)(0xFF & n);
        }

        private long SHL(long x, int n)
        {
            return (x & 0xFFFFFFFFu) << n;
        }

        private long ROTL(long x, int n)
        {
            return SHL(x, n) | (x >> 32 - n);
        }

        private void SWAP(long[] sk, int i)
        {
            long num = sk[i];
            sk[i] = sk[31 - i];
            sk[31 - i] = num;
        }

        private byte sm4Sbox(byte inch)
        {
            int num = inch & 0xFF;
            return SboxTable[num];
        }

        private long sm4Lt(long ka)
        {
            long num = 0L;
            byte[] array = new byte[4];
            byte[] array2 = new byte[4];
            PUT_ULONG_BE(ka, array, 0);
            array2[0] = sm4Sbox(array[0]);
            array2[1] = sm4Sbox(array[1]);
            array2[2] = sm4Sbox(array[2]);
            array2[3] = sm4Sbox(array[3]);
            num = GET_ULONG_BE(array2, 0);
            return num ^ ROTL(num, 2) ^ ROTL(num, 10) ^ ROTL(num, 18) ^ ROTL(num, 24);
        }

        private long sm4F(long x0, long x1, long x2, long x3, long rk)
        {
            return x0 ^ sm4Lt(x1 ^ x2 ^ x3 ^ rk);
        }

        private long sm4CalciRK(long ka)
        {
            long num = 0L;
            byte[] array = new byte[4];
            byte[] array2 = new byte[4];
            PUT_ULONG_BE(ka, array, 0);
            array2[0] = sm4Sbox(array[0]);
            array2[1] = sm4Sbox(array[1]);
            array2[2] = sm4Sbox(array[2]);
            array2[3] = sm4Sbox(array[3]);
            num = GET_ULONG_BE(array2, 0);
            return num ^ ROTL(num, 13) ^ ROTL(num, 23);
        }

        private void sm4_setkey(long[] SK, byte[] key)
        {
            long[] array = new long[4];
            long[] array2 = new long[36];
            int i = 0;
            array[0] = GET_ULONG_BE(key, 0);
            array[1] = GET_ULONG_BE(key, 4);
            array[2] = GET_ULONG_BE(key, 8);
            array[3] = GET_ULONG_BE(key, 12);
            array2[0] = array[0] ^ FK[0];
            array2[1] = array[1] ^ FK[1];
            array2[2] = array[2] ^ FK[2];
            array2[3] = array[3] ^ FK[3];
            for (; i < 32; i++)
            {
                array2[i + 4] = array2[i] ^ sm4CalciRK(array2[i + 1] ^ array2[i + 2] ^ array2[i + 3] ^ CK[i]);
                SK[i] = array2[i + 4];
            }
        }

        private void sm4_one_round(long[] sk, byte[] input, byte[] output)
        {
            int i = 0;
            long[] array = new long[36];
            array[0] = GET_ULONG_BE(input, 0);
            array[1] = GET_ULONG_BE(input, 4);
            array[2] = GET_ULONG_BE(input, 8);
            array[3] = GET_ULONG_BE(input, 12);
            for (; i < 32; i++)
            {
                array[i + 4] = sm4F(array[i], array[i + 1], array[i + 2], array[i + 3], sk[i]);
            }

            PUT_ULONG_BE(array[35], output, 0);
            PUT_ULONG_BE(array[34], output, 4);
            PUT_ULONG_BE(array[33], output, 8);
            PUT_ULONG_BE(array[32], output, 12);
        }

        private byte[] padding(byte[] input, int mode)
        {
            if (input == null)
            {
                return null;
            }

            byte[] array = null;
            if (mode == 1)
            {
                int num = 16 - input.Length % 16;
                array = new byte[input.Length + num];
                Array.Copy(input, 0, array, 0, input.Length);
                for (int i = 0; i < num; i++)
                {
                    array[input.Length + i] = (byte)num;
                }
            }
            else
            {
                int num2 = input[input.Length - 1];
                array = new byte[input.Length - num2];
                Array.Copy(input, 0, array, 0, input.Length - num2);
            }

            return array;
        }

        public void sm4_setkey_enc(SM4_Context ctx, byte[] key)
        {
            ctx.mode = 1;
            sm4_setkey(ctx.sk, key);
        }

        public void sm4_setkey_dec(SM4_Context ctx, byte[] key)
        {
            int num = 0;
            ctx.mode = 0;
            sm4_setkey(ctx.sk, key);
            for (num = 0; num < 16; num++)
            {
                SWAP(ctx.sk, num);
            }
        }

        public byte[] sm4_crypt_ecb(SM4_Context ctx, byte[] input)
        {
            if (ctx.isPadding && ctx.mode == 1)
            {
                input = padding(input, 1);
            }

            int num = input.Length;
            byte[] array = new byte[num];
            Array.Copy(input, 0, array, 0, num);
            byte[] array2 = new byte[num];
            int num2 = 0;
            while (num > 0)
            {
                byte[] array3 = new byte[16];
                byte[] array4 = new byte[16];
                Array.Copy(array, num2 * 16, array3, 0, (num > 16) ? 16 : num);
                sm4_one_round(ctx.sk, array3, array4);
                Array.Copy(array4, 0, array2, num2 * 16, (num > 16) ? 16 : num);
                num -= 16;
                num2++;
            }

            if (ctx.isPadding && ctx.mode == 0)
            {
                array2 = padding(array2, 0);
            }

            return array2;
        }

        public byte[] sm4_crypt_cbc(SM4_Context ctx, byte[] iv, byte[] input)
        {
            if (ctx.isPadding && ctx.mode == 1)
            {
                input = padding(input, 1);
            }

            int i = 0;
            int num = input.Length;
            byte[] array = new byte[num];
            Array.Copy(input, 0, array, 0, num);
            List<byte> list = new List<byte>();
            if (ctx.mode == 1)
            {
                int num2 = 0;
                while (num > 0)
                {
                    byte[] array2 = new byte[16];
                    byte[] array3 = new byte[16];
                    byte[] array4 = new byte[16];
                    Array.Copy(array, i * 16, array2, 0, (num > 16) ? 16 : num);
                    for (i = 0; i < 16; i++)
                    {
                        array3[i] = (byte)(array2[i] ^ iv[i]);
                    }

                    sm4_one_round(ctx.sk, array3, array4);
                    Array.Copy(array4, 0, iv, 0, 16);
                    for (int j = 0; j < 16; j++)
                    {
                        list.Add(array4[j]);
                    }

                    num -= 16;
                    num2++;
                }
            }
            else
            {
                byte[] array5 = new byte[16];
                int num3 = 0;
                while (num > 0)
                {
                    byte[] array6 = new byte[16];
                    byte[] array7 = new byte[16];
                    byte[] array8 = new byte[16];
                    Array.Copy(array, i * 16, array6, 0, (num > 16) ? 16 : num);
                    Array.Copy(array6, 0, array5, 0, 16);
                    sm4_one_round(ctx.sk, array6, array7);
                    for (i = 0; i < 16; i++)
                    {
                        array8[i] = (byte)(array7[i] ^ iv[i]);
                    }

                    Array.Copy(array5, 0, iv, 0, 16);
                    for (int k = 0; k < 16; k++)
                    {
                        list.Add(array8[k]);
                    }

                    num -= 16;
                    num3++;
                }
            }

            if (ctx.isPadding && ctx.mode == 0)
            {
                return padding(list.ToArray(), 0);
            }

            return list.ToArray();
        }
    }

    public class SM4CryptoUtil
    {
        public string secretKey = "";

        public string iv = "";

        public bool hexString;

        public string EncryptECB(string plainText, Encoding encoding)
        {
            SM4_Context sM4_Context = new SM4_Context();
            sM4_Context.isPadding = true;
            sM4_Context.mode = 1;
            byte[] key = ((!hexString) ? encoding.GetBytes(secretKey) : ByteUtils.GetBytesByHexString(secretKey));
            SM4 sM = new SM4();
            sM.sm4_setkey_enc(sM4_Context, key);
            byte[] data = sM.sm4_crypt_ecb(sM4_Context, encoding.GetBytes(plainText));
            return ByteUtils.ToHexString(data);
        }

        public string DecryptECB(string cipherText, Encoding encoding)
        {
            SM4_Context sM4_Context = new SM4_Context();
            sM4_Context.isPadding = true;
            sM4_Context.mode = 0;
            byte[] key = ((!hexString) ? encoding.GetBytes(secretKey) : ByteUtils.GetBytesByHexString(secretKey));
            SM4 sM = new SM4();
            sM.sm4_setkey_dec(sM4_Context, key);
            byte[] bytes = sM.sm4_crypt_ecb(sM4_Context, ByteUtils.GetBytesByHexString(cipherText));
            return encoding.GetString(bytes);
        }

        public string EncryptCBC(string plainText, Encoding encoding)
        {
            SM4_Context sM4_Context = new SM4_Context();
            sM4_Context.isPadding = true;
            sM4_Context.mode = 1;
            byte[] key;
            byte[] array;
            if (hexString)
            {
                key = ByteUtils.GetBytesByHexString(secretKey);
                array = ByteUtils.GetBytesByHexString(iv);
            }
            else
            {
                key = encoding.GetBytes(secretKey);
                array = encoding.GetBytes(iv);
            }

            SM4 sM = new SM4();
            sM.sm4_setkey_enc(sM4_Context, key);
            byte[] data = sM.sm4_crypt_cbc(sM4_Context, array, encoding.GetBytes(plainText));
            return encoding.GetString(Hex.Encode(data));
        }

        public string DecryptCBC(string cipherText, Encoding encoding)
        {
            SM4_Context sM4_Context = new SM4_Context();
            sM4_Context.isPadding = true;
            sM4_Context.mode = 0;
            byte[] key;
            byte[] array;
            if (hexString)
            {
                key = ByteUtils.GetBytesByHexString(secretKey);
                array = ByteUtils.GetBytesByHexString(iv);
            }
            else
            {
                key = encoding.GetBytes(secretKey);
                array = encoding.GetBytes(iv);
            }

            SM4 sM = new SM4();
            sM.sm4_setkey_dec(sM4_Context, key);
            byte[] bytes = sM.sm4_crypt_cbc(sM4_Context, array, ByteUtils.GetBytesByHexString(cipherText));
            return encoding.GetString(bytes);
        }
    }
}
