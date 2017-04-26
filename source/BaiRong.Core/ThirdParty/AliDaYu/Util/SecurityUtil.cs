
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Top.Api.Util
{
    /// <summary>
    /// 安全工具类
    /// </summary>
    public abstract class SecurityUtil
    {
        private static readonly char[] CA = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".ToCharArray();
        private static readonly int[] IA = InitIA();
        private static readonly int KeySize = 128;
        private static readonly int BlockSize = 128;
        private static readonly byte[] IvBytes = Encoding.UTF8.GetBytes("0102030405060708");//初始向量

        private static int[] InitIA()
        {
            int len = 256;
            int[] a = new int[len];
            for (int i = 0; i < len; i++)
            {
                a[i] = -1;
            }

            for (int i = 0, iS = CA.Length; i < iS; i++)
            {
                a[CA[i]] = i;
            }
            a['='] = 0;
            return a;
        }

        /// <summary>
        /// 判断是否base64值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBase64Value(string str)
        {
            // Check special case
            int sLen = str != null ? str.Length : 0;
            if (sLen == 0)
                return false;

            // Count illegal characters (including '\r', '\n') to know what size the returned array will be,
            // so we don't have to reallocate & copy it later.
            int sepCnt = 0; // Number of separator characters. (Actually illegal characters, but that's a bonus...)
            for (int i = 0; i < sLen; i++)  // If input is "pure" (I.e. no line separators or illegal chars) base64 this loop can be commented out.
                if (IA[str[i]] < 0)
                    sepCnt++;

            // Check so that legal chars (including '=') are evenly divideable by 4 as specified in RFC 2045.
            if ((sLen - sepCnt) % 4 != 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 生成滑动窗口
        /// </summary>
        /// <param name="input">数据</param>
        /// <param name="slideSize">分词大小</param>
        /// <returns>分词元素</returns>
        public static List<string> GetSlideWindows(string input, int slideSize)
        {
            List<string> windows = new List<string>();
            int startIndex = 0;
            int endIndex = 0;
            int currentWindowSize = 0;
            string currentWindow = null;

            while (endIndex < input.Length || currentWindowSize > slideSize)
            {
                bool startsWithLetterOrDigit;
                if (currentWindow == null)
                {
                    startsWithLetterOrDigit = false;
                }
                else
                {
                    startsWithLetterOrDigit = IsLetterOrDigit(currentWindow[0]);
                }

                if (endIndex == input.Length && !startsWithLetterOrDigit)
                {
                    break;
                }

                if (currentWindowSize == slideSize && !startsWithLetterOrDigit && IsLetterOrDigit(input[endIndex]))
                {
                    endIndex++;
                    currentWindow = input.Substring(startIndex, endIndex - startIndex);
                    currentWindowSize = 5;

                }
                else
                {
                    if (endIndex != 0)
                    {
                        if (startsWithLetterOrDigit)
                        {
                            currentWindowSize -= 1;
                        }
                        else
                        {
                            currentWindowSize -= 2;
                        }
                        startIndex++;
                    }

                    while (currentWindowSize < slideSize && endIndex < input.Length)
                    {
                        char currentChar = input[endIndex];
                        if (IsLetterOrDigit(currentChar))
                        {
                            currentWindowSize += 1;
                        }
                        else
                        {
                            currentWindowSize += 2;
                        }
                        endIndex++;
                    }
                    currentWindow = input.Substring(startIndex, endIndex - startIndex);

                }
                windows.Add(currentWindow);
            }
            return windows;
        }

        /// <summary>
        /// 判断是否小写字母
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static bool IsLetterOrDigit(char x)
        {
            if (0 <= x && x <= 127)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="input"></param>
        /// <param name="toLength"></param>
        /// <returns></returns>
        private static byte[] Compress(byte[] input, int toLength)
        {
            if (toLength < 0)
            {
                return null;
            }
            byte[] output = new byte[toLength];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = 0;
            }

            for (int i = 0; i < input.Length; i++)
            {
                int index_output = i % toLength;
                output[index_output] ^= input[i];
            }

            return output;
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="source">待加密的明文</param>
        /// <param name="encode">编码方式</param>
        /// <returns></returns>
        public static string EncodeBase64(string source, Encoding encode)
        {
            byte[] bytes = encode.GetBytes(source);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="source">待加密的明文</param>
        /// <returns></returns>
        /// 
        /// <seealso cref="SecurityUtil.EncodeBase64(string,Encoding)">  
        /// 参看SecurityUtil.EncodeBase64(string,Encoding)方法的说明 </seealso>  
        public static string EncodeBase64(string source)
        {
            return EncodeBase64(source, Encoding.UTF8);
        }

        /// <summary>
        /// AES加密 
        /// </summary>
        /// <param name="context">待加密的内容</param>
        /// <param name="keyBytes">加密密钥</param>
        /// <returns></returns>
        public static string AESEncrypt(string context, byte[] keyBytes)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = KeySize;
            rijndaelCipher.BlockSize = KeySize;


            // 加密密钥
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = IvBytes;

            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

            byte[] plainText = Encoding.UTF8.GetBytes(context);
            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="context"></param>
        /// <param name="keyBytes"></param>
        /// <returns></returns>
        public static string AESDecrypt(string context, byte[] keyBytes)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = KeySize;
            rijndaelCipher.BlockSize = BlockSize;

            byte[] encryptedData = Convert.FromBase64String(context);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = IvBytes;

            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();

            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }

        public static byte[] HmacMD5Encrypt(String encryptText, byte[] encryptKey)
        {
            HMACMD5 hmac = new HMACMD5(encryptKey);
            byte[] bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(encryptText));
            return bytes;
        }


        /// <summary>
        /// 生成BASE64(H_MAC)
        /// </summary>
        /// <param name="encryptText">被签名的字符串</param>
        /// <param name="encryptKey">秘钥</param>
        /// <returns></returns>
        public static string HmacMD5EncryptToBase64(string encryptText, byte[] encryptKey)
        {
            return Convert.ToBase64String(HmacMD5Encrypt(encryptText, encryptKey));
        }

        /// <summary>
        /// 生成BASE64(H_MAC),压缩H_MAC值
        /// </summary>
        /// <param name="encryptText"></param>
        /// <param name="encryptKey"></param>
        /// <param name="compressLen"></param>
        /// <returns></returns>
        public static String HmacMD5EncryptToBase64(string encryptText, byte[] encryptKey, int compressLen)
        {
            return Convert.ToBase64String(Compress(HmacMD5Encrypt(encryptText, encryptKey), compressLen));
        }

    }
}
