using System;

namespace Top.Api.Security
{

    public class SecretData
    {
        /// <summary>
        ///  明文、h_mac数据
        /// </summary>
        public string OriginalValue { get; set; }
        /// <summary>
        ///  原始base64加密之后的密文数据
        /// </summary>
        public string OriginalBase64Value { get; set; }
        /// <summary>
        /// 秘钥版本
        /// </summary>
        public Nullable<Int64> SecretVersion { get; set; }
        /// <summary>
        /// 搜索
        /// </summary>
        public bool Search { get; set; }
    }
}
