using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Cli.Core;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Cli.Abstractions
{
    public interface IDataRestoreService
    {
        Task<List<string>> RestoreAsync(IConsoleUtils console, List<string> includes, List<string> excludes, string tablesFilePath,
            Tree tree, string errorLogFilePath);
    }
}
