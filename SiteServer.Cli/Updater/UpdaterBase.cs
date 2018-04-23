using System.Collections.Generic;
using SiteServer.Cli.Core;

namespace SiteServer.Cli.Updater
{
    public abstract class UpdaterBase
    {
        protected UpdaterBase(TreeInfo oldTreeInfo, TreeInfo newTreeInfo)
        {
            OldTreeInfo = oldTreeInfo;
            NewTreeInfo = newTreeInfo;
        }

        protected TreeInfo OldTreeInfo { get; }

        protected TreeInfo NewTreeInfo { get; }

        public abstract KeyValuePair<string, TableInfo> UpdateTableInfo(string oldTableName, TableInfo oldTableInfo, List<string> contentTableNameList);
    }
}
