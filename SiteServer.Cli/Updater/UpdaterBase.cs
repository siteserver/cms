using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using SiteServer.Cli.Core;
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

        protected bool GetNewTableInfo(string oldTableName, TableInfo oldTableInfo, ConvertInfo converter, out string newTableName, out TableInfo newTableInfo)
        {
            newTableName = null;
            newTableInfo = null;

            if (converter == null)
            {
                converter = new ConvertInfo();
            }

            if (converter.IsAbandon)
            {
                CliUtils.PrintRow(oldTableName, "Abandon", "--");
                return false;
            }

            if (string.IsNullOrEmpty(converter.NewTableName))
            {
                converter.NewTableName = oldTableName;
            }
            if (converter.NewColumns == null || converter.NewColumns.Count == 0)
            {
                converter.NewColumns = oldTableInfo.Columns;
            }

            newTableInfo = new TableInfo
            {
                Columns = converter.NewColumns,
                TotalCount = oldTableInfo.TotalCount,
                RowFiles = oldTableInfo.RowFiles
            };

            CliUtils.PrintRow(oldTableName, converter.NewTableName, oldTableInfo.TotalCount.ToString("#,0"));

            var i = 0;

            using (var progress = new ProgressBar())
            {
                foreach (var fileName in oldTableInfo.RowFiles)
                {
                    progress.Report((double)i++ / oldTableInfo.RowFiles.Count);

                    var oldFilePath = OldTreeInfo.GetTableContentFilePath(oldTableName, fileName);
                    var newFilePath = NewTreeInfo.GetTableContentFilePath(converter.NewTableName, fileName);

                    if (converter.ConvertKeyDict != null)
                    {
                        var oldRows =
                            TranslateUtils.JsonDeserialize<List<JObject>>(FileUtils.ReadText(oldFilePath, Encoding.UTF8));

                        var newRows = UpdateUtils.UpdateRows(oldRows, converter.ConvertKeyDict, converter.ConvertValueDict);

                        FileUtils.WriteText(newFilePath, Encoding.UTF8, TranslateUtils.JsonSerialize(newRows));
                    }
                    else
                    {
                        FileUtils.CopyFile(oldFilePath, newFilePath);
                    }
                }
            }

            newTableName = converter.NewTableName;

            return true;
        }

        public abstract bool UpdateTableInfo(string oldTableName, TableInfo oldTableInfo, List<string> contentTableNameList, out string newTableName, out TableInfo newTableInfo);
    }
}
