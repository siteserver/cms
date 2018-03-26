using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Core
{
	public class CountManager
	{
		private CountManager()
		{
		}

		public static void AddCount(string relatedTableName, string relatedIdentity, ECountType countType)
		{
            if (DataProvider.CountDao.IsExists(relatedTableName, relatedIdentity, countType))
			{
                DataProvider.CountDao.AddCountNum(relatedTableName, relatedIdentity, countType);
			}
			else
			{
                DataProvider.CountDao.Insert(relatedTableName, relatedIdentity, countType, 1);
			}
		}

		public static void DeleteByRelatedTableName(string relatedTableName)
		{
            DataProvider.CountDao.DeleteByRelatedTableName(relatedTableName);
		}

		public static void DeleteByIdentity(string relatedTableName, string relatedIdentity)
		{
            DataProvider.CountDao.DeleteByIdentity(relatedTableName, relatedIdentity);
		}

        public static int GetCount(string relatedTableName, string relatedIdentity, ECountType countType)
        {
            return DataProvider.CountDao.GetCountNum(relatedTableName, relatedIdentity, countType);
        }

        public static int GetCount(string relatedTableName, int siteId, ECountType countType)
        {
            return DataProvider.CountDao.GetCountNum(relatedTableName, siteId, countType);
        }
	}
}
