using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageChannel : BasePageCms
    {
        public Repeater rptContents;

        public PlaceHolder PlaceHolder_AddChannel;
        public Button AddChannel1;
        public Button AddChannel2;
        public PlaceHolder PlaceHolder_ChannelEdit;
        public Button AddToGroup;
        public Button SelectEditColumns;
        public PlaceHolder PlaceHolder_Translate;
        public Button Translate;
        public PlaceHolder PlaceHolder_Import;
        public Button Import;
        public Button Export;
        public PlaceHolder PlaceHolder_Delete;
        public Button Delete;
        public PlaceHolder PlaceHolder_Create;
        public Button Create;

        private int _currentNodeId;

        public static string GetRedirectUrl(int publishmentSystemId, int currentNodeId)
        {
            if (currentNodeId > 0 && currentNodeId != publishmentSystemId)
            {
                return PageUtils.GetCmsUrl(nameof(PageChannel), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"CurrentNodeID", currentNodeId.ToString()}
                });
            }
            return PageUtils.GetCmsUrl(nameof(PageChannel), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

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

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdContent, "栏目管理", string.Empty);

                ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(PublishmentSystemInfo, ELoadingType.Channel, null));

                if (Body.IsQueryExists("CurrentNodeID"))
                {
                    _currentNodeId = Body.GetQueryInt("CurrentNodeID");
                    var onLoadScript = ChannelLoading.GetScriptOnLoad(PublishmentSystemId, _currentNodeId);
                    if (!string.IsNullOrEmpty(onLoadScript))
                    {
                        ClientScriptRegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                    }
                }

                ButtonPreLoad();

                BindGrid();
            }
        }

        private void ButtonPreLoad()
        {
            var showPopWinString = string.Empty;

            PlaceHolder_AddChannel.Visible = HasChannelPermissionsIgnoreNodeId(AppManager.Cms.Permission.Channel.ChannelAdd);
            if (PlaceHolder_AddChannel.Visible)
            {
                AddChannel1.Attributes.Add("onclick", ModalChannelAdd.GetOpenWindowString(PublishmentSystemId, PublishmentSystemId, GetRedirectUrl(PublishmentSystemId, PublishmentSystemId)));
                AddChannel2.Attributes.Add("onclick",
                    $"location.href='{PageChannelAdd.GetRedirectUrl(PublishmentSystemId, PublishmentSystemId, GetRedirectUrl(PublishmentSystemId, 0))}';return false;");
            }

            PlaceHolder_ChannelEdit.Visible = HasChannelPermissionsIgnoreNodeId(AppManager.Cms.Permission.Channel.ChannelEdit);
            if (PlaceHolder_ChannelEdit.Visible)
            {
                showPopWinString = ModalAddToGroup.GetOpenWindowStringToChannel(PublishmentSystemId);
                AddToGroup.Attributes.Add("onclick", showPopWinString);

                SelectEditColumns.Attributes.Add("onclick", ModalSelectColumns.GetOpenWindowStringToChannel(PublishmentSystemId, false));
            }

            PlaceHolder_Translate.Visible = HasChannelPermissionsIgnoreNodeId(AppManager.Cms.Permission.Channel.ChannelTranslate);
            if (PlaceHolder_Translate.Visible)
            {
                Translate.Attributes.Add("onclick",
                    PageUtils.GetRedirectStringWithCheckBoxValue(
                        PageChannelTranslate.GetRedirectUrl(PublishmentSystemId,
                            GetRedirectUrl(PublishmentSystemId, _currentNodeId)), "ChannelIDCollection",
                        "ChannelIDCollection", "请选择需要转移的栏目！"));
            }

            PlaceHolder_Delete.Visible = HasChannelPermissionsIgnoreNodeId(AppManager.Cms.Permission.Channel.ChannelDelete);
            if (PlaceHolder_Delete.Visible)
            {
                Delete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValue(PageChannelDelete.GetRedirectUrl(PublishmentSystemId, GetRedirectUrl(PublishmentSystemId, PublishmentSystemId)), "ChannelIDCollection", "ChannelIDCollection", "请选择需要删除的栏目！"));
            }

            PlaceHolder_Create.Visible = AdminUtility.HasWebsitePermissions(Body.AdministratorName, PublishmentSystemId, AppManager.Cms.Permission.WebSite.Create) 
 || HasChannelPermissionsIgnoreNodeId(AppManager.Cms.Permission.Channel.CreatePage);
            if (PlaceHolder_Create.Visible)
            {
                Create.Attributes.Add("onclick", ModalCreateChannels.GetOpenWindowString(PublishmentSystemId));
            }

            PlaceHolder_Import.Visible = PlaceHolder_AddChannel.Visible;
            if (PlaceHolder_Import.Visible)
            {
                Import.Attributes.Add("onclick", ModalChannelImport.GetOpenWindowString(PublishmentSystemId, PublishmentSystemId));
            }
            Export.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToChannel(PublishmentSystemId, "ChannelIDCollection", "请选择需要导出的栏目！"));
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

            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(PublishmentSystemInfo, nodeInfo, enabled, ELoadingType.Channel, null, Body.AdministratorName);
        }
    }
}
