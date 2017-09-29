using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RunningBox
{
    public partial class Function
    {
        private static AesCryptoServiceProvider _AesEncryptor = new AesCryptoServiceProvider();
        private static byte[] _Salt = new byte[] { 0x1A, 0x2B, 0x3C, 0x4D, 0x5E, 0x6F, 0x70, 0x88 };

        /// <summary>
        /// 使用指定key和iv加解密byte[]
        /// </summary>
        /// <param name="source">要加/解密的byte[]</param>
        /// <param name="key">金鑰</param>
        /// <param name="iv">初始向量</param>
        /// <param name="decrypt">是否為解密</param>
        /// <returns>加/解密後字串</returns>
        public static byte[] EncryptByte(byte[] source, string key, string iv, bool decrypt)
        {
            byte[] result;
            try
            {
                Rfc2898DeriveBytes rfcKey = new Rfc2898DeriveBytes("", _Salt, 8);
                Rfc2898DeriveBytes rfcIv = new Rfc2898DeriveBytes("", _Salt, 8);
                _AesEncryptor.Key = rfcKey.GetBytes(32);
                _AesEncryptor.IV = rfcIv.GetBytes(16);

                using (ICryptoTransform encryptor = decrypt ? _AesEncryptor.CreateDecryptor() : _AesEncryptor.CreateEncryptor())
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(source, 0, source.Length);
                    cs.FlushFinalBlock();
                    result = ms.ToArray();
                    cs.Close();
                    ms.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// byte陣列轉int32陣列
        /// </summary>
        /// <param name="byteArray">byte陣列</param>
        /// <returns>int32陣列</returns>
        public static int[] ByteToInt(byte[] byteArray)
        {
            if (byteArray.Length % 4 > 0) return null;

            int[] result = new int[byteArray.Length / 4];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = BitConverter.ToInt32(byteArray, i * 4);
            }
            return result;
        }

        /// <summary>
        /// int32陣列轉byte陣列
        /// </summary>
        /// <param name="intArray">int陣列</param>
        /// <returns>byte陣列</returns>
        public static byte[] IntToByte(int[] intArray)
        {
            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            {
                for (int i = 0; i < intArray.Length; i++)
                {
                    ms.Write(BitConverter.GetBytes(intArray[i]), 0, 4);
                }

                result = ms.ToArray();
                ms.Close();
            }
            return result;
        }
    }
}
