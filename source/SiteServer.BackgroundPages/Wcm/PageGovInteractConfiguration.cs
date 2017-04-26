using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovInteractConfiguration : BasePageGovInteract
    {
        public DropDownList ddlGovInteractNodeID;
        public RadioButtonList rblGovInteractApplyIsOpenWindow;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovInteractConfiguration), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
		{
            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovInteract, AppManager.Wcm.LeftMenu.GovInteract.IdGovInteractConfiguration, "互动交流设置", AppManager.Wcm.Permission.WebSite.GovInteractConfiguration);

                AddListItemsForGovInteract(ddlGovInteractNodeID.Items);
                ControlUtils.SelectListItems(ddlGovInteractNodeID, PublishmentSystemInfo.Additional.GovInteractNodeId.ToString());

                EBooleanUtils.AddListItems(rblGovInteractApplyIsOpenWindow);
                ControlUtils.SelectListItems(rblGovInteractApplyIsOpenWindow, PublishmentSystemInfo.Additional.GovInteractApplyIsOpenWindow.ToString());
			}
		}

        private void AddListItemsForGovInteract(ListItemCollection listItemCollection)
        {
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(PublishmentSystemId, EScopeType.SelfAndChildren, string.Empty, string.Empty);
            var nodeCount = nodeIdList.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (int nodeID in nodeIdList)
            {
                var enabled = true;
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);
                if (!EContentModelTypeUtils.Equals(EContentModelType.GovInteract, nodeInfo.ContentModelId))
                {
                    enabled = false;
                }

                var listitem = new ListItem(NodeManager.GetSelectText(PublishmentSystemInfo, nodeInfo, isLastNodeArray, false, true), nodeInfo.NodeId.ToString());
                if (!enabled)
                {
                    listitem.Attributes.Add("style", "color:gray;");
                }
                listItemCollection.Add(listitem);
            }
        }

		public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                var nodeID = TranslateUtils.ToInt(ddlGovInteractNodeID.SelectedValue);
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);
                if (nodeInfo == null || !EContentModelTypeUtils.Equals(EContentModelType.GovInteract, nodeInfo.ContentModelId))
                {
                    ddlGovInteractNodeID.Items.Clear();
                    AddListItemsForGovInteract(ddlGovInteractNodeID.Items);
                    ControlUtils.SelectListItems(ddlGovInteractNodeID, PublishmentSystemInfo.Additional.GovInteractNodeId.ToString());

                    FailMessage("互动交流设置修改失败，根栏目必须选择互动交流类型栏目！");
                    return;
                }
                PublishmentSystemInfo.Additional.GovInteractNodeId = nodeID;
                PublishmentSystemInfo.Additional.GovInteractApplyIsOpenWindow = TranslateUtils.ToBool(rblGovInteractApplyIsOpenWindow.SelectedValue);
				
				try
				{
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改互动交流设置");

                    SuccessMessage("互动交流设置修改成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "互动交流设置修改失败！");
				}
			}
		}
	}
}
