using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageChannel : BasePageCms
    {
        public Literal LtlButtonsHead;
        public Literal LtlButtonsFoot;
        public Repeater RptContents;

        private int _currentChannelId;

        public static string GetRedirectUrl(int siteId, int currentChannelId)
        {
            if (currentChannelId > 0 && currentChannelId != siteId)
            {
                return PageUtils.GetCmsUrl(siteId, nameof(PageChannel), new NameValueCollection
                {
                    {"CurrentChannelId", currentChannelId.ToString()}
                });
            }
            return PageUtils.GetCmsUrl(siteId, nameof(PageChannel), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (AuthRequest.IsQueryExists("channelId") && (AuthRequest.IsQueryExists("Subtract") || AuthRequest.IsQueryExists("Add")))
            {
                var channelId = AuthRequest.GetQueryInt("channelId");
                if (SiteId != channelId)
                {
                    var isSubtract = AuthRequest.IsQueryExists("Subtract");
                    DataProvider.ChannelDao.UpdateTaxisAsync(SiteId, channelId, isSubtract).GetAwaiter().GetResult();

                    AuthRequest.AddSiteLogAsync(SiteId, channelId, 0, "栏目排序" + (isSubtract ? "上升" : "下降"),
                        $"栏目:{ChannelManager.GetChannelNameAsync(SiteId, channelId).GetAwaiter().GetResult()}").GetAwaiter().GetResult();

                    PageUtils.Redirect(GetRedirectUrl(SiteId, channelId));
                    return;
                }
            }

            if (IsPostBack) return;

            ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(Site, string.Empty, ELoadingType.Channel, null));

            if (AuthRequest.IsQueryExists("CurrentChannelId"))
            {
                _currentChannelId = AuthRequest.GetQueryInt("CurrentChannelId");
                var onLoadScript = ChannelLoading.GetScriptOnLoad(SiteId, _currentChannelId);
                if (!string.IsNullOrEmpty(onLoadScript))
                {
                    ClientScriptRegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                }
            }

            LtlButtonsHead.Text = LtlButtonsFoot.Text = GetButtonsHtml();

            var channelIdList = ChannelManager.GetChannelIdListAsync(ChannelManager.GetChannelAsync(SiteId, SiteId).GetAwaiter().GetResult(), EScopeType.SelfAndChildren, string.Empty, string.Empty, string.Empty).GetAwaiter().GetResult();

            RptContents.DataSource = channelIdList;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private string GetButtonsHtml()
        {
            var builder = new StringBuilder();

            if (HasChannelPermissionsIgnoreChannelId(Constants.ChannelPermissions.ChannelAdd))
            {
                builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalChannelsAdd.GetOpenWindowString(SiteId, SiteId, GetRedirectUrl(SiteId, SiteId))}"">快速添加</a>
<a href=""{PageChannelAdd.GetRedirectUrl(SiteId, SiteId, GetRedirectUrl(SiteId, 0))}"" class=""btn btn-light text-secondary"">添加栏目</a>
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalChannelImport.GetOpenWindowString(SiteId, SiteId)}"">导 入</a>
");
            }

            builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalExportMessage.GetOpenWindowStringToChannel(SiteId, "ChannelIDCollection", "请选择需要导出的栏目！")}"">导 出</a>
");

            if (HasChannelPermissionsIgnoreChannelId(Constants.ChannelPermissions.ChannelEdit))
            {
                builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalAddToGroup.GetOpenWindowStringToChannel(SiteId)}"">设置栏目组</a>
");

                builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalChannelTaxis.GetOpenWindowString(SiteId)}"">排 序</a>
");
            }

            if (HasChannelPermissionsIgnoreChannelId(Constants.ChannelPermissions.ChannelTranslate))
            {
                builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{
                        PageUtils.GetRedirectStringWithCheckBoxValue(
                            PageChannelTranslate.GetRedirectUrl(SiteId,
                                GetRedirectUrl(SiteId, _currentChannelId)), "ChannelIDCollection",
                            "ChannelIDCollection", "请选择需要转移的栏目！")
                    }"">转 移</a>
");
            }

            if (HasChannelPermissionsIgnoreChannelId(Constants.ChannelPermissions.ChannelDelete))
            {
                builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{
                        PageUtils.GetRedirectStringWithCheckBoxValue(
                            PageChannelDelete.GetRedirectUrl(SiteId, GetRedirectUrl(SiteId, SiteId)), "ChannelIDCollection",
                            "ChannelIDCollection", "请选择需要删除的栏目！")
                    }"">删 除</a>
");
            }

            if (HasSitePermissions(Constants.WebSitePermissions.Create) ||
                HasChannelPermissionsIgnoreChannelId(Constants.ChannelPermissions.CreatePage))
            {
                builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalCreateChannels.GetOpenWindowString(SiteId)}"">生 成</a>
");
            }

            return builder.ToString();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var channelId = (int)e.Item.DataItem;
            var enabled = IsOwningChannelId(channelId);
            if (!enabled)
            {
                if (!IsDescendantOwningChannelId(channelId)) e.Item.Visible = false;
            }
            var nodeInfo = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = ChannelLoading.GetChannelRowHtmlAsync(Site, nodeInfo, enabled, ELoadingType.Channel, null, AuthRequest.AdminPermissionsImpl).GetAwaiter().GetResult();
        }
    }
}
