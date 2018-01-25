using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
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

        public static string GetRedirectUrl(int siteId, int currentNodeId)
        {
            if (currentNodeId > 0 && currentNodeId != siteId)
            {
                return PageUtils.GetCmsUrl(siteId, nameof(PageChannel), new NameValueCollection
                {
                    {"CurrentNodeID", currentNodeId.ToString()}
                });
            }
            return PageUtils.GetCmsUrl(siteId, nameof(PageChannel), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (Body.IsQueryExists("NodeID") && (Body.IsQueryExists("Subtract") || Body.IsQueryExists("Add")))
            {
                var nodeId = Body.GetQueryInt("NodeID");
                if (SiteId != nodeId)
                {
                    var isSubtract = Body.IsQueryExists("Subtract");
                    DataProvider.ChannelDao.UpdateTaxis(SiteId, nodeId, isSubtract);

                    Body.AddSiteLog(SiteId, nodeId, 0, "栏目排序" + (isSubtract ? "上升" : "下降"),
                        $"栏目:{ChannelManager.GetChannelName(SiteId, nodeId)}");

                    PageUtils.Redirect(GetRedirectUrl(SiteId, nodeId));
                    return;
                }
            }

            if (IsPostBack) return;

            ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(SiteInfo, ELoadingType.Channel, null));

            if (Body.IsQueryExists("CurrentNodeID"))
            {
                _currentNodeId = Body.GetQueryInt("CurrentNodeID");
                var onLoadScript = ChannelLoading.GetScriptOnLoad(SiteId, _currentNodeId);
                if (!string.IsNullOrEmpty(onLoadScript))
                {
                    ClientScriptRegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                }
            }

            PhAddChannel.Visible = HasChannelPermissionsIgnoreNodeId(ConfigManager.Permissions.Channel.ChannelAdd);
            if (PhAddChannel.Visible)
            {
                BtnAddChannel1.Attributes.Add("onclick", ModalChannelsAdd.GetOpenWindowString(SiteId, SiteId, GetRedirectUrl(SiteId, SiteId)));
                BtnAddChannel2.Attributes.Add("onclick",
                    $"location.href='{PageChannelAdd.GetRedirectUrl(SiteId, SiteId, GetRedirectUrl(SiteId, 0))}';return false;");
            }

            PhChannelEdit.Visible = HasChannelPermissionsIgnoreNodeId(ConfigManager.Permissions.Channel.ChannelEdit);
            if (PhChannelEdit.Visible)
            {
                var showPopWinString = ModalAddToGroup.GetOpenWindowStringToChannel(SiteId);
                BtnAddToGroup.Attributes.Add("onclick", showPopWinString);

                BtnSelectEditColumns.Attributes.Add("onclick", ModalSelectColumns.GetOpenWindowStringToChannel(SiteId, false));
            }

            PhTranslate.Visible = HasChannelPermissionsIgnoreNodeId(ConfigManager.Permissions.Channel.ChannelTranslate);
            if (PhTranslate.Visible)
            {
                BtnTranslate.Attributes.Add("onclick",
                    PageUtils.GetRedirectStringWithCheckBoxValue(
                        PageChannelTranslate.GetRedirectUrl(SiteId,
                            GetRedirectUrl(SiteId, _currentNodeId)), "ChannelIDCollection",
                        "ChannelIDCollection", "请选择需要转移的栏目！"));
            }

            PhDelete.Visible = HasChannelPermissionsIgnoreNodeId(ConfigManager.Permissions.Channel.ChannelDelete);
            if (PhDelete.Visible)
            {
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValue(PageChannelDelete.GetRedirectUrl(SiteId, GetRedirectUrl(SiteId, SiteId)), "ChannelIDCollection", "ChannelIDCollection", "请选择需要删除的栏目！"));
            }

            PhCreate.Visible = HasSitePermissions(ConfigManager.Permissions.WebSite.Create) || HasChannelPermissionsIgnoreNodeId(ConfigManager.Permissions.Channel.CreatePage);
            if (PhCreate.Visible)
            {
                BtnCreate.Attributes.Add("onclick", ModalCreateChannels.GetOpenWindowString(SiteId));
            }

            PhImport.Visible = PhAddChannel.Visible;
            if (PhImport.Visible)
            {
                BtnImport.Attributes.Add("onclick", ModalChannelImport.GetOpenWindowString(SiteId, SiteId));
            }
            BtnExport.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToChannel(SiteId, "ChannelIDCollection", "请选择需要导出的栏目！"));

            RptContents.DataSource = DataProvider.ChannelDao.GetIdListByParentId(SiteId, 0);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var nodeId = (int)e.Item.DataItem;
            var enabled = IsOwningNodeId(nodeId);
            if (!enabled)
            {
                if (!IsHasChildOwningNodeId(nodeId)) e.Item.Visible = false;
            }
            var nodeInfo = ChannelManager.GetChannelInfo(SiteId, nodeId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(SiteInfo, nodeInfo, enabled, ELoadingType.Channel, null, Body.AdminName);
        }
    }
}
