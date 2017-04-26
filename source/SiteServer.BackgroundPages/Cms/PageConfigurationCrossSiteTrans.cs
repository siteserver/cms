using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageConfigurationCrossSiteTrans : BasePageCms
    {
        public Repeater rptContents;
        public RadioButtonList IsCrossSiteTransChecked;

        private int _currentNodeId;

        public static string GetRedirectUrl(int publishmentSystemId, int currentNodeId)
        {
            return PageUtils.GetCmsUrl(nameof(PageConfigurationCrossSiteTrans), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"CurrentNodeID", currentNodeId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, "跨站转发设置", AppManager.Cms.Permission.WebSite.Configration);

                ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(PublishmentSystemInfo, ELoadingType.ConfigurationCrossSiteTrans, null));

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

                EBooleanUtils.AddListItems(IsCrossSiteTransChecked, "无需审核", "需要审核");
                ControlUtils.SelectListItems(IsCrossSiteTransChecked, PublishmentSystemInfo.Additional.IsCrossSiteTransChecked.ToString());
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
            var nodeID = (int)e.Item.DataItem;
            var enabled = (IsOwningNodeId(nodeID)) ? true : false;
            if (!enabled)
            {
                if (!IsHasChildOwningNodeId(nodeID)) e.Item.Visible = false;
            }
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);
            var ltlHtml = e.Item.FindControl("ltlHtml") as Literal;
            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(PublishmentSystemInfo, nodeInfo, enabled, ELoadingType.ConfigurationCrossSiteTrans, null, Body.AdministratorName);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                PublishmentSystemInfo.Additional.IsCrossSiteTransChecked = TranslateUtils.ToBool(IsCrossSiteTransChecked.SelectedValue);
				
				try
				{
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改默认跨站转发设置");

                    SuccessMessage("默认跨站转发设置修改成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "默认跨站转发设置修改失败！");
				}
			}
		}
	}
}
