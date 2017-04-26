using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTemplateFilePathRule : BasePageCms
    {
        public Repeater rptContents;

        private int _currentNodeId;
        private NameValueCollection _additional;

        public static string GetRedirectUrl(int publishmentSystemId, int currentNodeId)
        {
            return PageUtils.GetCmsUrl(nameof(PageTemplateFilePathRule), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"CurrentNodeID", currentNodeId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _additional = new NameValueCollection();

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdCreate, AppManager.Cms.LeftMenu.Create.IdConfigurationCreate, "页面命名规则", AppManager.Cms.Permission.WebSite.Create);

                ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(PublishmentSystemInfo, ELoadingType.TemplateFilePathRule, _additional));

                if (Body.IsQueryExists("CurrentNodeID"))
                {
                    _currentNodeId = Body.GetQueryInt("CurrentNodeID");
                    var onLoadScript = ChannelLoading.GetScriptOnLoad(PublishmentSystemId, _currentNodeId);
                    if (!string.IsNullOrEmpty(onLoadScript))
                    {
                        ClientScriptRegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                    }
                }

                rptContents.DataSource = DataProvider.NodeDao.GetNodeIdListByParentId(PublishmentSystemId, 0);
                rptContents.ItemDataBound += rptContents_ItemDataBound;
                rptContents.DataBind();
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

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(PublishmentSystemInfo, nodeInfo, enabled, ELoadingType.TemplateFilePathRule, _additional, Body.AdministratorName);
        }
	}
}
