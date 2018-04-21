using System.Collections.Generic;
using SiteServer.Cli.Core;

namespace SiteServer.Cli.Updater
{
    public abstract class UpdaterBase
    {
        protected UpdaterBase(string oldFolderName, string newFolderName)
        {
            OldFolderName = oldFolderName;
            NewFolderName = newFolderName;
        }

        protected string OldFolderName { get; }

        protected string NewFolderName { get; }

        public abstract KeyValuePair<string, TableInfo> UpdateTableInfo(string oldTableName, TableInfo oldTableInfo);
    }
}
