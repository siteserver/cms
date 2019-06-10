using System;
using System.Collections.Generic;
using System.Text;
using SS.CMS.Data;

namespace SS.CMS.Core.Settings
{
    public class AppSettings
    {
        public bool IsProtectData { get; set; }
        public string ApiPrefix { get; set; }
        public string AdminDirectory { get; set; }
        public string HomeDirectory { get; set; }
        public string SecretKey { get; set; }
        public string IsNightlyUpdate { get; set; }

        public DatabaseSettings Database { get; set; }

        public RedisSettings Redis { get; set; }
    }
}
