using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core
{
    public static class ArchiveManager
	{
        public static void CreateArchiveTableIfNotExists(SiteInfo siteInfo, string tableName)
        {
            if (!DataProvider.DatabaseDao.IsTableExists(TableMetadataManager.GetTableNameOfArchive(tableName)))
            {
                try
                {
                    DataProvider.TableDao.CreateDbTableOfArchive(tableName);
                }
                catch
                {
                    // ignored
                }
            }
        }
	}
}
