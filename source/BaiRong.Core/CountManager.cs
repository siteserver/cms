using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core
{
	public class CountManager
	{
		private CountManager()
		{
		}

		public static void AddCount(string relatedTableName, string relatedIdentity, ECountType countType)
		{
            if (BaiRongDataProvider.CountDao.IsExists(relatedTableName, relatedIdentity, countType))
			{
				BaiRongDataProvider.CountDao.AddCountNum(relatedTableName, relatedIdentity, countType);
			}
			else
			{
                BaiRongDataProvider.CountDao.Insert(relatedTableName, relatedIdentity, countType, 1);
			}
		}

		public static void DeleteByRelatedTableName(string relatedTableName)
		{
            BaiRongDataProvider.CountDao.DeleteByRelatedTableName(relatedTableName);
		}

		public static void DeleteByIdentity(string relatedTableName, string relatedIdentity)
		{
            BaiRongDataProvider.CountDao.DeleteByIdentity(relatedTableName, relatedIdentity);
		}

        public static int GetCount(string relatedTableName, string relatedIdentity, ECountType countType)
        {
            return BaiRongDataProvider.CountDao.GetCountNum(relatedTableName, relatedIdentity, countType);
        }

        public static int GetCount(string relatedTableName, int publishmentSystemId, ECountType countType)
        {
            return BaiRongDataProvider.CountDao.GetCountNum(relatedTableName, publishmentSystemId, countType);
        }
	}
}
