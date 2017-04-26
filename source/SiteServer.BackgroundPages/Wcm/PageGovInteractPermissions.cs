using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Wcm.GovInteract;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovInteractPermissions : BasePageGovInteract
    {
        public DataGrid dgContents;

        private int _nodeId;

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovInteractPermissions), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString() },
                {"NodeID", nodeId.ToString() }
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            _nodeId = TranslateUtils.ToInt(Request.QueryString["NodeID"]);

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovInteract, AppManager.Wcm.LeftMenu.GovInteract.IdGovInteractConfiguration, "负责人员设置", AppManager.Wcm.Permission.WebSite.GovInteractConfiguration);

                var channelInfo = DataProvider.GovInteractChannelDao.GetChannelInfo(PublishmentSystemId, _nodeId);
                var departmentIdList = GovInteractManager.GetFirstDepartmentIdList(channelInfo);
                var userNameArrayList = new ArrayList();
                foreach (var departmentId in departmentIdList)
                {
                    userNameArrayList.AddRange(BaiRongDataProvider.AdministratorDao.GetUserNameArrayList(departmentId, true));
                }

                dgContents.DataSource = userNameArrayList;
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();
            }
        }

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var userName = e.Item.DataItem as string;
                var administratorInfo = BaiRongDataProvider.AdministratorDao.GetByUserName(userName);
                var permissionsInfo = DataProvider.GovInteractPermissionsDao.GetPermissionsInfo(userName, _nodeId);

                var ltlDepartmentName = e.Item.FindControl("ltlDepartmentName") as Literal;
                var ltlUserName = e.Item.FindControl("ltlUserName") as Literal;
                var ltlDisplayName = e.Item.FindControl("ltlDisplayName") as Literal;
                var ltlPermissions = e.Item.FindControl("ltlPermissions") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlDepartmentName.Text = DepartmentManager.GetDepartmentName(administratorInfo.DepartmentId);
                ltlUserName.Text = userName;
                ltlDisplayName.Text = administratorInfo.DisplayName;

                if (permissionsInfo != null)
                {
                    var permissionNameArrayList = new ArrayList();
                    var permissionArrayList = TranslateUtils.StringCollectionToStringList(permissionsInfo.Permissions);
                    foreach (string permission in permissionArrayList)
                    {
                        permissionNameArrayList.Add(AppManager.Wcm.Permission.GovInteract.GetPermissionName(permission));
                    }
                    ltlPermissions.Text = TranslateUtils.ObjectCollectionToString(permissionNameArrayList);
                }

                ltlEditUrl.Text =
                    $@"<a href='javascript:;' onclick=""{ModalGovInteractPermissions.GetOpenWindowString(
                        PublishmentSystemId, _nodeId, userName)}"">设置权限</a>";
            }
        }
    }
}
