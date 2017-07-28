using BaiRong.Core.Model;
using System;

namespace BaiRong.Core
{
    public class ConfigManager
    {
        private ConfigManager() { }

        private static ConfigInfo _configInfo;
        private static readonly object LockObject = new object();
        private static bool _async = true;//缓存与数据库不同步

        public static ConfigInfo Instance
        {
            get
            {
                if (_configInfo == null)
                {
                    _configInfo = BaiRongDataProvider.ConfigDao.GetConfigInfo();
                    return _configInfo;
                }
                lock (LockObject)
                {
                    if (_async)
                    {
                        _configInfo = BaiRongDataProvider.ConfigDao.GetConfigInfo();
                        _async = false;
                    }
                }
                return _configInfo;
            }
        }

        public static UserConfigInfo UserConfigInfo => Instance.UserConfigInfo;

        public static SystemConfigInfo SystemConfigInfo => Instance.SystemConfigInfo;

        public static bool IsChanged
        {
            set { _async = value; }
        }

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
