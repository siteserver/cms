using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    /// <summary>
    /// STL 动态接口参数签名（#3891 / #3892）。
    /// /api/stl/actions/{dynamic,if,more,search,pagecontents} 会在未认证状态下解密并执行请求中的模板，
    /// 旧格式 SecurityKey 退化为仅 8 字符有效的 DES，可被离线爆破后伪造模板。
    /// 这里在密文后追加 HMAC-SHA256(完整 SecurityKey 派生) 签名，服务端验签通过才解密。
    /// </summary>
    public static class StlSignUtils
    {
        private const char Separator = '.';

        private static readonly Regex SqlElementRegex = new Regex(
            @"stl:\s*(page)?sql\s*contents?\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static byte[] GetSignKey(string securityKey)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes("sscms-stl-sign:" + securityKey));
        }

        private static string Sign(string securityKey, string cipherText)
        {
            using var hmac = new HMACSHA256(GetSignKey(securityKey));
            return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(cipherText))).ToLowerInvariant();
        }

        public static string SignedEncrypt(this ISettingsManager settingsManager, string plainText)
        {
            var cipherText = settingsManager.Encrypt(plainText);
            return cipherText + Separator + Sign(settingsManager.SecurityKey, cipherText);
        }

        /// <summary>
        /// 验签后解密；签名缺失或不匹配返回 null。
        /// </summary>
        public static string VerifiedDecrypt(this ISettingsManager settingsManager, string signedValue)
        {
            if (string.IsNullOrEmpty(signedValue)) return null;
            var index = signedValue.LastIndexOf(Separator);
            if (index <= 0 || index == signedValue.Length - 1) return null;

            var cipherText = signedValue.Substring(0, index);
            var signature = signedValue.Substring(index + 1);
            if (!SecureCompareUtils.FixedTimeEquals(Sign(settingsManager.SecurityKey, cipherText), signature))
            {
                return null;
            }

            return settingsManager.Decrypt(cipherText);
        }

        /// <summary>
        /// 动态接口模板中是否包含直接执行 SQL 的标签（stl:sqlContent / stl:sqlContents / stl:pageSqlContents）。
        /// </summary>
        public static bool ContainsSqlElement(string template)
        {
            return !string.IsNullOrEmpty(template) && SqlElementRegex.IsMatch(template);
        }

        /// <summary>
        /// 动态接口默认禁止执行 SQL 类标签；sscms.json 配置 "Stl": { "AllowSqlInDynamic": true } 可放开。
        /// </summary>
        public static bool IsSqlBlocked(IServiceProvider services, params string[] templates)
        {
            var configuration = services?.GetService(typeof(IConfiguration)) as IConfiguration;
            if (configuration != null && configuration.GetValue("Stl:AllowSqlInDynamic", false)) return false;
            foreach (var template in templates)
            {
                if (ContainsSqlElement(template)) return true;
            }
            return false;
        }
    }
}
