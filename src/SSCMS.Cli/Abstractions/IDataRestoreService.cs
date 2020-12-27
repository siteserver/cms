using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Cli.Core;

namespace SSCMS.Cli.Abstractions
{
    public interface IDataRestoreService
    {
        Task<List<string>> RestoreAsync(List<string> includes, List<string> excludes, string tablesFilePath,
            TreeInfo treeInfo, string errorLogFilePath);
    }
}
