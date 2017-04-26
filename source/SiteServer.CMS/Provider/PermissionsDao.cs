using System.Collections;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using System.Collections.Generic;

namespace SiteServer.CMS.Provider
{
    public class PermissionsDao : DataProviderBase
	{
        public void InsertRoleAndPermissions(string roleName, string creatorUserName, string description, List<string> generalPermissionList, List<SystemPermissionsInfo> systemPermissionsInfoList)
		{
			using (var conn = GetConnection()) 
			{
				conn.Open();
				using (var trans = conn.BeginTransaction()) 
				{
					try 
					{
						if (generalPermissionList != null && generalPermissionList.Count > 0)
						{
							var permissionsInRolesInfo = new PermissionsInRolesInfo(roleName, TranslateUtils.ObjectCollectionToString(generalPermissionList));
                            BaiRongDataProvider.PermissionsInRolesDao.InsertWithTrans(permissionsInRolesInfo, trans);
						}

                        foreach (var systemPermissionsInfo in systemPermissionsInfoList)
                        {
                            systemPermissionsInfo.RoleName = roleName;
                            DataProvider.SystemPermissionsDao.InsertWithTrans(systemPermissionsInfo, trans);
                        }

                        trans.Commit();
					}
					catch
					{
						trans.Rollback();
						throw;
					}
				}
			}
            BaiRongDataProvider.RoleDao.InsertRole(roleName, creatorUserName, description);
		}

        public void UpdatePublishmentPermissions(string roleName, List<SystemPermissionsInfo> systemPermissionsInfoList)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        DataProvider.SystemPermissionsDao.DeleteWithTrans(roleName, trans);
                        foreach (var systemPermissionsInfo in systemPermissionsInfoList)
                        {
                            systemPermissionsInfo.RoleName = roleName;
                            DataProvider.SystemPermissionsDao.InsertWithTrans(systemPermissionsInfo, trans);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

		public void DeleteRoleAndPermissions(string roleName)
		{
			using (var conn = GetConnection()) 
			{
				conn.Open();
				using (var trans = conn.BeginTransaction()) 
				{
					try 
					{
                        DataProvider.SystemPermissionsDao.DeleteWithTrans(roleName, trans);

                        BaiRongDataProvider.PermissionsInRolesDao.DeleteWithTrans(roleName, trans);

						trans.Commit();
					}
					catch
					{
						trans.Rollback();
						throw;
					}
				}
			}

            BaiRongDataProvider.RoleDao.DeleteRole(roleName);
        }
    }
}
