using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationCreateTrigger : BasePageCms
    {
        public Repeater RptContents;

        private int _currentNodeId;

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetCmsUrl(nameof(PageConfigurationCreateTrigger), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"CurrentNodeID", nodeId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
			{
                VerifySitePermissions(AppManager.Permissions.WebSite.Create);

                ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(PublishmentSystemInfo, ELoadingType.ConfigurationCreateDetails, null));

                if (Body.IsQueryExists("CurrentNodeID"))
                {
                    _currentNodeId = Body.GetQueryInt("CurrentNodeID");
                    var onLoadScript = ChannelLoading.GetScriptOnLoad(PublishmentSystemId, _currentNodeId);
                    if (!string.IsNullOrEmpty(onLoadScript))
                    {
                        ClientScriptRegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                    }
                }

                BindGrid();
			}
		}

        public void BindGrid()
        {
            RptContents.DataSource = DataProvider.NodeDao.GetNodeIdListByParentId(PublishmentSystemId, 0);
            RptContents.ItemDataBound += rptContents_ItemDataBound;
            RptContents.DataBind();
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var nodeId = (int)e.Item.DataItem;
            var enabled = IsOwningNodeId(nodeId);
            if (!enabled)
            {
                if (!IsHasChildOwningNodeId(nodeId)) e.Item.Visible = false;
            }

            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            var ltlHtml = e.Item.FindControl("ltlHtml") as Literal;
            if (ltlHtml != null)
            {
                ltlHtml.Text = ChannelLoading.GetChannelRowHtml(PublishmentSystemInfo, nodeInfo, enabled, ELoadingType.ConfigurationCreateDetails, null, Body.AdminName);
            }
        }
	}
}
