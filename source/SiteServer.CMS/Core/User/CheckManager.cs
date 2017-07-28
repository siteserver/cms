using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Permissions;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core.User
{
	public class CheckManager
	{
        public static KeyValuePair<bool, int> GetUserCheckLevel(string administratorName, PublishmentSystemInfo publishmentSystemInfo, int nodeId)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return new KeyValuePair<bool, int>(true, publishmentSystemInfo.CheckContentLevel);
            }

            var isChecked = false;
            var checkedLevel = 0;
            if (publishmentSystemInfo.IsCheckContentUseLevel == false)
            {
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeId, AppManager.Cms.Permission.Channel.ContentCheck))
                {
                    isChecked = true;
                }
            }
            else
            {
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeId, AppManager.Cms.Permission.Channel.ContentCheckLevel5))
                {
                    isChecked = true;
                }
                else if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeId, AppManager.Cms.Permission.Channel.ContentCheckLevel4))
                {
                    if (publishmentSystemInfo.CheckContentLevel <= 4)
                    {
                        isChecked = true;
                    }
                    else
                    {
                        checkedLevel = 4;
                    }
                }
                else if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeId, AppManager.Cms.Permission.Channel.ContentCheckLevel3))
                {
                    if (publishmentSystemInfo.CheckContentLevel <= 3)
                    {
                        isChecked = true;
                    }
                    else
                    {
                        checkedLevel = 3;
                    }
                }
                else if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeId, AppManager.Cms.Permission.Channel.ContentCheckLevel2))
                {
                    if (publishmentSystemInfo.CheckContentLevel <= 2)
                    {
                        isChecked = true;
                    }
                    else
                    {
                        checkedLevel = 2;
                    }
                }
                else if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeId, AppManager.Cms.Permission.Channel.ContentCheckLevel1))
                {
                    if (publishmentSystemInfo.CheckContentLevel <= 1)
                    {
                        isChecked = true;
                    }
                    else
                    {
                        checkedLevel = 1;
                    }
                }
                else
                {
                    checkedLevel = 0;
                }
            }
            return new KeyValuePair<bool, int>(isChecked, checkedLevel);
        }

        public static bool GetUserCheckLevel(string administratorName, PublishmentSystemInfo publishmentSystemInfo, int nodeId, out int userCheckedLevel)
        {
            var checkContentLevel = publishmentSystemInfo.CheckContentLevel;

            var pair = GetUserCheckLevel(administratorName, publishmentSystemInfo, nodeId);
            var isChecked = pair.Key;
            var checkedLevel = pair.Value;
            if (isChecked)
            {
                checkedLevel = checkContentLevel;
            }
            userCheckedLevel = checkedLevel;
            return isChecked;
        }

        public static List<KeyValuePair<int, int>> GetUserCountListUnChecked(string administratorName)
        {
            var list = new List<KeyValuePair<int, int>>();

            var tableEnNameList = BaiRongDataProvider.TableCollectionDao.GetTableEnNameListCreatedInDbByAuxiliaryTableType(EAuxiliaryTableType.BackgroundContent, EAuxiliaryTableType.GovPublicContent, EAuxiliaryTableType.GovInteractContent, EAuxiliaryTableType.VoteContent, EAuxiliaryTableType.JobContent);

            foreach (var tableEnName in tableEnNameList)
            {
                list.AddRange(GetUserCountListUnChecked(administratorName, tableEnName));
            }

            return list;
        }

        private static List<KeyValuePair<int, int>> GetUserCountListUnChecked(string administratorName, string tableName)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            return DataProvider.BackgroundContentDao.GetCountArrayListUnChecked(permissions.IsSystemAdministrator, administratorName, ProductPermissionsManager.Current.PublishmentSystemIdList, ProductPermissionsManager.Current.OwningNodeIdList, tableName);
        }
	}
}
