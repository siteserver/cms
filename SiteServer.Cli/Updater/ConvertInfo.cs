using System.Collections.Generic;
using Datory;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater
{
    public class ConvertInfo
    {
        public bool IsAbandon { get; set; }

        public string NewTableName { get; set; }

        public List<DatoryColumn> NewColumns { get; set; }

        public Dictionary<string, string> ConvertKeyDict { get; set; }

        public Dictionary<string, string> ConvertValueDict { get; set; }
    }
}
