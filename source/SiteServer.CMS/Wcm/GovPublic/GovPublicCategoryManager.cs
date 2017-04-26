using System.Collections;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Wcm.GovPublic
{
	public class GovPublicCategoryManager
	{
        private GovPublicCategoryManager()
		{
            
		}

        public static void Initialize(PublishmentSystemInfo publishmentSystemInfo)
        {
            if (DataProvider.GovPublicCategoryClassDao.GetCount(publishmentSystemInfo.PublishmentSystemId) == 0)
            {
                var categoryClassInfoArrayList = new ArrayList();
                categoryClassInfoArrayList.Add(GetCategoryClassInfo(EGovPublicCategoryClassType.Channel, publishmentSystemInfo.PublishmentSystemId));
                categoryClassInfoArrayList.Add(GetCategoryClassInfo(EGovPublicCategoryClassType.Department, publishmentSystemInfo.PublishmentSystemId));
                categoryClassInfoArrayList.Add(GetCategoryClassInfo(EGovPublicCategoryClassType.Form, publishmentSystemInfo.PublishmentSystemId));
                categoryClassInfoArrayList.Add(GetCategoryClassInfo(EGovPublicCategoryClassType.Service, publishmentSystemInfo.PublishmentSystemId));

                foreach (GovPublicCategoryClassInfo categoryClassInfo in categoryClassInfoArrayList)
                {
                    DataProvider.GovPublicCategoryClassDao.Insert(categoryClassInfo);
                }
            }
        }

        private static GovPublicCategoryClassInfo GetCategoryClassInfo(EGovPublicCategoryClassType categoryType, int publishmentSystemID)
        {
            var isSystem = false;
            if (categoryType == EGovPublicCategoryClassType.Channel || categoryType == EGovPublicCategoryClassType.Department)
            {
                isSystem = true;
            }
            return new GovPublicCategoryClassInfo(EGovPublicCategoryClassTypeUtils.GetValue(categoryType), publishmentSystemID, EGovPublicCategoryClassTypeUtils.GetText(categoryType), isSystem, true, string.Empty, 0, string.Empty);
        }
	}
}
