using SiteServer.Utils;

namespace SiteServer.Cli.Core
{
    public class TreeInfo
    {
        public string DirectoryPath { get; }

        public TreeInfo(string directory)
        {
            DirectoryPath = PathUtils.Combine(CliUtils.PhysicalApplicationPath, directory);
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
