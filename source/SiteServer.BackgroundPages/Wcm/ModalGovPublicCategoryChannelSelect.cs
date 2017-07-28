using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Wcm
{
	public class ModalGovPublicCategoryChannelSelect : BasePageCms
	{
        public Repeater RptCategory;

	    public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
	    {
	        return PageUtils.GetOpenWindowString("选择分类",
	            PageUtils.GetWcmUrl(nameof(ModalGovPublicCategoryChannelSelect), new NameValueCollection
	            {
	                {"PublishmentSystemID", publishmentSystemId.ToString()},
	                {"NodeID", nodeId.ToString()}
	            }), 500, 360);
	    }

	    public static string GetOpenWindowString(int publishmentSystemId)
	    {
	        return PageUtils.GetOpenWindowString("设置分类",
	            PageUtils.GetWcmUrl(nameof(ModalGovPublicCategoryChannelSelect), new NameValueCollection
	            {
	                {"PublishmentSystemID", publishmentSystemId.ToString()}
	            }), 460, 360, true);
	    }

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetWcmUrl(nameof(ModalGovPublicCategoryChannelSelect), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"NodeID", nodeId.ToString()}
                });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
                if (Body.IsQueryExists("NodeID"))
                {
                    var nodeId = TranslateUtils.ToInt(Request.QueryString["NodeID"]);
                    var nodeNames = NodeManager.GetNodeNameNavigationByGovPublic(PublishmentSystemId, nodeId);
                    string scripts = $"window.parent.showCategoryChannel('{nodeNames}', '{nodeId}');";
                    PageUtils.CloseModalPageWithoutRefresh(Page, scripts);
                }
                else
                {
                    ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(PublishmentSystemInfo, ELoadingType.GovPublicChannelAdd, null));
                    BindGrid();
                }
			}
		}

        public void BindGrid()
        {
            try
            {
                RptCategory.DataSource = DataProvider.NodeDao.GetNodeIdListByParentId(PublishmentSystemId, PublishmentSystemInfo.Additional.GovPublicNodeId);
                RptCategory.ItemDataBound += rptCategory_ItemDataBound;
                RptCategory.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        private void rptCategory_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var nodeId = (int)e.Item.DataItem;
            var enabled = IsOwningNodeId(nodeId);
            if (!enabled)
            {
                if (!IsHasChildOwningNodeId(nodeId)) e.Item.Visible = false;
            }
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(PublishmentSystemInfo, nodeInfo, enabled, ELoadingType.GovPublicChannelAdd, null, Body.AdministratorName);
        }
	}
}
