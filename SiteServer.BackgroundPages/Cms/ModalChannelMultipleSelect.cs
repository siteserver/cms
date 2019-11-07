using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Db;
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
                var site = SiteManager.GetSiteAsync(siteId).GetAwaiter().GetResult();
                if (site.ParentId == 0)
                {
                    mySystemInfoArrayList.Add(site);
                }
                else
                {
                    var children = new ArrayList();
                    if (parentWithChildren.Contains(site.ParentId))
                    {
                        children = (ArrayList)parentWithChildren[site.ParentId];
                    }
                    children.Add(site);
                    parentWithChildren[site.ParentId] = children;
                }
            }
            foreach (Site site in mySystemInfoArrayList)
            {
                AddSite(DdlSiteId, site, parentWithChildren, 0);
            }
            ControlUtils.SelectSingleItem(DdlSiteId, _targetSiteId.ToString());

            var targetChannelId = AuthRequest.GetQueryInt("TargetChannelId");
            if (targetChannelId > 0)
            {
                var siteName = SiteManager.GetSiteAsync(_targetSiteId).GetAwaiter().GetResult().SiteName;
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
                ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(SiteManager.GetSiteAsync(_targetSiteId).GetAwaiter().GetResult(), string.Empty, ELoadingType.ChannelClickSelect, additional));

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

            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(Site, nodeInfo, enabled, ELoadingType.ChannelClickSelect, additional, AuthRequest.AdminPermissionsImpl);
        }

        public void DdlSiteId_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var redirectUrl = GetRedirectUrl(TranslateUtils.ToInt(DdlSiteId.SelectedValue), string.Empty);
            PageUtils.Redirect(redirectUrl);
        }

        private void AddSite(ListControl listControl, Site site, Hashtable parentWithChildren, int level)
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

            if (parentWithChildren[site.Id] != null)
            {
                var children = (ArrayList)parentWithChildren[site.Id];

                var listitem = new ListItem(padding + site.SiteName +
                                                 $"({children.Count})", site.Id.ToString());
                if (site.Id == SiteId) listitem.Selected = true;

                listControl.Items.Add(listitem);
                level++;
                foreach (Site subSite in children)
                {
                    AddSite(listControl, subSite, parentWithChildren, level);
                }
            }
            else
            {
                var listitem = new ListItem(padding + site.SiteName, site.Id.ToString());
                if (site.Id == SiteId) listitem.Selected = true;

                listControl.Items.Add(listitem);
            }
        }
	}
}
