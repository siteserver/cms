using BaiRong.Core.Model;
using System;

namespace BaiRong.Core
{
    public class ConfigManager
    {
        private const string CacheKey = "BaiRong.Core.ConfigManager";
        private static readonly object LockObject = new object();

        public static ConfigInfo Instance
        {
            get
            {
                var retval = CacheUtils.Get<ConfigInfo>(CacheKey);
                if (retval != null) return retval;

                lock (LockObject)
                {
                    retval = CacheUtils.Get<ConfigInfo>(CacheKey);
                    if (retval == null)
                    {
                        retval = BaiRongDataProvider.ConfigDao.GetConfigInfo();
                        CacheUtils.Insert(CacheKey, retval);
                    }
                }

                return retval;
            }
        }

        public static bool IsChanged
        {
            set
            {
                if (value)
                {
                    CacheUtils.Remove(CacheKey);
                }
            }
        }

        private ConfigManager() { }

        public static UserConfigInfo UserConfigInfo => Instance.UserConfigInfo;

        public static SystemConfigInfo SystemConfigInfo => Instance.SystemConfigInfo;

        public static string Cipherkey
        {
            get
            {
                var cipherkey = Instance.SystemConfigInfo.Cipherkey;
                if (string.IsNullOrEmpty(cipherkey))
                {
                    var s = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'I', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };//枚举数组
                    var r = new Random();
                    for (var i = 0; i < 8; i++)
                    {
                        cipherkey += s[r.Next(0, s.Length)].ToString();
                    }

                    Instance.SystemConfigInfo.Cipherkey = cipherkey;

                    BaiRongDataProvider.ConfigDao.Update(Instance);
                }
                return cipherkey;
            }
        }
    }
}
