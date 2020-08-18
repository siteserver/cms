using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Core
{
    public class TreeInfo
    {
        public string DirectoryPath { get; }

        public TreeInfo(ISettingsManager settingsManager,  string directory)
        {
            DirectoryPath = PathUtils.Combine(settingsManager.ContentRootPath, directory);
        }

        public string TablesFilePath => PathUtils.Combine(DirectoryPath, "_tables.json");

        public string GetTableMetadataFilePath(string tableName)
        {
            return PathUtils.Combine(DirectoryPath, tableName, "_metadata.json");
        }

        public string GetTableContentFilePath(string tableName, string fileName)
        {
            return PathUtils.Combine(DirectoryPath, tableName, fileName);
        }
    }
}
