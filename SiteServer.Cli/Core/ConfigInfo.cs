using System.Collections.Generic;

namespace SiteServer.Cli.Core
{
    public class BackupConfigInfo
    {
        public List<string> Includes { get; set; }

        public List<string> Excludes { get; set; }
    }

    public class RestoreConfigInfo
    {
        public List<string> Includes { get; set; }

        public List<string> Excludes { get; set; }

        public bool SyncDatabase { get; set; } = true;
    }

    public class ConfigInfo
    {
        public string DatabaseType { get; set; }

        public string ConnectionString { get; set; }

        public BackupConfigInfo BackupConfig { get; set; }

        public RestoreConfigInfo RestoreConfig { get; set; }
    }
}
