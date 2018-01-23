using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

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

        public static string GetRedirectUrl(int siteId, int nodeId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalChannelSelect), new NameValueCollection
            {
                {"NodeID", nodeId.ToString()}
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

            _isProtocol = Body.GetQueryBool("isProtocol");
            _jsMethod = Body.GetQueryString("jsMethod");
            _itemIndex = Body.GetQueryInt("itemIndex");

            _additional.Add("isProtocol", _isProtocol.ToString());
            _additional.Add("jsMethod", _jsMethod);
            _additional.Add("itemIndex", _itemIndex.ToString());

			if (!IsPostBack)
			{
                if (Body.IsQueryExists("NodeID"))
                {
                    var nodeId = Body.GetQueryInt("NodeID");
                    var nodeNames = ChannelManager.GetChannelNameNavigation(SiteId, nodeId);

                    if (!string.IsNullOrEmpty(_jsMethod))
                    {
                        string scripts = $"window.parent.{_jsMethod}({_itemIndex}, '{nodeNames}', {nodeId});";
                        LayerUtils.CloseWithoutRefresh(Page, scripts);
                    }
                    else
                    {
                        var pageUrl = PageUtility.GetChannelUrl(SiteInfo, ChannelManager.GetChannelInfo(SiteId, nodeId), false);
                        if (_isProtocol)
                        {
                            pageUrl = PageUtils.AddProtocolToUrl(pageUrl);
                        }

                        string scripts = $"window.parent.selectChannel('{nodeNames}', '{nodeId}', '{pageUrl}');";
                        LayerUtils.CloseWithoutRefresh(Page, scripts);
                    }
                }
                else
                {
                    var nodeInfo = ChannelManager.GetChannelInfo(SiteId, SiteId);

                    var linkUrl = PageUtils.GetCmsUrl(SiteId, nameof(ModalChannelSelect), new NameValueCollection
                    {
                        {"NodeID", nodeInfo.Id.ToString()},
                        {"isProtocol", _isProtocol.ToString()},
                        {"jsMethod", _jsMethod},
                        {"itemIndex", _itemIndex.ToString()}
                    });
                    LtlSite.Text = $"<a href='{linkUrl}'>{nodeInfo.ChannelName}</a>";
                    ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(SiteInfo, ELoadingType.ChannelSelect, null));
                    BindGrid();
                }
			}
		}

        public void BindGrid()
        {
            RptChannel.DataSource = DataProvider.ChannelDao.GetIdListByParentId(SiteId, SiteId);
            RptChannel.ItemDataBound += rptChannel_ItemDataBound;
            RptChannel.DataBind();
        }

        void rptChannel_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var nodeId = (int)e.Item.DataItem;
            var enabled = IsOwningNodeId(nodeId);
            if (!enabled)
            {
                if (!IsHasChildOwningNodeId(nodeId)) e.Item.Visible = false;
            }
            var nodeInfo = ChannelManager.GetChannelInfo(SiteId, nodeId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(SiteInfo, nodeInfo, enabled, ELoadingType.ChannelSelect, _additional, Body.AdminName);
        }
	}
}
