using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicChannel : BasePageGovPublic
    {
        public Repeater RptContents;

        public Button AddChannel;
        public Button Delete;

        private int _currentNodeId;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovPublicChannel), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public static string GetRedirectUrl(int publishmentSystemId, int currentNodeId)
        {
            if (currentNodeId != 0 && currentNodeId != publishmentSystemId)
            {
                return PageUtils.GetWcmUrl(nameof(PageGovPublicChannel), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"CurrentNodeID", currentNodeId.ToString()}
                });
            }

            return PageUtils.GetWcmUrl(nameof(PageGovPublicChannel), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (Body.IsQueryExists("NodeID") && (Body.IsQueryExists("Subtract") || Body.IsQueryExists("Add")))
            {
                var nodeId = Body.GetQueryInt("NodeID");
                if (PublishmentSystemId != nodeId)
                {
                    var isSubtract = Body.IsQueryExists("Subtract");
                    DataProvider.NodeDao.UpdateTaxis(PublishmentSystemId, nodeId, isSubtract);

                    Body.AddSiteLog(PublishmentSystemId, nodeId, 0, "栏目排序" + (isSubtract ? "上升" : "下降"),
                        $"栏目:{NodeManager.GetNodeName(PublishmentSystemId, nodeId)}");

                    PageUtils.Redirect(GetRedirectUrl(PublishmentSystemId, nodeId));
                    return;
                }
            }
            else if (Body.IsQueryExists("Delete") && Body.IsQueryExists("ChannelIDCollection"))
            {
                var channelIdList = TranslateUtils.StringCollectionToIntList(Body.GetPostString("ChannelIDCollection"));
                if (channelIdList.Count > 0)
                {
                    foreach (var nodeId in channelIdList)
                    {
                        DataProvider.NodeDao.Delete(nodeId);
                    }
                }
                PageUtils.Redirect(GetRedirectUrl(PublishmentSystemId, 0));
                return;
            }

            if (IsPostBack) return;

            BreadCrumbWithItemTitle(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicContentConfiguration, "分类法管理", "主题分类", AppManager.Wcm.Permission.WebSite.GovPublicContentConfiguration);

            ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(PublishmentSystemInfo, ELoadingType.GovPublicChannel, null));

            if (Body.IsQueryExists("CurrentNodeID"))
            {
                _currentNodeId = TranslateUtils.ToInt(Request.QueryString["CurrentNodeID"]);
                var onLoadScript = ChannelLoading.GetScriptOnLoad(PublishmentSystemId, _currentNodeId);
                if (!string.IsNullOrEmpty(onLoadScript))
                {
                    ClientScriptRegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                }
            }

            AddChannel.Attributes.Add("onclick", ModalGovPublicChannelAdd.GetOpenWindowStringToAdd(PublishmentSystemId, string.Empty));

            Delete.Attributes.Add("onclick",
                PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                    PageUtils.GetWcmUrl(nameof(PageGovPublicChannel), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"Delete", true.ToString()}
                    }), "ChannelIDCollection", "ChannelIDCollection", "请选择需要删除的节点！", "此操作将删除对应节点以及所有下级节点，确认删除吗？"));

            BindGrid();
        }

        public void BindGrid()
        {
            try
            {
                RptContents.DataSource = DataProvider.NodeDao.GetNodeIdListByParentId(PublishmentSystemId, PublishmentSystemInfo.Additional.GovPublicNodeId);
                RptContents.ItemDataBound += rptContents_ItemDataBound;
                RptContents.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var nodeId = (int)e.Item.DataItem;
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            var enabled = EContentModelTypeUtils.Equals(EContentModelType.GovPublic, nodeInfo.ContentModelId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(PublishmentSystemInfo, nodeInfo, enabled, ELoadingType.GovPublicChannel, null, Body.AdministratorName);
        }
    }
}
