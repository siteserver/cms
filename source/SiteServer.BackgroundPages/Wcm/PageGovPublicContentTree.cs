using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovPublic;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicContentTree : BasePageGovPublic
    {
        public Literal LtlCategoryChannel;
        public Repeater RptCategoryChannel;
        public Repeater RptCategoryClass;

        public void Page_Load(object sender, EventArgs e)
        {
            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (IsPostBack) return;

            LtlCategoryChannel.Text = string.Format($@"<a href='{PageContent.GetRedirectUrl(PublishmentSystemId, PublishmentSystemInfo.Additional.GovPublicNodeId)}' isLink='true' onclick='fontWeightLink(this)' target='content'>主题分类</a>");

            ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(PublishmentSystemInfo, ELoadingType.GovPublicChannelTree, null));

            var additional = new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {
                    "DepartmentIDCollection",
                    TranslateUtils.ObjectCollectionToString(
                        GovPublicManager.GetFirstDepartmentIdList(PublishmentSystemInfo))
                }
            };

            ClientScriptRegisterClientScriptBlock("DepartmentTreeScript", DepartmentTreeItem.GetScript(EDepartmentLoadingType.ContentTree, additional));

            var categoryClassInfoArrayList = DataProvider.GovPublicCategoryClassDao.GetCategoryClassInfoArrayList(PublishmentSystemId, ETriState.False, ETriState.True);
            foreach (GovPublicCategoryClassInfo categoryClassInfo in categoryClassInfoArrayList)
            {
                ClientScriptRegisterClientScriptBlock("CategoryTreeScript_" + categoryClassInfo.ClassCode, GovPublicCategoryTreeItem.GetScript(categoryClassInfo.ClassCode, PublishmentSystemId, EGovPublicCategoryLoadingType.Tree, null));
            }

            BindGrid(categoryClassInfoArrayList);
        }

        public void BindGrid(ArrayList categoryClassInfoArrayList)
        {
            try
            {
                RptCategoryChannel.DataSource = DataProvider.NodeDao.GetNodeIdListByParentId(PublishmentSystemId, PublishmentSystemInfo.Additional.GovPublicNodeId);
                RptCategoryChannel.ItemDataBound += RptCategoryChannel_ItemDataBound;
                RptCategoryChannel.DataBind();

                RptCategoryClass.DataSource = categoryClassInfoArrayList;
                RptCategoryClass.ItemDataBound += RptCategoryClass_ItemDataBound;
                RptCategoryClass.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        private void RptCategoryClass_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var categoryClassInfo = (GovPublicCategoryClassInfo)e.Item.DataItem;

            var ltlClassName = (Literal)e.Item.FindControl("ltlClassName");
            var ltlPlusIcon = (Literal)e.Item.FindControl("ltlPlusIcon");

            ltlClassName.Text = categoryClassInfo.ClassName;
            ltlPlusIcon.Text =
                $@"<img align=""absmiddle"" style=""cursor:pointer"" onClick=""displayChildren_{categoryClassInfo.ClassCode}(this);"" isAjax=""true"" isOpen=""false"" id=""0"" src=""../assets/icons/tree/plus.gif"" />";
        }

        private void RptCategoryChannel_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var nodeId = (int)e.Item.DataItem;
            var enabled = IsOwningNodeId(nodeId);
            if (!enabled)
            {
                if (!IsHasChildOwningNodeId(nodeId)) e.Item.Visible = false;
            }
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(PublishmentSystemInfo, nodeInfo, enabled, ELoadingType.GovPublicChannelTree, null, Body.AdministratorName);
        }
    }
}
