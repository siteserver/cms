using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SiteServer.Utils.ThirdParty.Jdpay
{
    /// <summary>
    /// DES3加密解密
    /// </summary>
    public class Des3
    {
        private const int MAX_MSG_LENGTH = 16 * 1024;
        private static byte[] iv = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        public static String Des3EncryptECB(byte[] key, String data)
        {
            byte[] resultByte = initResultByteArray(data);
            byte[] desdata = Des3EncodeECB(key, iv, resultByte);
            return byteToHexStr(desdata);
        }
        public static String Des3DecryptECB(byte[] key, String data)
        {
            //byte[] dataByte = Encoding.UTF8.GetBytes(data);
            byte[] hexSourceData = hex2byte(data);
            byte[] unDesResult = Des3DecodeECB(key, iv, hexSourceData);
            byte[] dataSizeByte = new byte[4];
            dataSizeByte[0] = unDesResult[0];
            dataSizeByte[1] = unDesResult[1];
            dataSizeByte[2] = unDesResult[2];
            dataSizeByte[3] = unDesResult[3];
            int dsb = byteArrayToInt(dataSizeByte, 0);
            if (dsb > MAX_MSG_LENGTH)
            {
                throw new Exception("msg over MAX_MSG_LENGTH or msg error");
            }
            byte[] tempData = new byte[dsb];
            for (int i = 0; i < dsb; i++)
            {
                tempData[i] = unDesResult[4 + i];
            }
            String hexStr = byteToHexStr(tempData);
            String str = hex2bin(hexStr);
            return str;
        }
        public static String Des3EncryptCBC(byte[] key, String data)
        {
            byte[] desdata = Des3EncodeCBC(key, iv, Encoding.UTF8.GetBytes(data));
            return byteToHexStr(desdata);
        }
        public static String Des3DecryptCBC(byte[] key, String data)
        {
            byte[] desdata = Des3DecodeCBC(key, iv, Encoding.UTF8.GetBytes(data));
            return Encoding.UTF8.GetString(desdata);
        }

        private static byte[] initResultByteArray(String data)
        {
            byte[] source = Encoding.UTF8.GetBytes(data);
            int merchantData = source.Length;
            int x = (merchantData + 4) % 8;
            int y = (x == 0) ? 0 : (8 - x);
            byte[] resultByte = new byte[merchantData + 4 + y];
            resultByte[0] = (byte)((merchantData >> 24) & 0xFF);
            resultByte[1] = (byte)((merchantData >> 16) & 0xFF);
            resultByte[2] = (byte)((merchantData >> 8) & 0xFF);
            resultByte[3] = (byte)(merchantData & 0xFF);
            //4.填充补位数据
            for (int i = 0; i < merchantData; i++)
            {
                resultByte[4 + i] = source[i];
            }
            for (int i = 0; i < y; i++)
            {
                resultByte[merchantData + 4 + i] = 0x00;
            }

            return resultByte;
        }
        public static byte[] hex2byte(String b)
        {
            if ((b.Length % 2) != 0)
            {
                throw new Exception("长度不是偶数");
            }
            byte[] b2 = new byte[b.Length / 2];
            for (int n = 0; n < b.Length; n += 2)
            {
                //byte[] sub = new byte[2];
                //Array.Copy(b,n,sub,0,2);
                //String item = byteToHexStr(sub);
                String item = b.Substring(n, 2);
                // 两位一组，表示一个字节,把这样表示的16进制字符串，还原成一个进制字节
                b2[n / 2] = (byte)Convert.ToInt32(item, 16);
            }
            b = null;
            return b2;
        }
        private static int byteArrayToInt(byte[] b, int offset)
        {
            int value = 0;
            for (int i = 0; i < 4; i++)
            {
                int shift = (4 - 1 - i) * 8;
                value += (b[i + offset] & 0x000000FF) << shift;//往高位游
            }
            return value;
        }

        public static String hex2bin(String hex)
        {
            String digital = "0123456789abcdef";
            char[] hex2char = hex.ToCharArray();
            byte[] bytes = new byte[hex.Length / 2];
            int temp;
            for (int i = 0; i < bytes.Length; i++)
            {
                temp = digital.IndexOf(hex2char[2 * i]) * 16;
                temp += digital.IndexOf(hex2char[2 * i + 1]);
                bytes[i] = (byte)(temp & 0xff);
            }

            return Encoding.UTF8.GetString(bytes);
        }

        public static String bytesToString(byte[] src)
        {
            String hexString = "0123456789ABCDEF";
            StringBuilder stringBuilder = new StringBuilder("");
            if (src == null || src.Length <= 0)
            {
                return null;
            }
            for (int i = 0; i < src.Length; i++)
            {
                int v = src[i] & 0xFF;
                String hv = Convert.ToString(v, 16);
                if (hv.Length < 2)
                {
                    stringBuilder.Append(0);
                }
                stringBuilder.Append(hv);
            }
            String srcStr = stringBuilder.ToString();
            //ByteArrayOutputStream baos = new ByteArrayOutputStream(bytes.length() / 2);
            // 将每2位16进制整数组装成一个字节
            // for (int i = 0; i<bytes.length(); i += 2)
            //     baos.write((hexString.indexOf(bytes.charAt(i)) << 4 | hexString.indexOf(bytes.charAt(i + 1))));

            char[] chars = srcStr.ToCharArray();
            byte[] bytes = new byte[srcStr.Length / 2];
            int temp;
            for (int i = 0; i < bytes.Length; i++)
            {
                temp = hexString.IndexOf(chars[2 * i]) << 4;
                temp += hexString.IndexOf(chars[2 * i + 1]);
                bytes[i] = (byte)(temp & 0xff);
            }

            return Encoding.UTF8.GetString(bytes);
        }


        #region CBC模式**
        /// <summary>
        /// DES3 CBC模式加密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV</param>
        /// <param name="data">明文的byte数组</param>
        /// <returns>密文的byte数组</returns>
        private static byte[] Des3EncodeCBC(byte[] key, byte[] iv, byte[] data)
        {
            try
            {

                MemoryStream mStream = new MemoryStream();
                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.CBC;             //默认值
                tdsp.Padding = PaddingMode.PKCS7;       //默认值
                //tdsp.Padding = PaddingMode.Zeros;
                CryptoStream cStream = new CryptoStream(mStream, tdsp.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                cStream.Write(data, 0, data.Length);
                cStream.FlushFinalBlock();
                byte[] ret = mStream.ToArray();
                cStream.Close();
                mStream.Close();
                return ret;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }
        /// <summary>
        /// DES3 CBC模式解密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV</param>
        /// <param name="data">密文的byte数组</param>
        /// <returns>明文的byte数组</returns>
        private static byte[] Des3DecodeCBC(byte[] key, byte[] iv, byte[] data)
        {
            try
            {
                MemoryStream msDecrypt = new MemoryStream(data);
                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.CBC;
                tdsp.Padding = PaddingMode.PKCS7;
                //tdsp.Padding = PaddingMode.Zeros;
                CryptoStream csDecrypt = new CryptoStream(msDecrypt, tdsp.CreateDecryptor(key, iv), CryptoStreamMode.Read);
                byte[] fromEncrypt = new byte[data.Length];
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
                return fromEncrypt;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }
        #endregion
        #region ECB模式
        /// <summary>
        /// DES3 ECB模式加密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV(当模式为ECB时，IV无用)</param>
        /// <param name="str">明文的byte数组</param>
        /// <returns>密文的byte数组</returns>
        private static byte[] Des3EncodeECB(byte[] key, byte[] iv, byte[] data)
        {
            try
            {
                MemoryStream mStream = new MemoryStream();
                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.ECB;
                tdsp.Padding = PaddingMode.Zeros;
                //tdsp.Padding = PaddingMode.None;
                CryptoStream cStream = new CryptoStream(mStream, tdsp.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                cStream.Write(data, 0, data.Length);
                cStream.FlushFinalBlock();
                byte[] ret = mStream.ToArray();
                cStream.Close();
                mStream.Close();
                return ret;
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }
        /// <summary>
        /// DES3 ECB模式解密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV(当模式为ECB时，IV无用)</param>
        /// <param name="str">密文的byte数组</param>
        /// <returns>明文的byte数组</returns>
        private static byte[] Des3DecodeECB(byte[] key, byte[] iv, byte[] data)
        {
            try
            {
                MemoryStream msDecrypt = new MemoryStream(data);
                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.ECB;
                tdsp.Padding = PaddingMode.Zeros;
                //tdsp.Padding = PaddingMode.None;
                CryptoStream csDecrypt = new CryptoStream(msDecrypt, tdsp.CreateDecryptor(key, iv), CryptoStreamMode.Read);
                byte[] fromEncrypt = new byte[data.Length];
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
                return fromEncrypt;
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }
        #endregion



        //将数组转换成16进制字符串
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr.ToLower();
        }
    }
}
