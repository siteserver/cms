using System.Collections.Generic;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater
{
    public class ConvertInfo
    {
        public bool IsAbandon { get; set; }

        public string NewTableName { get; set; }

        public List<TableColumn> NewColumns { get; set; }

        public Dictionary<string, string> ConvertKeyDict { get; set; }

        public Dictionary<string, string> ConvertValueDict { get; set; }
    }
}
