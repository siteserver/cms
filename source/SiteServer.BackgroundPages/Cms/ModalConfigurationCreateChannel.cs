using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalConfigurationCreateChannel : BasePageCms
    {
        public RadioButtonList IsCreateChannelIfContentChanged;

        protected ListBox NodeIDCollection;

		private int nodeID;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetOpenWindowString("栏目生成设置",
                PageUtils.GetCmsUrl(nameof(ModalConfigurationCreateChannel), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"NodeID", nodeId.ToString()}
                }), 550, 400);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID");
            nodeID = Body.GetQueryInt("NodeID");

			if (!IsPostBack)
			{
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);

                EBooleanUtils.AddListItems(IsCreateChannelIfContentChanged, "生成", "不生成");
                ControlUtils.SelectListItemsIgnoreCase(IsCreateChannelIfContentChanged, nodeInfo.Additional.IsCreateChannelIfContentChanged.ToString());

                //NodeManager.AddListItemsForAddContent(this.NodeIDCollection.Items, base.PublishmentSystemInfo, false);
                NodeManager.AddListItemsForCreateChannel(NodeIDCollection.Items, PublishmentSystemInfo, false, Body.AdministratorName);
                ControlUtils.SelectListItems(NodeIDCollection, TranslateUtils.StringCollectionToStringList(nodeInfo.Additional.CreateChannelIDsIfContentChanged));
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isSuccess = false;

            try
            {
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);

                nodeInfo.Additional.IsCreateChannelIfContentChanged = TranslateUtils.ToBool(IsCreateChannelIfContentChanged.SelectedValue);
                nodeInfo.Additional.CreateChannelIDsIfContentChanged = ControlUtils.GetSelectedListControlValueCollection(NodeIDCollection);

                DataProvider.NodeDao.UpdateNodeInfo(nodeInfo);

                Body.AddSiteLog(PublishmentSystemId, nodeID, 0, "设置栏目变动生成页面", $"栏目:{nodeInfo.NodeName}");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }

            if (isSuccess)
            {
                PageUtils.CloseModalPageAndRedirect(Page, PageConfigurationCreateTrigger.GetRedirectUrl(PublishmentSystemId, nodeID));
            }
        }
	}
}
