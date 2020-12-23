using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Core.Utils.Serialization;

namespace SSCMS.Core.Services
{
    public partial class PathManager
    {
        public async Task<string> ExportStylesAsync(int siteId, string tableName, List<int> relatedIdentities)
        {
            return await ExportObject.ExportRootSingleTableStyleAsync(this, _databaseManager, siteId, tableName,
                relatedIdentities);
        }

        public async Task ImportStylesByDirectoryAsync(string tableName, List<int> relatedIdentities, string directoryPath)
        {
            await ImportObject.ImportTableStyleByDirectoryAsync(_databaseManager, tableName, relatedIdentities,
                directoryPath);
        }

        public async Task<string> ImportStylesByZipFileAsync(string tableName, List<int> relatedIdentities, string zipFilePath)
        {
            return await ImportObject.ImportTableStyleByZipFileAsync(this, _databaseManager, tableName, relatedIdentities,
                zipFilePath);
        }
    }
}
