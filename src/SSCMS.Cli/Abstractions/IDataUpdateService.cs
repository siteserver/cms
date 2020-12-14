using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Cli.Core;
using SSCMS.Cli.Updater;
using TableInfo = SSCMS.Cli.Core.TableInfo;

namespace SSCMS.Cli.Abstractions
{
    public interface IDataUpdateService
    {
        void Load(TreeInfo oldTreeInfo, TreeInfo newTreeInfo);

        Task<Tuple<string, TableInfo>> GetNewTableInfoAsync(string oldTableName, TableInfo oldTableInfo,
            ConvertInfo converter);

        Task UpdateSplitContentsTableInfoAsync(Dictionary<int, TableInfo> splitSiteTableDict,
            List<int> siteIdList, string oldTableName, TableInfo oldTableInfo, ConvertInfo converter);

        Task<Tuple<string, TableInfo>> UpdateTableInfoAsync(string oldTableName, TableInfo oldTableInfo);
    }
}
