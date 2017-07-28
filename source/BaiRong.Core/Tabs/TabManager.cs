using System.Collections;

namespace BaiRong.Core.Tabs
{
    public class TabManager
	{
        public static bool IsValid(Tab tab, IList permissionList)
        {
            if (!string.IsNullOrEmpty(tab.Ban))
            {
                return false;
                //foreach (string ban in TranslateUtils.StringCollectionToStringList(tab.Ban))
                //{
                //    if (ban.IndexOf(':') != -1)
                //    {
                //        string banProductID = ban.Split(':')[0];
                //        if (StringUtils.EqualsIgnoreCase(productID, banProductID) && ELicenseTypeUtils.Equals(LicenseManager.GetInstance(productID).LicenseType, ban.Split(':')[1]))
                //        {
                //            return false;
                //        }
                //    }
                //    else
                //    {
                //        if (StringUtils.EqualsIgnoreCase(productID, ban))
                //        {
                //            return false;
                //        }
                //    }
                //}
            }
            if (tab.HasPermissions)
            {
                if (permissionList != null && permissionList.Count > 0)
                {
                    var tabPermissions = tab.Permissions.Split(',');
                    foreach (var tabPermission in tabPermissions)
                    {
                        if (permissionList.Contains(tabPermission))
                            return true;
                    }
                }

                //ITab valid, but invalid role set
                return false;
            }

            //ITab valid, but no roles
            return true;
        }
	}
}
