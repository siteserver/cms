using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core
{
	public class ArchiveManager
	{
        public static void CreateArchiveTableIfNotExists(PublishmentSystemInfo publishmentSystemInfo, string tableName)
        {
            if (!BaiRongDataProvider.TableStructureDao.IsTableExists(TableManager.GetTableNameOfArchive(tableName)))
            {
                try
                {
                    BaiRongDataProvider.TableMetadataDao.CreateAuxiliaryTableOfArchive(tableName);
                }
                catch { }
            }
        }
	}
}
