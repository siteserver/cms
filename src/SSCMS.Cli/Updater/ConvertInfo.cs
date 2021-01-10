using System;
using System.Collections.Generic;
using Datory;

namespace SSCMS.Cli.Updater
{
    public class ConvertInfo
    {
        public bool IsAbandon { get; set; }

        public string NewTableName { get; set; }

        public List<TableColumn> NewColumns { get; set; }

        public Dictionary<string, string[]> ConvertKeyDict { get; set; }

        public Dictionary<string, string> ConvertValueDict { get; set; }

        public Func<Dictionary<string, object>, Dictionary<string, object>> Process { get; set; }
    }
}
