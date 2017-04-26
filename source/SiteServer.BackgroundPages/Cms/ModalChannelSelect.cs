using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalChannelSelect : BasePageCms
    {
        public Literal ltlPublishmentSystem;
        public Repeater rptChannel;

        private readonly NameValueCollection _additional = new NameValueCollection();
        private bool _isProtocol;
        private string _jsMethod;
        private int _itemIndex;

        public static string GetOpenWindowString(int publishmentSystemId)
        {
            return GetOpenWindowString(publishmentSystemId, false);
        }

        public static string GetOpenWindowString(int publishmentSystemId, bool isProtocol)
        {
            return PageUtils.GetOpenWindowString("栏目选择",
                PageUtils.GetCmsUrl(nameof(ModalChannelSelect), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"isProtocol", isProtocol.ToString()}
                }), 460, 450, true);
        }

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetCmsUrl(nameof(ModalChannelSelect), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()}
            });
        }

        public static string GetOpenWindowStringByItemIndex(int publishmentSystemId, string jsMethod, string itemIndex)
        {
            return PageUtils.GetOpenWindowString("栏目选择",
                PageUtils.GetCmsUrl(nameof(ModalChannelSelect), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"jsMethod", jsMethod},
                    {"itemIndex", itemIndex}
                }), 460, 450, true);
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
                    var nodeNames = NodeManager.GetNodeNameNavigation(PublishmentSystemId, nodeId);

                    if (!string.IsNullOrEmpty(_jsMethod))
                    {
                        string scripts = $"window.parent.{_jsMethod}({_itemIndex}, '{nodeNames}', {nodeId});";
                        PageUtils.CloseModalPageWithoutRefresh(Page, scripts);
                    }
                    else
                    {
                        var pageUrl = PageUtility.GetChannelUrl(PublishmentSystemInfo, NodeManager.GetNodeInfo(PublishmentSystemId, nodeId));
                        if (_isProtocol)
                        {
                            pageUrl = PageUtils.AddProtocolToUrl(pageUrl);
                        }

                        string scripts = $"window.parent.selectChannel('{nodeNames}', '{nodeId}', '{pageUrl}');";
                        PageUtils.CloseModalPageWithoutRefresh(Page, scripts);
                    }
                }
                else
                {
                    var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, PublishmentSystemId);

                    var linkUrl = PageUtils.GetCmsUrl(nameof(ModalChannelSelect), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"NodeID", nodeInfo.NodeId.ToString()},
                        {"isProtocol", _isProtocol.ToString()},
                        {"jsMethod", _jsMethod},
                        {"itemIndex", _itemIndex.ToString()}
                    });
                    ltlPublishmentSystem.Text = $"<a href='{linkUrl}'>{nodeInfo.NodeName}</a>";
                    ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(PublishmentSystemInfo, ELoadingType.ChannelSelect, null));
                    BindGrid();
                }
			}
		}

        public void BindGrid()
        {
            try
            {
                rptChannel.DataSource = DataProvider.NodeDao.GetNodeIdListByParentId(PublishmentSystemId, PublishmentSystemId);
                rptChannel.ItemDataBound += rptChannel_ItemDataBound;
                rptChannel.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        void rptChannel_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var nodeId = (int)e.Item.DataItem;
            var enabled = IsOwningNodeId(nodeId);
            if (!enabled)
            {
                if (!IsHasChildOwningNodeId(nodeId)) e.Item.Visible = false;
            }
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(PublishmentSystemInfo, nodeInfo, enabled, ELoadingType.ChannelSelect, _additional, Body.AdministratorName);
        }
	}
}
