using BaiRong.Core;
using BaiRong.Core.Table;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core
{
	public class ArchiveManager
	{
        public static void CreateArchiveTableIfNotExists(PublishmentSystemInfo publishmentSystemInfo, string tableName)
        {
            if (!BaiRongDataProvider.DatabaseDao.IsTableExists(TableMetadataManager.GetTableNameOfArchive(tableName)))
            {
                try
                {
                    BaiRongDataProvider.TableCollectionDao.CreateDbTableOfArchive(tableName);
                }
                catch
                {
                    // ignored
                }
            }
        }
	}
}
