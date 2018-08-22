using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        protected async Task<Tuple<string, TableInfo>> GetNewTableInfoAsync(string oldTableName, TableInfo oldTableInfo, ConvertInfo converter)
        {
            if (converter == null)
            {
                converter = new ConvertInfo();
            }

            if (converter.IsAbandon)
            {
                await CliUtils.PrintRowAsync(oldTableName, "Abandon", "--");
                return null;
            }

            if (string.IsNullOrEmpty(converter.NewTableName))
            {
                converter.NewTableName = oldTableName;
            }
            if (converter.NewColumns == null || converter.NewColumns.Count == 0)
            {
                converter.NewColumns = oldTableInfo.Columns;
            }

            var newTableInfo = new TableInfo
            {
                Columns = converter.NewColumns,
                TotalCount = oldTableInfo.TotalCount,
                RowFiles = oldTableInfo.RowFiles
            };

            await CliUtils.PrintRowAsync(oldTableName, converter.NewTableName, oldTableInfo.TotalCount.ToString("#,0"));

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
                            TranslateUtils.JsonDeserialize<List<JObject>>(await FileUtils.ReadTextAsync(oldFilePath, Encoding.UTF8));

                        var newRows = UpdateUtils.UpdateRows(oldRows, converter.ConvertKeyDict, converter.ConvertValueDict);

                        await FileUtils.WriteTextAsync(newFilePath, Encoding.UTF8, TranslateUtils.JsonSerialize(newRows));
                    }
                    else
                    {
                        FileUtils.CopyFile(oldFilePath, newFilePath);
                    }
                }
            }

            return new Tuple<string, TableInfo>(converter.NewTableName, newTableInfo);
        }

        public abstract Task<Tuple<string, TableInfo>> UpdateTableInfoAsync(string oldTableName, TableInfo oldTableInfo, List<string> contentTableNameList);
    }
}
