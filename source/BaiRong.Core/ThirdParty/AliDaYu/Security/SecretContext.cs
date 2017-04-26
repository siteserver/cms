using System;
using Top.Api.Util;

namespace Top.Api.Security
{

    public class SecretContext : SecurityConstants
    {
        /// <summary>
        ///  秘钥
        /// </summary>
        public byte[] Secret { get; set; }
        /// <summary>
        /// 秘钥版本
        /// </summary>
        public Nullable<Int64> SecretVersion { get; set; }
        /// <summary>
        /// 过期时间，单位（毫秒）
        /// </summary>
        public long InvalidTime { get; set; }
        /// <summary>
        /// 最长有效期，单位（毫秒）
        /// </summary>
        public long MaxInvalidTime { get; set; }

        /// <summary>
        /// 判断是否过期
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return InvalidTime > TopUtils.GetCurrentTimeMillis();
        }

        /// <summary>
        /// 容灾，调用api获取秘钥可能会失败，在失败情况下最长有效期
        /// </summary>
        /// <returns></returns>
        public bool IsMaxValid()
        {
            return MaxInvalidTime > TopUtils.GetCurrentTimeMillis();
        }

    }
}
