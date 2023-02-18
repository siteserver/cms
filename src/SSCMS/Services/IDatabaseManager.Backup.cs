using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Services
{
    public partial interface IDatabaseManager
    {
        Task<List<string>> BackupAsync(IConsoleUtils console, List<string> includes, List<string> excludes, int maxRows, int pageSize, Tree tree, string errorLogFilePath);
    }
}
