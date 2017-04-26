using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Configuration;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
	public class ModalGovInteractPermissions : BasePageCms
	{
        protected CheckBoxList cblPermissions;

        private int _nodeId;
        private string _userName;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId, string userName)
        {
            return PageUtils.GetOpenWindowString("权限设置", PageUtils.GetWcmUrl(nameof(ModalGovInteractPermissions), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"UserName", userName}
            }), 450, 320);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _nodeId = TranslateUtils.ToInt(Request.QueryString["NodeID"]);
            _userName = Request.QueryString["UserName"];

			if (!IsPostBack)
			{
                var permissionArrayList = new List<string>();
                var permissionsInfo = DataProvider.GovInteractPermissionsDao.GetPermissionsInfo(_userName, _nodeId);
                if (permissionsInfo != null)
                {
                    permissionArrayList = TranslateUtils.StringCollectionToStringList(permissionsInfo.Permissions);
                }

                foreach (PermissionConfig permission in PermissionConfigManager.Instance.GovInteractPermissions)
                {
                    var listItem = new ListItem(permission.Text, permission.Name);
                    if (permissionArrayList.Contains(permission.Name))
                    {
                        listItem.Selected = true;
                    }
                    cblPermissions.Items.Add(listItem);
                }

				
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;
            var permissionsInfo = DataProvider.GovInteractPermissionsDao.GetPermissionsInfo(_userName, _nodeId);

            try
            {
                if (permissionsInfo == null)
                {
                    permissionsInfo = new GovInteractPermissionsInfo(_userName, _nodeId, ControlUtils.GetSelectedListControlValueCollection(cblPermissions));
                    DataProvider.GovInteractPermissionsDao.Insert(PublishmentSystemId, permissionsInfo);
                }
                else
                {
                    permissionsInfo.Permissions = ControlUtils.GetSelectedListControlValueCollection(cblPermissions);
                    DataProvider.GovInteractPermissionsDao.Update(permissionsInfo);
                }

                Body.AddSiteLog(PublishmentSystemId, "设置互动交流管理员权限", $"互动交流类别:{NodeManager.GetNodeName(PublishmentSystemId, _nodeId)}");

                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "互动交流权限设置失败！");
            }

			if (isChanged)
			{
                PageUtils.CloseModalPageAndRedirect(Page, PageGovInteractPermissions.GetRedirectUrl(PublishmentSystemId, _nodeId));
			}
		}
	}
}
