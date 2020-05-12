using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Cli.Core;

namespace SSCMS.Cli.Abstractions
{
    public interface IDataRestoreService
    {
        Task RestoreAsync(List<string> includes, List<string> excludes, bool dataOnly, string tablesFilePath,
            TreeInfo treeInfo, string errorLogFilePath);
    }
}
