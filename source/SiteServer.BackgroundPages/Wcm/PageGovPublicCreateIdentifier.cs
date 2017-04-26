using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicCreateIdentifier : BasePageGovPublic
    {
        public DropDownList ddlNodeID;
        public RadioButtonList rblCreateType;

		public void Page_Load(object sender, EventArgs e)
		{
            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicContentConfiguration, "重新生成索引号", AppManager.Wcm.Permission.WebSite.GovPublicContentConfiguration);

                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, PublishmentSystemInfo.Additional.GovPublicNodeId);
                var listItem = new ListItem("└" + nodeInfo.NodeName, PublishmentSystemInfo.Additional.GovPublicNodeId.ToString());
                ddlNodeID.Items.Add(listItem);

                var nodeIdList = DataProvider.NodeDao.GetNodeIdListByParentId(PublishmentSystemId, PublishmentSystemInfo.Additional.GovPublicNodeId);
                var index = 1;
                foreach (int nodeID in nodeIdList)
                {
                    nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);
                    listItem = new ListItem("　├" + nodeInfo.NodeName, nodeID.ToString());
                    if (index++ == nodeIdList.Count)
                    {
                        listItem = new ListItem("　└" + nodeInfo.NodeName, nodeID.ToString());
                    }
                    if (!EContentModelTypeUtils.Equals(nodeInfo.ContentModelId, EContentModelType.GovPublic))
                    {
                        listItem.Attributes.Add("style", "color:gray;");
                        listItem.Value = "";
                    }
                    ddlNodeID.Items.Add(listItem);
                }

                listItem = new ListItem("索引号为空的信息", "Empty");
                listItem.Selected = true;
                rblCreateType.Items.Add(listItem);
                listItem = new ListItem("全部信息", "All");
                listItem.Selected = false;
                rblCreateType.Items.Add(listItem);
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
            if (Page.IsPostBack && Page.IsValid)
            {
                var nodeID = TranslateUtils.ToInt(ddlNodeID.SelectedValue);
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);
                if (nodeInfo == null || !EContentModelTypeUtils.Equals(EContentModelType.GovPublic, nodeInfo.ContentModelId))
                {
                    FailMessage("索引号生成失败，所选栏目必须为信息公开类型栏目！");
                    return;
                }

                try
                {
                    var isAll = StringUtils.EqualsIgnoreCase(rblCreateType.SelectedValue, "All");
                    DataProvider.GovPublicContentDao.CreateIdentifier(PublishmentSystemInfo, nodeID, isAll);

                    Body.AddSiteLog(PublishmentSystemId, "重新生成索引号");

                    SuccessMessage("索引号重新生成成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "索引号重新生成失败！");
                }
            }
		}
	}
}
