using System.Collections.Generic;
using System.Text;
using System;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SiteServer.B2C.Core.Union
{
    public class SDKUtil
    {

        public static SDKConfig sdkConfig { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="dataStr"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public static bool Sign(Dictionary<string, string> data, Encoding encoder)
        {
            //设置签名证书序列号 ？

            data["certId"] = CertUtil.GetSignCertId();

            //将Dictionary信息转换成key1=value1&key2=value2的形式
            string stringData = CoverDictionaryToString(data);

            string stringSign = null;

            byte[] signDigest = SecurityUtil.Sha1X16(stringData, encoder);

            string stringSignDigest = BitConverter.ToString(signDigest).Replace("-", "").ToLower();

            byte[] byteSign = SecurityUtil.SignBySoft(CertUtil.GetSignProviderFromPfx(), encoder.GetBytes(stringSignDigest));

            stringSign = Convert.ToBase64String(byteSign);

            //设置签名域值
            data["signature"] = stringSign;

            return true;
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public static bool Validate(Dictionary<string, string> data, Encoding encoder)
        {
            //获取签名
            string signValue = data["signature"];
            byte[] signByte = Convert.FromBase64String(signValue);
            data.Remove("signature");
            string stringData = CoverDictionaryToString(data);
            byte[] signDigest = SecurityUtil.Sha1X16(stringData, encoder);
            string stringSignDigest = BitConverter.ToString(signDigest).Replace("-", "").ToLower();
            RSACryptoServiceProvider provider = CertUtil.GetValidateProviderFromPath(data["certId"]);
            if (null == provider)
            {
                return false;
            }
            return SecurityUtil.ValidateBySoft(provider, signByte, encoder.GetBytes(stringSignDigest));
        }


        /// <summary>
        /// 将Dictionary内容排序后输出为键值对字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string CoverDictionaryToString(Dictionary<string, string> data)
        {
            //如果不加stringComparer.Ordinal，排序方式和java treemap有差异
            SortedDictionary<string, string> treeMap = new SortedDictionary<string, string>(StringComparer.Ordinal);

            foreach (KeyValuePair<string, string> kvp in data)
            {
                treeMap.Add(kvp.Key, kvp.Value);
            }

            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> element in treeMap)
            {
                builder.Append(element.Key + "=" + element.Value + "&");
            }

            return builder.ToString().Substring(0, builder.Length - 1);
        }

        /// <summary>
        /// 将字符串key1=value1&key2=value2转换为Dictionary数据结构
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CoverstringToDictionary(string data)
        {
            if (null == data || 0 == data.Length)
            {
                return null;
            }
            string[] arrray = data.Split(new char[] { '&' });
            Dictionary<string, string> res = new Dictionary<string, string>();
            foreach (string s in arrray)
            {
                int n = s.IndexOf("=");
                string key = s.Substring(0, n);
                string value = s.Substring(n + 1);
                Console.WriteLine(key + "=" + value);
                res.Add(key, value);
            }
            return res;
        }

        public static string CreateAutoSubmitForm(string url, Dictionary<string, string> data, Encoding encoder)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendFormat("<meta http-equiv=\"Content-Type\" content=\"text/html; charset={0}\" />", encoder.BodyName);
            html.AppendLine("</head>");
            html.AppendLine("<body onload=\"OnLoadSubmit();\">");
            html.AppendFormat("<form id=\"pay_form\" action=\"{0}\" method=\"post\">", url);
            foreach (KeyValuePair<string, string> kvp in data)
            {
                html.AppendFormat("<input type=\"hidden\" name=\"{0}\" id=\"{0}\" value=\"{1}\" />", kvp.Key, kvp.Value);
            }
            html.AppendLine("</form>");
            html.AppendLine("<script type=\"text/javascript\">");
            html.AppendLine("<!--");
            html.AppendLine("function OnLoadSubmit()");
            html.AppendLine("{");
            html.AppendLine("document.getElementById(\"pay_form\").submit();");
            html.AppendLine("}");
            html.AppendLine("//-->");
            html.AppendLine("</script>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            return html.ToString();
        }

        public static string CreateAutoSubmitForm(int orderID, string url, Dictionary<string, string> data, Encoding encoder)
        {
            StringBuilder html = new StringBuilder();

            html.AppendFormat("<form id=\"pay_form_{0}\" action=\"{1}\" method=\"post\">", orderID, url);
            foreach (KeyValuePair<string, string> kvp in data)
            {
                html.AppendFormat("<input type=\"hidden\" name=\"{0}\" id=\"{0}\" value=\"{1}\" />", kvp.Key, kvp.Value);
            }
            html.Append("<input type='submit' value='' style='display:none;'>");

            html.AppendLine("</form>");
            return html.ToString();
        }

        public static string CreateAutoSubmitFormBtnClickString(int orderID)
        {
            return string.Format("document.forms['pay_form_{0}'].submit();", orderID);
        }

        /// <summary>
        /// 将Dictionary内容排序后输出为键值对字符串,供打印报文使用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string PrintDictionaryToString(Dictionary<string, string> data)
        {
            //如果不加stringComparer.Ordinal，排序方式和java treemap有差异
            SortedDictionary<string, string> treeMap = new SortedDictionary<string, string>(StringComparer.Ordinal);

            foreach (KeyValuePair<string, string> kvp in data)
            {
                treeMap.Add(kvp.Key, kvp.Value);
            }

            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> element in treeMap)
            {
                builder.Append(element.Key + "=" + element.Value + "&");
            }

            return builder.ToString().Substring(0, builder.Length - 1);
        }


        /// <summary>
        /// pinblock 16进制计算
        /// </summary>
        /// <param name="encoder"></param>
        /// <returns></returns>

        public static string printHexString(byte[] b)
        {

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < b.Length; i++)
            {
                string hex = Convert.ToString(b[i] & 0xFF, 16);
                if (hex.Length == 1)
                {
                    hex = '0' + hex;
                }
                sb.Append("0x");
                sb.Append(hex + " ");
            }
            sb.Append("");
            return sb.ToString();
        }



        /// <summary>
        /// 密码pinblock加密
        /// </summary>
        /// <param name="card"></param>
        /// <param name="pwd"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string encryptPin(string card, string pwd, string encoding)
        {

            /** 生成PIN Block **/
            byte[] pinBlock = SecurityUtil.pin2PinBlockWithCardNO(pwd, card);
            printHexString(pinBlock);


            X509Certificate2 pc = new X509Certificate2(sdkConfig.EncryptCert);


            RSACryptoServiceProvider p = new RSACryptoServiceProvider();

            p = (RSACryptoServiceProvider)pc.PublicKey.Key;

            byte[] enBytes = p.Encrypt(pinBlock, false);

            return Convert.ToBase64String(enBytes);


            // return SecurityUtil.EncryptPin(pwd, card, encoding);
        }

        /// <summary>
        /// 数据加密
        /// </summary>
        /// <param name="encoder"></param>
        /// <returns></returns>


        public static string encryptData(string data, string encoding)
        {

            X509Certificate2 pc = new X509Certificate2(sdkConfig.EncryptCert);


            RSACryptoServiceProvider p = new RSACryptoServiceProvider();

            p = (RSACryptoServiceProvider)pc.PublicKey.Key;

            byte[] enBytes = p.Encrypt(UTF8Encoding.UTF8.GetBytes(data), false);

            return Convert.ToBase64String(enBytes);
        }





    }
}