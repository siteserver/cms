using SiteServer.Utils;
using SiteServer.Utils.Table;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core
{
	public class ArchiveManager
	{
        public static void CreateArchiveTableIfNotExists(PublishmentSystemInfo publishmentSystemInfo, string tableName)
        {
            if (!DataProvider.DatabaseDao.IsTableExists(TableMetadataManager.GetTableNameOfArchive(tableName)))
            {
                try
                {
                    DataProvider.TableCollectionDao.CreateDbTableOfArchive(tableName);
                }
                catch
                {
                    // ignored
                }
            }
        }
	}
}
