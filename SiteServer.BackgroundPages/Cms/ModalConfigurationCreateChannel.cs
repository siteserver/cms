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
        public DropDownList DdlIsCreateChannelIfContentChanged;

        protected ListBox LbNodeId;

		private int _nodeId;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
        {
            return LayerUtils.GetOpenScript("栏目生成设置",
                PageUtils.GetCmsUrl(nameof(ModalConfigurationCreateChannel), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"NodeID", nodeId.ToString()}
                }), 550, 500);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID");
            _nodeId = Body.GetQueryInt("NodeID");

			if (!IsPostBack)
			{
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);

                EBooleanUtils.AddListItems(DdlIsCreateChannelIfContentChanged, "生成", "不生成");
                ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateChannelIfContentChanged, nodeInfo.Additional.IsCreateChannelIfContentChanged.ToString());

                //NodeManager.AddListItemsForAddContent(this.NodeIDCollection.Items, base.PublishmentSystemInfo, false);
                NodeManager.AddListItemsForCreateChannel(LbNodeId.Items, PublishmentSystemInfo, false, Body.AdminName);
                ControlUtils.SelectMultiItems(LbNodeId, TranslateUtils.StringCollectionToStringList(nodeInfo.Additional.CreateChannelIDsIfContentChanged));
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isSuccess = false;

            try
            {
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);

                nodeInfo.Additional.IsCreateChannelIfContentChanged = TranslateUtils.ToBool(DdlIsCreateChannelIfContentChanged.SelectedValue);
                nodeInfo.Additional.CreateChannelIDsIfContentChanged = ControlUtils.GetSelectedListControlValueCollection(LbNodeId);

                DataProvider.NodeDao.UpdateNodeInfo(nodeInfo);

                Body.AddSiteLog(PublishmentSystemId, _nodeId, 0, "设置栏目变动生成页面", $"栏目:{nodeInfo.NodeName}");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }

            if (isSuccess)
            {
                LayerUtils.CloseAndRedirect(Page, PageConfigurationCreateTrigger.GetRedirectUrl(PublishmentSystemId, _nodeId));
            }
        }
	}
}
