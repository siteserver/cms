using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

//http://payapi.jd.com/docList.html?methodName=0

namespace SiteServer.Utils.ThirdParty.Jdpay
{
    public class SHAUtil
    {
        public static String encryptSHA256(String strSrc)
        {
            byte[] sourceByte = Encoding.UTF8.GetBytes(strSrc);
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            byte[] cryByte = sha256.ComputeHash(sourceByte);
            return byteToHexStr(cryByte);
        }
       
        //将数组转换成16进制字符串
        private static string byteToHexStr(byte[] bytes)
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
