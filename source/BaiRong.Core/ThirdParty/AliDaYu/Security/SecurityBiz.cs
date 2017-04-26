using System;
using System.Collections.Generic;
using Top.Api.Util;
using System.Text;


namespace Top.Api.Security
{

    public class SecurityBiz : SecurityConstants
    {
        private static IDictionary<string, Nullable<char>> SEPARATOR_CHAR_MAP = InitSeparatorCharMap();
        private static IDictionary<string, string> SEPARATOR_MAP = InitSeparatorMap();

        private static IDictionary<string, Nullable<char>> InitSeparatorCharMap()
        {
            IDictionary<string, Nullable<char>> map = new Dictionary<string, Nullable<char>>();
            map.Add(NICK, NICK_SEPARATOR_CHAR);
            map.Add(RECEIVER_NAME, NICK_SEPARATOR_CHAR);
            map.Add(NORMAL, NORMAL_SEPARATOR_CHAR);
            map.Add(PHONE, PHONE_SEPARATOR_CHAR);
            return map;
        }


        private static IDictionary<string, string> InitSeparatorMap()
        {
            IDictionary<string, string> map = new Dictionary<string, string>();
            map.Add(NICK, NICK_SEPARATOR);
            map.Add(RECEIVER_NAME, NICK_SEPARATOR);
            map.Add(NORMAL, NORMAL_SEPARATOR);
            map.Add(PHONE, PHONE_SEPARATOR);
            return map;
        }

        public static IDictionary<string, Nullable<char>> GetSeparatorCharMap()
        {
            return SEPARATOR_CHAR_MAP;
        }

        public static IDictionary<string, string> GetSeparatorMap()
        {
            return SEPARATOR_MAP;
        }

        /// <summary>
        /// 判断是否密文数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type">加密字段类型(例如：nick\phone)</param>
        /// <returns></returns>
        public static bool IsEncryptData(string data, string type)
        {
            if (string.IsNullOrEmpty(data) || data.Length < 4)
            {
                return false;
            }

            Nullable<char> charValue = null;
            SEPARATOR_CHAR_MAP.TryGetValue(type, out charValue);
            if (charValue == null)
            {
                throw new SecretException("type error");
            }
            char separatorChar = charValue.Value;
            if (!(data[0] == charValue && data[data.Length - 1] == charValue))
            {
                return false;
            }

            if (separatorChar == PHONE_SEPARATOR_CHAR)
            {
                // 拆分元素
                string[] dataArray = StringUtil.Split(data, separatorChar);
                if (dataArray.Length != 3)
                {
                    return false;
                }
                if (data[data.Length - 2] == separatorChar)
                {
                    return CheckEncryptData(dataArray);
                }
                else
                {

                    string version = dataArray[dataArray.Length - 1];
                    if (StringUtil.IsDigits(version) && Convert.ToInt64(version) > 0)
                    {
                        bool isBase64Value = SecurityUtil.IsBase64Value(dataArray[dataArray.Length - 2]);
                        if (isBase64Value)
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
            else
            {
                // 拆分元素
                string[] dataArray = StringUtil.Split(data, separatorChar);
                if (data[data.Length - 2] == separatorChar)
                {
                    if (dataArray.Length != 3)
                    {
                        return false;
                    }
                    return CheckEncryptData(dataArray);
                }
                else
                {
                    if (dataArray.Length != 2)
                    {
                        return false;
                    }
                    return CheckEncryptData(dataArray);
                }
            }

            return false;
        }

       /// <summary>
        /// 判断是否密文数据
       /// </summary>
       /// <param name="dataArray"></param>
       /// <returns></returns>
        private static bool CheckEncryptData(string[] dataArray)
        {
            string version = dataArray[dataArray.Length - 1];
            if (StringUtil.IsDigits(version) && Convert.ToInt64(version) > 0)
            {
                bool isBase64Value = SecurityUtil.IsBase64Value(dataArray[0]);
                if (isBase64Value)
                {
                    if (dataArray.Length == 3)
                    {
                        isBase64Value = SecurityUtil.IsBase64Value(dataArray[1]);
                        if (isBase64Value)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 加密手机尾号后8位
        /// </summary>
        /// <param name="data"></param>
        /// <param name="separator"></param>
        /// <param name="secretContext"></param>
        /// <returns></returns>
        public static string EncryptPhone(string data, string separator, SecretContext secretContext)
        {
            if (data.Length < 11)
            {
                return data;
            }
            string prefixNumber = data.Substring(0, data.Length - 8);
            // 取后8位置
            string last8Number = data.Substring(data.Length - 8);

            return separator + prefixNumber + separator + SecurityUtil.AESEncrypt(last8Number, secretContext.Secret) + separator
                    + secretContext.SecretVersion + separator;
        }

        /// <summary>
        /// 加密手机后4位转H-MAC
        /// </summary>
        /// <param name="data"></param>
        /// <param name="separator"></param>
        /// <param name="secretContext"></param>
        /// <returns></returns>
        public static string EncryptPhoneIndex(string data, string separator, SecretContext secretContext)
        {
            if (data.Length < 11)
            {
                return data;
            }
            // 取后4位
            string last4Number = data.Substring(data.Length - 4);
            return separator + SecurityUtil.HmacMD5EncryptToBase64(last4Number, secretContext.Secret) + separator
                    + SecurityUtil.AESEncrypt(data, secretContext.Secret) + separator + secretContext.SecretVersion
                    + separator + separator;
        }

        /// <summary>
        /// 手机号后4位H-MAC值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="separator"></param>
        /// <param name="secretContext"></param>
        /// <returns></returns>
        public static string SearchPhoneIndex(string data, string separator, SecretContext secretContext)
        {
            if (data.Length != 4)
            {
                throw new SecretException("phoneNumber error");
            }
            return separator + SecurityUtil.HmacMD5EncryptToBase64(data, secretContext.Secret) + separator;
        }

        /// <summary>
        ///  生成密文数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="separator"></param>
        /// <param name="secretContext"></param>
        /// <returns></returns>
        public static string EncryptNormal(string data, string separator, SecretContext secretContext)
        {
            return separator + SecurityUtil.AESEncrypt(data, secretContext.Secret) + separator + secretContext.SecretVersion
                    + separator;
        }

        /// <summary>
        /// 滑窗加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="compressLen"></param>
        /// <param name="slideSize"></param>
        /// <param name="separator"></param>
        /// <param name="secretContext"></param>
        /// <returns></returns>
        public static string EncryptNormalIndex(string data, int compressLen, int slideSize, string separator,
                SecretContext secretContext)
        {
            List<string> slideList = SecurityUtil.GetSlideWindows(data, slideSize);
            StringBuilder builder = new StringBuilder();
            foreach (string slide in slideList)
            {
                builder.Append(SecurityUtil.HmacMD5EncryptToBase64(slide, secretContext.Secret, compressLen));
            }

            return separator + SecurityUtil.AESEncrypt(data, secretContext.Secret) + separator + builder.ToString() + separator
                    + secretContext.SecretVersion + separator + separator;
        }

        /// <summary>
        /// 密文检索
        /// </summary>
        /// <param name="data"></param>
        /// <param name="compressLen"></param>
        /// <param name="slideSize"></param>
        /// <param name="secretContext"></param>
        /// <returns></returns>
        public static string SearchNormalIndex(string data, int compressLen, int slideSize, SecretContext secretContext)
        {
            List<string> slideList = SecurityUtil.GetSlideWindows(data, slideSize);
            StringBuilder builder = new StringBuilder();
            foreach (string slide in slideList)
            {
                builder.Append(SecurityUtil.HmacMD5EncryptToBase64(slide, secretContext.Secret, compressLen));
            }

            return builder.ToString();
        }

        /// <summary>
        /// 获取秘钥版本、加密原始数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="separatorChar"></param>
        /// <returns></returns>
        public static SecretData GetSecretData(string data, char separatorChar)
        {
            SecretData secretData = null;
            if (PHONE_SEPARATOR_CHAR == separatorChar)
            {
                string[] dataArray = StringUtil.Split(data, separatorChar);
                if (dataArray.Length != 3)
                {
                    return null;
                }

                string version = dataArray[2];
                if (StringUtil.IsDigits(version) && Convert.ToInt64(version) > 0)
                {
                    secretData = new SecretData();
                    secretData.OriginalValue = dataArray[0]; ;// 手机号码前缀
                    secretData.OriginalBase64Value = dataArray[1];
                    secretData.SecretVersion = Convert.ToInt64(version);
                }
            }
            else
            {
                string[] dataArray = StringUtil.Split(data, separatorChar);
                if (dataArray.Length != 2)
                {
                    return null;
                }

                string version = dataArray[1];
                if (StringUtil.IsDigits(version) && Convert.ToInt64(version) > 0)
                {
                    secretData = new SecretData();
                    secretData.OriginalBase64Value = dataArray[0];
                    secretData.SecretVersion = Convert.ToInt64(version);
                }
            }
            return secretData;
        }

        /// <summary>
        /// 获取秘钥版本、加密原始数据（支持密文检索）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="separatorChar"></param>
        /// <returns></returns>
        public static SecretData GetIndexSecretData(string data, char separatorChar)
        {
            SecretData secretData = null;
            if (PHONE_SEPARATOR_CHAR == separatorChar)
            {
                string[] dataArray = StringUtil.Split(data, separatorChar);
                if (dataArray.Length != 3)
                {
                    return null;
                }

                string version = dataArray[2];
                if (StringUtil.IsDigits(version) && Convert.ToInt64(version) > 0)
                {
                    secretData = new SecretData();
                    secretData.OriginalValue = dataArray[0];// H-MAC(手机号码后4位)
                    secretData.OriginalBase64Value = dataArray[1];
                    secretData.SecretVersion = Convert.ToInt64(version);
                }
            }
            else
            {
                string[] dataArray = StringUtil.Split(data, separatorChar);
                if (dataArray.Length != 3)
                {
                    return null;
                }

                string version = dataArray[2];
                if (StringUtil.IsDigits(version) && Convert.ToInt64(version) > 0)
                {
                    secretData = new SecretData();
                    secretData.OriginalBase64Value = dataArray[0];
                    secretData.OriginalValue = dataArray[1];// H-MAC value
                    secretData.SecretVersion = Convert.ToInt64(version);
                }
            }
            if (secretData == null)
            {
                return secretData;
            }

            secretData.Search = true;
            return secretData;
        }

    }
}
