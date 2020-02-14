using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalChannelSelect : BasePageCms
    {
        public Literal LtlSite;
        public Repeater RptChannel;

        private readonly NameValueCollection _additional = new NameValueCollection();
        private bool _isProtocol;
        private string _jsMethod;
        private int _itemIndex;

        public static string GetOpenWindowString(int siteId)
        {
            return GetOpenWindowString(siteId, false);
        }

        public static string GetOpenWindowString(int siteId, bool isProtocol)
        {
            return LayerUtils.GetOpenScript("栏目选择",
                PageUtils.GetCmsUrl(siteId, nameof(ModalChannelSelect), new NameValueCollection
                {
                    {"isProtocol", isProtocol.ToString()}
                }), 460, 450);
        }

        public static string GetRedirectUrl(int siteId, int channelId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalChannelSelect), new NameValueCollection
            {
                {"channelId", channelId.ToString()}
            });
        }

        public static string GetOpenWindowStringByItemIndex(int siteId, string jsMethod, string itemIndex)
        {
            return LayerUtils.GetOpenScript("栏目选择",
                PageUtils.GetCmsUrl(siteId, nameof(ModalChannelSelect), new NameValueCollection
                {
                    {"jsMethod", jsMethod},
                    {"itemIndex", itemIndex}
                }), 460, 450);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _isProtocol = AuthRequest.GetQueryBool("isProtocol");
            _jsMethod = AuthRequest.GetQueryString("jsMethod");
            _itemIndex = AuthRequest.GetQueryInt("itemIndex");

            _additional.Add("isProtocol", _isProtocol.ToString());
            _additional.Add("jsMethod", _jsMethod);
            _additional.Add("itemIndex", _itemIndex.ToString());

			if (!IsPostBack)
			{
                if (AuthRequest.IsQueryExists("channelId"))
                {
                    var channelId = AuthRequest.GetQueryInt("channelId");
                    var nodeNames = DataProvider.ChannelRepository.GetChannelNameNavigationAsync(SiteId, channelId).GetAwaiter().GetResult();

                    if (!string.IsNullOrEmpty(_jsMethod))
                    {
                        string scripts = $"window.parent.{_jsMethod}({_itemIndex}, '{nodeNames}', {channelId});";
                        LayerUtils.CloseWithoutRefresh(Page, scripts);
                    }
                    else
                    {
                        var pageUrl = PageUtility.GetChannelUrlAsync(Site, DataProvider.ChannelRepository.GetAsync(channelId).GetAwaiter().GetResult(), false).GetAwaiter().GetResult();
                        if (_isProtocol)
                        {
                            pageUrl = PageUtils.AddProtocolToUrl(pageUrl);
                        }

                        string scripts = $"window.parent.selectChannel('{nodeNames}', '{channelId}', '{pageUrl}');";
                        LayerUtils.CloseWithoutRefresh(Page, scripts);
                    }
                }
                else
                {
                    var nodeInfo = DataProvider.ChannelRepository.GetAsync(SiteId).GetAwaiter().GetResult();

                    var linkUrl = PageUtils.GetCmsUrl(SiteId, nameof(ModalChannelSelect), new NameValueCollection
                    {
                        {"channelId", nodeInfo.Id.ToString()},
                        {"isProtocol", _isProtocol.ToString()},
                        {"jsMethod", _jsMethod},
                        {"itemIndex", _itemIndex.ToString()}
                    });
                    LtlSite.Text = $"<a href='{linkUrl}'>{nodeInfo.ChannelName}</a>";
                    ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(Site, string.Empty, ELoadingType.ChannelClickSelect, null));
                    BindGrid();
                }
			}
		}

        public void BindGrid()
        {
            var channelIdList = DataProvider.ChannelRepository.GetChannelIdsAsync(SiteId, SiteId, EScopeType.Children).GetAwaiter().GetResult();
            RptChannel.DataSource = channelIdList;
            RptChannel.ItemDataBound += rptChannel_ItemDataBound;
            RptChannel.DataBind();
        }

        void rptChannel_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var channelId = (int)e.Item.DataItem;
            var enabled = IsOwningChannelId(channelId);
            if (!enabled)
            {
                if (!IsDescendantOwningChannelId(channelId)) e.Item.Visible = false;
            }
            var nodeInfo = DataProvider.ChannelRepository.GetAsync(channelId).GetAwaiter().GetResult();

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = ChannelLoading.GetChannelRowHtmlAsync(Site, nodeInfo, enabled, ELoadingType.ChannelClickSelect, _additional, AuthRequest.AdminPermissionsImpl).GetAwaiter().GetResult();
        }
	}
}
