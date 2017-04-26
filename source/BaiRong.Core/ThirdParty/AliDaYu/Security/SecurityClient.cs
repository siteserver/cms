using System;
using System.Collections.Generic;
using Top.Api.Util;


namespace Top.Api.Security
{
    /// <summary>
    /// 加、解密客户端(单例使用，不要初始化多个)
    /// </summary>
    public class SecurityClient : SecurityConstants
    {
        // 秘钥管理核心类
        private SecurityCore secretCore;

        /// <summary>
        /// 秘钥管理核心类
        /// </summary>
        /// <param name="topClientt"> serverUrl必须是https协议</param>
        /// <param name="randomNum">伪随机码</param>
        public SecurityClient(ITopClient topClientt, string randomNum)
        {
            secretCore = new SecurityCore(topClientt, randomNum);
        }

        /// <summary>
        ///  初始化秘钥（如果半小时内会调用加、解密方法，建议先初始化秘钥）。所有用户共用秘钥
        /// </summary>
        public void InitSecret()
        {
            secretCore.GetSecret(null, null);
        }

        public void SetRandomNum(string randomNum)
        {
            this.secretCore.SetRandomNum(randomNum);
        }

        /// <summary>
        /// 初始化秘钥（如果半小时内会调用加、解密方法，建议先初始化秘钥）。每个用户单独分配秘钥
        /// </summary>
        /// <param name="session"></param>
        public void InitSecret(string session)
        {
            secretCore.GetSecret(session, null);
        }

        /// <summary>
        /// 批量解密（所有用户共用秘钥）
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IDictionary<string, string> Decrypt(List<string> dataList, string type)
        {
            return Decrypt(dataList, type, null);
        }

        /// <summary>
        /// 批量解密（每个用户单独分配秘钥）
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="type"></param>
        /// <param name="session"></param>
        /// <returns>key=密文数据，value=明文数据</returns>
        public IDictionary<string, string> Decrypt(List<string> dataList, string type, string session)
        {
            if (dataList == null || dataList.Count == 0)
            {
                throw new SecretException("dataList can`t be empty");
            }
            IDictionary<string, string> resultMap = new Dictionary<string, string>();
            foreach (string data in dataList)
            {
                if (!resultMap.ContainsKey(data))
                {
                    string decryptValue = Decrypt(data, type, session);
                    resultMap.Add(data, decryptValue);
                }
            }
            return resultMap;
        }

        /// <summary>
        /// 解密（所有用户共用秘钥）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string Decrypt(string data, string type)
        {
            return Decrypt(data, type, null);
        }

        /// <summary>
        /// 解密（每个用户单独分配秘钥）
        /// </summary>
        /// <param name="data">
        /// 密文数据 手机号码格式：$手机号码前3位明文$base64(encrypt(phone后8位))$111$
        /// nick格式：~base64(encrypt(nick))~111~
        /// </param>
        /// <param name="type">解密字段类型(例如：nick\phone)</param>
        /// <param name="session">用户身份,用户级加密必填</param>
        /// <returns></returns>
        public string Decrypt(string data, string type, string session)
        {
            if (string.IsNullOrEmpty(data) || data.Length < 4)
            {
                return data;
            }

            // 获取分隔符
            Nullable<char> charValue = null;
            SecurityBiz.GetSeparatorCharMap().TryGetValue(type, out charValue);

            if (charValue == null)
            {
                throw new SecretException("type error");
            }

            // 校验
            char separator = charValue.Value;
            if (!(data[0] == separator && data[data.Length - 1] == separator))
            {
                return data;
            }
            SecretData secretDataDO = null;
            if (data[data.Length - 2] == separator)
            {
                secretDataDO = SecurityBiz.GetIndexSecretData(data, separator);
            }
            else
            {
                secretDataDO = SecurityBiz.GetSecretData(data, separator);
            }

            // 非法密文
            if (secretDataDO == null)
            {
                return data;
            }
            SecurityCounter.AddDecryptCount(type);// 计数器
            SecretContext secretContextDO = secretCore.GetSecret(session, secretDataDO.SecretVersion);
            string decryptValue = SecurityUtil.AESDecrypt(secretDataDO.OriginalBase64Value, secretContextDO.Secret);
            if (PHONE.Equals(type) && !secretDataDO.Search)
            {
                // 加上手机号前3位，手机号只加密了后8位
                return secretDataDO.OriginalValue + decryptValue;
            }
            return decryptValue;

        }

        /// <summary>
        /// 判断list元素是否全部为密文数据
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="type">加密字段类型(例如：nick\phone)</param>
        /// <returns></returns>
        public static bool IsEncryptData(List<string> dataList, string type)
        {
            if (dataList == null || dataList.Count == 0)
            {
                return false;
            }
            bool result = false;
            foreach (string data in dataList)
            {
                result = IsEncryptData(data, type);
                if (!result)
                {
                    return false;
                }
            }
            return result;
        }

        /// <summary>
        /// 判断是否密文数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type">加密字段类型(例如：nick\phone)</param>
        /// <returns></returns>
        public static bool IsEncryptData(string data, string type)
        {
            return SecurityBiz.IsEncryptData(data, type);
        }

        /// <summary>
        /// 加密（所有用户共用秘钥）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string Encrypt(string data, string type)
        {
            return Encrypt(data, type, null, null);
        }

        /// <summary>
        /// 用老秘钥加密，只在秘钥升级时使用（所有用户共用秘钥）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string EncryptPrevious(string data, string type)
        {
            return Encrypt(data, type, null, -1L);
        }

        /// <summary>
        /// 加密（每个用户单独分配秘钥）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public string Encrypt(string data, string type, string session)
        {
            return Encrypt(data, type, session, null);
        }

        /// <summary>
        /// 用老秘钥加密，只在秘钥升级时使用（每个用户单独分配秘钥）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public string EncryptPrevious(string data, string type, string session)
        {
            return Encrypt(data, type, session, -1L);
        }

        /// <summary>
        /// 密文检索（所有用户共用秘钥）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string Search(string data, string type)
        {
            return Search(data, type, null, null);
        }

        /// <summary>
        /// 密文检索,在秘钥升级场景下兼容查询（所有用户共用秘钥）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string SearchPrevious(string data, string type)
        {
            return Search(data, type, null, -1L);
        }

        /// <summary>
        /// 密文检索（每个用户单独分配秘钥）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public string Search(string data, string type, string session)
        {
            return Search(data, type, session, null);
        }

        /// <summary>
        /// 密文检索,在秘钥升级场景下兼容查询（每个用户单独分配秘钥）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public string SearchPrevious(string data, string type, string session)
        {
            return Search(data, type, session, -1L);
        }

        /// <summary>
        /// 密文检索。 手机号码格式：$base64(H-MAC(phone后4位))$ nick格式：base64(H-MAC(滑窗))
        /// </summary>
        /// <param name="data">明文数据</param>
        /// <param name="type">加密字段类型(例如：nick\phone)</param>
        /// <param name="session">用户身份,用户级加密必填</param>
        /// <param name="version">秘钥历史版本</param>
        /// <returns></returns>
        private string Search(string data, string type, string session, Nullable<Int64> version)
        {
            if (string.IsNullOrEmpty(data))
            {
                return data;
            }

            SecretContext secretContext = secretCore.GetSecret(session, version);
            if (secretContext == null)
            {
                throw new SecretException("secretKey is null");
            }
            if (secretContext.Secret == null)
            {
                return data;
            }


            string separator = null;
            SecurityBiz.GetSeparatorMap().TryGetValue(type, out separator);
            if (separator == null)
            {
                throw new SecretException("type error");
            }

            SecurityCounter.AddSearchCount(type);// 计数器
            if (PHONE.Equals(type))
            {
                return SecurityBiz.SearchPhoneIndex(data, separator, secretContext);
            }
            else
            {
                int compressLen = secretCore.GetCompressLen();
                int slideSize = secretCore.GetSlideSize();
                return SecurityBiz.SearchNormalIndex(data, compressLen, slideSize, secretContext);
            }

        }

        /// <summary>
        /// 加密之后格式。 手机号码格式：$手机号码前3位明文$base64(encrypt(phone后8位))$111$ 
        /// nick格式：~base64(encrypt(nick))~111~
        /// </summary>
        /// <param name="data">明文数据</param>
        /// <param name="type">加密字段类型(例如：nick\phone)</param>
        /// <param name="session">用户身份,用户级加密必填</param>
        /// <param name="version">秘钥历史版本</param>
        /// <returns></returns>
        private string Encrypt(string data, string type, string session, Nullable<Int64> version)
        {
            if (string.IsNullOrEmpty(data))
            {
                return data;
            }
            SecretContext secretContext = secretCore.GetSecret(session, version);
            if (secretContext == null)
            {
                throw new SecretException("secretKey is null");
            }
            if (secretContext.Secret == null)
            {
                return data;
            }

            string separator = null;
            SecurityBiz.GetSeparatorMap().TryGetValue(type, out separator);
            if (separator == null)
            {
                throw new SecretException("type error");
            }

            SecurityCounter.AddEncryptCount(type);// 计数器
            bool isEncryptIndex = secretCore.IsIndexEncrypt(type, version);
            // 支持密文检索
            if (isEncryptIndex)
            {
                if (PHONE.Equals(type))
                {
                    return SecurityBiz.EncryptPhoneIndex(data, separator, secretContext);
                }
                else
                {
                    int compressLen = secretCore.GetCompressLen();
                    int slideSize = secretCore.GetSlideSize();
                    return SecurityBiz.EncryptNormalIndex(data, compressLen, slideSize, separator, secretContext);
                }
            }
            else
            {
                if (PHONE.Equals(type))
                {
                    return SecurityBiz.EncryptPhone(data, separator, secretContext);
                }
                else
                {
                    return SecurityBiz.EncryptNormal(data, separator, secretContext);
                }
            }

        }

        /// <summary>
        /// 批量加密（所有用户共用秘钥）
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IDictionary<string, string> Encrypt(List<string> dataList, string type)
        {
            return Encrypt(dataList, type, null);
        }

        /// <summary>
        /// 批量加密（每个用户单独分配秘钥）
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="type"></param>
        /// <param name="session"></param>
        /// <returns>key=明文数据，value=密文数据</returns>
        public IDictionary<string, string> Encrypt(List<string> dataList, string type, string session)
        {
            if (dataList == null || dataList.Count == 0)
            {
                throw new SecretException("dataList can`t be empty");
            }
            IDictionary<string, string> resultMap = new Dictionary<string, string>();
            foreach (string data in dataList)
            {
                if (!resultMap.ContainsKey(data))
                {
                    string encryptValue = Encrypt(data, type, session, null);
                    resultMap.Add(data, encryptValue);
                }
            }
            return resultMap;
        }

        /// <summary>
        /// 生成自定义session，提供给自主账号使用
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GenerateCustomerSession(long userId)
        {
            return UNDERLINE + userId;
        }

    }
}
