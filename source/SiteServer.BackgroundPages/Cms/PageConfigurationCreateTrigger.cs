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
        public Repeater rptContents;

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
                BreadCrumb(AppManager.Cms.LeftMenu.IdCreate, AppManager.Cms.LeftMenu.Create.IdConfigurationCreate, "页面生成触发器", AppManager.Cms.Permission.WebSite.Create);

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
            try
            {
                rptContents.DataSource = DataProvider.NodeDao.GetNodeIdListByParentId(PublishmentSystemId, 0);
                rptContents.ItemDataBound += rptContents_ItemDataBound;
                rptContents.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
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
                ltlHtml.Text = ChannelLoading.GetChannelRowHtml(PublishmentSystemInfo, nodeInfo, enabled, ELoadingType.ConfigurationCreateDetails, null, Body.AdministratorName);
            }
        }
	}
}
