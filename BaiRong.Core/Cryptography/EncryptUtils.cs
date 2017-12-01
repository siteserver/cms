using System;
using System.Text;
using System.Security.Cryptography;

namespace BaiRong.Core.Cryptography
{
	/// <summary>
	/// 对称加密算法类。
	/// </summary>
	public class EncryptUtils
	{
		private HashAlgorithm mhash;
  
		public static EncryptUtils Instance = new EncryptUtils();

		/// <summary>
		/// 对称加密类的构造函数，不可对加密字符串进行解密
		/// </summary>
		private EncryptUtils()
		{
		}

		/// <summary>
		/// 加密方法
		/// </summary>
		/// <param name="Value">待加密的串</param>
		/// <returns>经过加密的串</returns>
		public string EncryptString(string Value)
		{
			byte[] bytValue;
			byte[] bytHash;

			mhash = SetHash(true);

			// Convert the original string to array of Bytes
			bytValue = Encoding.UTF8.GetBytes(Value);

			// Compute the Hash, returns an array of Bytes
			bytHash = mhash.ComputeHash(bytValue);

			mhash.Clear();

			// Return a base 64 encoded string of the Hash value
			return Convert.ToBase64String(bytHash);
		}

		private HashAlgorithm SetHash(bool isMD5)
		{
			if(!isMD5)
				return new SHA1CryptoServiceProvider();
			else
				return new MD5CryptoServiceProvider();
		}

        public static string Md5(string str)
        {
            var cl = str;
            var pwd = string.Empty;
            var md5 = MD5.Create();
            var s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            for (var i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 

                pwd = pwd + s[i].ToString("x");

            }
            return pwd;
        }
	}
}
