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
    public class PageGovInteractChannel : BasePageGovInteract
    {
        public Repeater rptContents;

        public Button AddChannel;
        public Button Delete;

        private int _currentNodeId;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovInteractChannel), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public static string GetRedirectUrl(int publishmentSystemId, int currentNodeId)
        {
            if (currentNodeId != 0 && currentNodeId != publishmentSystemId)
            {
                return PageUtils.GetWcmUrl(nameof(PageGovInteractChannel), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"CurrentNodeID", currentNodeId.ToString()}
                });
            }
            return PageUtils.GetWcmUrl(nameof(PageGovInteractChannel), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (Request.QueryString["NodeID"] != null && (Request.QueryString["Subtract"] != null || Request.QueryString["Add"] != null))
            {
                var nodeId = int.Parse(Request.QueryString["NodeID"]);
                if (PublishmentSystemId != nodeId)
                {
                    var isSubtract = Request.QueryString["Subtract"] != null;
                    DataProvider.NodeDao.UpdateTaxis(PublishmentSystemId, nodeId, isSubtract);

                    Body.AddSiteLog(PublishmentSystemId, nodeId, 0, "栏目排序" + (isSubtract ? "上升" : "下降"),
                        $"栏目:{NodeManager.GetNodeName(PublishmentSystemId, nodeId)}");

                    PageUtils.Redirect(GetRedirectUrl(PublishmentSystemId, nodeId));
                    return;
                }
            }
            else if (Request.QueryString["Delete"] != null && Request.QueryString["ChannelIDCollection"] != null)
            {
                var channelIdArrayList = TranslateUtils.StringCollectionToIntList(Request.QueryString["ChannelIDCollection"]);
                if (channelIdArrayList.Count > 0)
                {
                    foreach (var nodeId in channelIdArrayList)
                    {
                        DataProvider.NodeDao.Delete(nodeId);
                    }
                }
                PageUtils.Redirect(GetRedirectUrl(PublishmentSystemId, 0));
                return;
            }

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovInteract, AppManager.Wcm.LeftMenu.GovInteract.IdGovInteractConfiguration, "互动交流分类", AppManager.Wcm.Permission.WebSite.GovInteractConfiguration);

                if (PublishmentSystemInfo.Additional.GovInteractNodeId == 0)
                {
                    PageUtils.Redirect(PageGovInteractConfiguration.GetRedirectUrl(PublishmentSystemId));
                    return;
                }

                ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(PublishmentSystemInfo, ELoadingType.GovInteractChannel, null));

                if (Body.IsQueryExists("CurrentNodeID"))
                {
                    _currentNodeId = TranslateUtils.ToInt(Request.QueryString["CurrentNodeID"]);
                    var onLoadScript = ChannelLoading.GetScriptOnLoad(PublishmentSystemId, _currentNodeId);
                    if (!string.IsNullOrEmpty(onLoadScript))
                    {
                        ClientScriptRegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                    }
                }

                AddChannel.Attributes.Add("onclick", ModalGovInteractChannelAdd.GetOpenWindowStringToAdd(PublishmentSystemId, string.Empty));

                Delete.Attributes.Add("onclick",
                    PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                        PageUtils.GetWcmUrl(nameof(PageGovInteractChannel), new NameValueCollection
                        {
                            {"PublishmentSystemID", PublishmentSystemId.ToString()},
                            {"Delete", true.ToString()}
                        }), "ChannelIDCollection", "ChannelIDCollection", "请选择需要删除的节点！", "此操作将删除对应节点以及所有下级节点，确认删除吗？"));

                rptContents.DataSource = DataProvider.NodeDao.GetNodeIdListByParentId(PublishmentSystemId, PublishmentSystemInfo.Additional.GovInteractNodeId);
                rptContents.ItemDataBound += rptContents_ItemDataBound;
                rptContents.DataBind();
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var nodeId = (int)e.Item.DataItem;
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            var enabled = EContentModelTypeUtils.Equals(EContentModelType.GovInteract, nodeInfo.ContentModelId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(PublishmentSystemInfo, nodeInfo, enabled, ELoadingType.GovInteractChannel, null, Body.AdministratorName);
        }
    }
}
