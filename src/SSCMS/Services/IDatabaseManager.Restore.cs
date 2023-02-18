using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Services
{
    public partial interface IDatabaseManager
    {
        Task<List<string>> RestoreAsync(IConsoleUtils console, List<string> includes, List<string> excludes, string tablesFilePath, Tree tree, string errorLogFilePath);
    }
}
