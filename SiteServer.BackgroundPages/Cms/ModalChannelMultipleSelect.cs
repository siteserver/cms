using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalChannelMultipleSelect : BasePageCms
    {
        public PlaceHolder PhSiteId;
        public DropDownList DdlSiteId;
        public Literal LtlChannelName;
        public Repeater RptChannel;

        private int _targetSiteId;
        private bool _isSiteSelect;
        private string _jsMethod;

        public static string GetOpenWindowString(int siteId, bool isSiteSelect,
            string jsMethod)
        {
            return LayerUtils.GetOpenScript("选择目标栏目",
                PageUtils.GetCmsUrl(siteId, nameof(ModalChannelMultipleSelect), new NameValueCollection
                {
                    {"isSiteSelect", isSiteSelect.ToString()},
                    {"jsMethod", jsMethod}
                }), 650, 580);
        }

        public static string GetOpenWindowString(int siteId, bool isSiteSelect)
        {
            return GetOpenWindowString(siteId, isSiteSelect, "translateNodeAdd");
        }

        public string GetRedirectUrl(int targetSiteId, string targetChannelId)
        {
            return PageUtils.GetCmsUrl(targetSiteId, nameof(ModalChannelMultipleSelect), new NameValueCollection
            {
                {"isSiteSelect", _isSiteSelect.ToString()},
                {"jsMethod", _jsMethod},
                {"targetSiteId", targetSiteId.ToString()},
                {"targetChannelId", targetChannelId}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _isSiteSelect = AuthRequest.GetQueryBool("isSiteSelect");
            _jsMethod = AuthRequest.GetQueryString("jsMethod");

            _targetSiteId = AuthRequest.GetQueryInt("TargetSiteId");
            if (_targetSiteId == 0)
            {
                _targetSiteId = SiteId;
            }

            if (IsPostBack) return;

            PhSiteId.Visible = _isSiteSelect;

            var siteIdList = AuthRequest.AdminPermissionsImpl.GetSiteIdList();

            var mySystemInfoArrayList = new ArrayList();
            var parentWithChildren = new Hashtable();
            foreach (var siteId in siteIdList)
            {
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo.ParentId == 0)
                {
                    mySystemInfoArrayList.Add(siteInfo);
                }
                else
                {
                    var children = new ArrayList();
                    if (parentWithChildren.Contains(siteInfo.ParentId))
                    {
                        children = (ArrayList)parentWithChildren[siteInfo.ParentId];
                    }
                    children.Add(siteInfo);
                    parentWithChildren[siteInfo.ParentId] = children;
                }
            }
            foreach (SiteInfo siteInfo in mySystemInfoArrayList)
            {
                AddSite(DdlSiteId, siteInfo, parentWithChildren, 0);
            }
            ControlUtils.SelectSingleItem(DdlSiteId, _targetSiteId.ToString());

            var targetChannelId = AuthRequest.GetQueryInt("TargetChannelId");
            if (targetChannelId > 0)
            {
                var siteName = SiteManager.GetSiteInfo(_targetSiteId).SiteName;
                var nodeNames = ChannelManager.GetChannelNameNavigation(_targetSiteId, targetChannelId);
                if (_targetSiteId != SiteId)
                {
                    nodeNames = siteName + "：" + nodeNames;
                }
                string value = $"{_targetSiteId}_{targetChannelId}";
                if (!_isSiteSelect)
                {
                    value = targetChannelId.ToString();
                }
                string scripts = $"window.parent.{_jsMethod}('{nodeNames}', '{value}');";
                LayerUtils.CloseWithoutRefresh(Page, scripts);
            }
            else
            {
                var nodeInfo = ChannelManager.GetChannelInfo(_targetSiteId, _targetSiteId);
                var linkUrl = GetRedirectUrl(_targetSiteId, _targetSiteId.ToString());
                LtlChannelName.Text = $"<a href='{linkUrl}'>{nodeInfo.ChannelName}</a>";

                var additional = new NameValueCollection
                {
                    ["linkUrl"] = GetRedirectUrl(_targetSiteId, string.Empty)
                };
                ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(SiteManager.GetSiteInfo(_targetSiteId), string.Empty, ELoadingType.ChannelClickSelect, additional));

                var channelIdList = ChannelManager.GetChannelIdList(nodeInfo, EScopeType.Children, string.Empty, string.Empty, string.Empty);

                RptChannel.DataSource = channelIdList;
                RptChannel.ItemDataBound += RptChannel_ItemDataBound;
                RptChannel.DataBind();
            }
        }

        private void RptChannel_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var channelId = (int)e.Item.DataItem;
            var enabled = IsOwningChannelId(channelId);
            if (!enabled)
            {
                if (!IsDescendantOwningChannelId(channelId)) e.Item.Visible = false;
            }
            var nodeInfo = ChannelManager.GetChannelInfo(_targetSiteId, channelId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            var additional = new NameValueCollection
            {
                ["linkUrl"] = GetRedirectUrl(_targetSiteId, string.Empty)
            };

            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(SiteInfo, nodeInfo, enabled, ELoadingType.ChannelClickSelect, additional, AuthRequest.AdminPermissionsImpl);
        }

        public void DdlSiteId_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var redirectUrl = GetRedirectUrl(TranslateUtils.ToInt(DdlSiteId.SelectedValue), string.Empty);
            PageUtils.Redirect(redirectUrl);
        }

        private void AddSite(ListControl listControl, SiteInfo siteInfo, Hashtable parentWithChildren, int level)
        {
            var padding = string.Empty;
            for (var i = 0; i < level; i++)
            {
                padding += "　";
            }
            if (level > 0)
            {
                padding += "└ ";
            }

            if (parentWithChildren[siteInfo.Id] != null)
            {
                var children = (ArrayList)parentWithChildren[siteInfo.Id];

                var listitem = new ListItem(padding + siteInfo.SiteName +
                                                 $"({children.Count})", siteInfo.Id.ToString());
                if (siteInfo.Id == SiteId) listitem.Selected = true;

                listControl.Items.Add(listitem);
                level++;
                foreach (SiteInfo subSiteInfo in children)
                {
                    AddSite(listControl, subSiteInfo, parentWithChildren, level);
                }
            }
            else
            {
                var listitem = new ListItem(padding + siteInfo.SiteName, siteInfo.Id.ToString());
                if (siteInfo.Id == SiteId) listitem.Selected = true;

                listControl.Items.Add(listitem);
            }
        }
	}
}
