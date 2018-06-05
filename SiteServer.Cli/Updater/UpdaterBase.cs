using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using SiteServer.Cli.Core;
using SiteServer.CMS.Core;
using SiteServer.Utils;

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

        protected KeyValuePair<string, TableInfo> GetNewTableInfo(string oldTableName, TableInfo oldTableInfo, string newTableName, List<TableColumnInfo> newColumns, Dictionary<string, string> convertDict)
        {
            if (string.IsNullOrEmpty(newTableName))
            {
                newTableName = oldTableName;
            }
            if (newColumns == null || newColumns.Count == 0)
            {
                newColumns = oldTableInfo.Columns;
            }

            var newTableInfo = new TableInfo
            {
                Columns = newColumns,
                TotalCount = oldTableInfo.TotalCount,
                RowFiles = oldTableInfo.RowFiles
            };

            CliUtils.PrintRow(oldTableName, newTableName, oldTableInfo.TotalCount.ToString("#,0"));

            var i = 0;

            using (var progress = new ProgressBar())
            {
                foreach (var fileName in oldTableInfo.RowFiles)
                {
                    progress.Report((double)i++ / oldTableInfo.RowFiles.Count);

                    var oldFilePath = OldTreeInfo.GetTableContentFilePath(oldTableName, fileName);
                    var newFilePath = NewTreeInfo.GetTableContentFilePath(newTableName, fileName);

                    if (convertDict != null)
                    {
                        var oldRows =
                            TranslateUtils.JsonDeserialize<List<JObject>>(FileUtils.ReadText(oldFilePath, Encoding.UTF8));

                        var newRows = UpdateUtils.UpdateRows(oldRows, convertDict);

                        FileUtils.WriteText(newFilePath, Encoding.UTF8, TranslateUtils.JsonSerialize(newRows));
                    }
                    else
                    {
                        FileUtils.CopyFile(oldFilePath, newFilePath);
                    }
                }
            }

            return new KeyValuePair<string, TableInfo>(newTableName, newTableInfo);
        }

        public abstract KeyValuePair<string, TableInfo> UpdateTableInfo(string oldTableName, TableInfo oldTableInfo, List<string> contentTableNameList);
    }
}
