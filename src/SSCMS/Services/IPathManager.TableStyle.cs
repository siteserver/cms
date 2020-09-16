using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IPathManager
    {
        Task<string> ExportStylesAsync(int siteId, string tableName, List<int> relatedIdentities);

        Task<string> ImportStylesAsync(string tableName, List<int> relatedIdentities, string zipFilePath);
    }
}
