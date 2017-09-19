using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Numerics;
using System.IO;
using System;
using BaiRong.Core;



namespace SiteServer.B2C.Core.Union
{

    public class CertUtil
    {

        public static string PUBLIC_CERT = "PUBLIC_CERT";
        public static string SIGN_CERT = "SIGN_CERT";
        private static bool isChange = false;
        public static bool IsChange
        {
            get { return isChange; }
            set { isChange = value; }
        }

        /// <summary>
        /// 加载银联公钥
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static X509Certificate2 LoadCertFile(string path)
        {
            X509Certificate2 pc = CacheUtils.Get(PUBLIC_CERT) as X509Certificate2;
            if (pc == null)
            {
                pc = new X509Certificate2(path);
                CacheUtils.Max(PUBLIC_CERT, pc);
            }
            return pc;
        }

        /// <summary>
        /// 加载商家私钥
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 LoadCertFile()
        {
            X509Certificate2 pc = CacheUtils.Get(SIGN_CERT) as X509Certificate2;
            if (pc == null)
            {
                pc = new X509Certificate2(sdkConfig.SignCertPath, sdkConfig.SignCertPwd, X509KeyStorageFlags.PersistKeySet);
                CacheUtils.Max(SIGN_CERT, pc);
            }
            return pc;
        }


        public static SDKConfig sdkConfig { get; set; }

        /// <summary>
        /// 获取签名证书私钥
        /// </summary>
        /// <returns></returns>
        public static RSACryptoServiceProvider GetSignProviderFromPfx()
        {
            try
            {
                X509Certificate2 pc = LoadCertFile();
                return (RSACryptoServiceProvider)pc.PrivateKey;
            }
            catch (Exception)
            {
                return new RSACryptoServiceProvider();   
            }


        }

        /// <summary>
        /// 获取签名证书的证书序列号
        /// </summary>
        /// <returns></returns>
        public static string GetSignCertId()
        {
            try
            {
                X509Certificate2 pc = LoadCertFile();
                return BigNum.ToDecimalStr(BigNum.ConvertFromHex(pc.SerialNumber)); //低于4.0版本的.NET请使用此方法
                //return BigInteger.Parse(pc.SerialNumber, System.Globalization.NumberStyles.HexNumber).ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// 通过证书id，获取验证签名的证书
        /// </summary>
        /// <param name="certId"></param>
        /// <returns></returns>
        public static RSACryptoServiceProvider GetValidateProviderFromPath(string certId)// 
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(sdkConfig.ValidateCertDir);
                FileInfo[] files = directory.GetFiles("*.cer");
                if (null == files || 0 == files.Length)
                {
                    return null;
                }
                foreach (FileInfo file in files)
                {
                    X509Certificate2 pc = LoadCertFile(file.DirectoryName + "\\" + file.Name);
                    string id = BigNum.ToDecimalStr(BigNum.ConvertFromHex(pc.SerialNumber)); //低于4.0版本的.NET请使用此方法
                    //string id = BigInteger.Parse(pc.SerialNumber, System.Globalization.NumberStyles.HexNumber).ToString();
                    if (certId.Equals(id))
                    {

                        return (RSACryptoServiceProvider)pc.PublicKey.Key;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
            

        }

    }
}