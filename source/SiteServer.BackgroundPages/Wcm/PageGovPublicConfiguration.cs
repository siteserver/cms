using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicConfiguration : BasePageGovPublic
    {
        public DropDownList ddlGovPublicNodeID;
        public RadioButtonList rblIsPublisherRelatedDepartmentID;

		public void Page_Load(object sender, EventArgs e)
		{
            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicContentConfiguration, "信息公开设置", AppManager.Wcm.Permission.WebSite.GovPublicContentConfiguration);

                AddListItemsForGovPublic(ddlGovPublicNodeID.Items);
                ControlUtils.SelectListItems(ddlGovPublicNodeID, PublishmentSystemInfo.Additional.GovPublicNodeId.ToString());

                ControlUtils.SelectListItems(rblIsPublisherRelatedDepartmentID, PublishmentSystemInfo.Additional.GovPublicIsPublisherRelatedDepartmentId.ToString());
			}
		}

        private void AddListItemsForGovPublic(ListItemCollection listItemCollection)
        {
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(PublishmentSystemId, EScopeType.SelfAndChildren, string.Empty, string.Empty);
            var nodeCount = nodeIdList.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (int nodeID in nodeIdList)
            {
                var enabled = true;
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);
                if (!EContentModelTypeUtils.Equals(EContentModelType.GovPublic, nodeInfo.ContentModelId))
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
                var nodeID = TranslateUtils.ToInt(ddlGovPublicNodeID.SelectedValue);
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);
                if (nodeInfo == null || !EContentModelTypeUtils.Equals(EContentModelType.GovPublic, nodeInfo.ContentModelId))
                {
                    ddlGovPublicNodeID.Items.Clear();
                    AddListItemsForGovPublic(ddlGovPublicNodeID.Items);
                    ControlUtils.SelectListItems(ddlGovPublicNodeID, PublishmentSystemInfo.Additional.GovPublicNodeId.ToString());

                    FailMessage("信息公开设置修改失败，主题分类根栏目必须选择信息公开类型栏目！");
                    return;
                }
                PublishmentSystemInfo.Additional.GovPublicNodeId = nodeID;
                PublishmentSystemInfo.Additional.GovPublicIsPublisherRelatedDepartmentId = TranslateUtils.ToBool(rblIsPublisherRelatedDepartmentID.SelectedValue);
				
				try
				{
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改信息公开设置");

                    SuccessMessage("信息公开设置修改成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "信息公开设置修改失败！");
				}
			}
		}
	}
}
