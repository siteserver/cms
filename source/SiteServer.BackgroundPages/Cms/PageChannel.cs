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
        public Repeater RptContents;

        public PlaceHolder PhAddChannel;
        public Button BtnAddChannel1;
        public Button BtnAddChannel2;
        public PlaceHolder PhChannelEdit;
        public Button BtnAddToGroup;
        public Button BtnSelectEditColumns;
        public PlaceHolder PhTranslate;
        public Button BtnTranslate;
        public PlaceHolder PhImport;
        public Button BtnImport;
        public Button BtnExport;
        public PlaceHolder PhDelete;
        public Button BtnDelete;
        public PlaceHolder PhCreate;
        public Button BtnCreate;

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
            PhAddChannel.Visible = HasChannelPermissionsIgnoreNodeId(AppManager.Permissions.Channel.ChannelAdd);
            if (PhAddChannel.Visible)
            {
                BtnAddChannel1.Attributes.Add("onclick", ModalChannelAdd.GetOpenWindowString(PublishmentSystemId, PublishmentSystemId, GetRedirectUrl(PublishmentSystemId, PublishmentSystemId)));
                BtnAddChannel2.Attributes.Add("onclick",
                    $"location.href='{PageChannelAdd.GetRedirectUrl(PublishmentSystemId, PublishmentSystemId, GetRedirectUrl(PublishmentSystemId, 0))}';return false;");
            }

            PhChannelEdit.Visible = HasChannelPermissionsIgnoreNodeId(AppManager.Permissions.Channel.ChannelEdit);
            if (PhChannelEdit.Visible)
            {
                var showPopWinString = ModalAddToGroup.GetOpenWindowStringToChannel(PublishmentSystemId);
                BtnAddToGroup.Attributes.Add("onclick", showPopWinString);

                BtnSelectEditColumns.Attributes.Add("onclick", ModalSelectColumns.GetOpenWindowStringToChannel(PublishmentSystemId, false));
            }

            PhTranslate.Visible = HasChannelPermissionsIgnoreNodeId(AppManager.Permissions.Channel.ChannelTranslate);
            if (PhTranslate.Visible)
            {
                BtnTranslate.Attributes.Add("onclick",
                    PageUtils.GetRedirectStringWithCheckBoxValue(
                        PageChannelTranslate.GetRedirectUrl(PublishmentSystemId,
                            GetRedirectUrl(PublishmentSystemId, _currentNodeId)), "ChannelIDCollection",
                        "ChannelIDCollection", "请选择需要转移的栏目！"));
            }

            PhDelete.Visible = HasChannelPermissionsIgnoreNodeId(AppManager.Permissions.Channel.ChannelDelete);
            if (PhDelete.Visible)
            {
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValue(PageChannelDelete.GetRedirectUrl(PublishmentSystemId, GetRedirectUrl(PublishmentSystemId, PublishmentSystemId)), "ChannelIDCollection", "ChannelIDCollection", "请选择需要删除的栏目！"));
            }

            PhCreate.Visible = AdminUtility.HasWebsitePermissions(Body.AdministratorName, PublishmentSystemId, AppManager.Permissions.WebSite.Create) 
 || HasChannelPermissionsIgnoreNodeId(AppManager.Permissions.Channel.CreatePage);
            if (PhCreate.Visible)
            {
                BtnCreate.Attributes.Add("onclick", ModalCreateChannels.GetOpenWindowString(PublishmentSystemId));
            }

            PhImport.Visible = PhAddChannel.Visible;
            if (PhImport.Visible)
            {
                BtnImport.Attributes.Add("onclick", ModalChannelImport.GetOpenWindowString(PublishmentSystemId, PublishmentSystemId));
            }
            BtnExport.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToChannel(PublishmentSystemId, "ChannelIDCollection", "请选择需要导出的栏目！"));
        }

        public void BindGrid()
        {
            try
            {
                RptContents.DataSource = DataProvider.NodeDao.GetNodeIdListByParentId(PublishmentSystemId, 0);
                RptContents.ItemDataBound += RptContents_ItemDataBound;
                RptContents.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var nodeId = (int)e.Item.DataItem;
            var enabled = IsOwningNodeId(nodeId);
            if (!enabled)
            {
                if (!IsHasChildOwningNodeId(nodeId)) e.Item.Visible = false;
            }
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(PublishmentSystemInfo, nodeInfo, enabled, ELoadingType.Channel, null, Body.AdministratorName);
        }
    }
}
