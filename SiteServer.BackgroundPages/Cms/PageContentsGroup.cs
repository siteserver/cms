using System;
using System.Collections.Specialized;
using System.Data;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContentsGroup : BasePageCms
    {
        public Literal LtlContentGroupName;

        public Repeater RptContents;
        public SqlPager SpContents;

        private string _tableName;
        private ChannelInfo _channelInfo;
        private string _contentGroupName;

        public static string GetRedirectUrl(int siteId, string contentGroupName)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageContentsGroup), new NameValueCollection
            {
                {"contentGroupName", contentGroupName}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var siteId = AuthRequest.GetQueryInt("siteId");
            _contentGroupName = AuthRequest.GetQueryString("contentGroupName");
            _channelInfo = ChannelManager.GetChannelInfo(siteId, siteId);
            _tableName = ChannelManager.GetTableName(SiteInfo, _channelInfo);

            if (AuthRequest.IsQueryExists("remove"))
            {
                var contentId = AuthRequest.GetQueryInt("contentId");

                var contentInfo = ContentManager.GetContentInfo(SiteInfo, _channelInfo, contentId);
                var groupList = TranslateUtils.StringCollectionToStringList(contentInfo.GroupNameCollection);
                if (groupList.Contains(_contentGroupName))
                {
                    groupList.Remove(_contentGroupName);
                }

                contentInfo.GroupNameCollection = TranslateUtils.ObjectCollectionToString(groupList);
                DataProvider.ContentDao.Update(SiteInfo, _channelInfo, contentInfo);
                AuthRequest.AddSiteLog(SiteId, "移除内容", $"内容:{contentInfo.Title}");
                SuccessMessage("移除成功");
                AddWaitAndRedirectScript(PageUrl);
            }

            SpContents.ControlToPaginate = RptContents;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            SpContents.ItemsPerPage = SiteInfo.Additional.PageSize;
            SpContents.SelectCommand = DataProvider.ContentDao.GetSqlStringByContentGroup(_tableName, _contentGroupName, siteId);
            SpContents.SortField = ContentAttribute.AddDate;
            SpContents.SortMode = SortMode.DESC;

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Configration);
            LtlContentGroupName.Text = "内容组：" + _contentGroupName;
            SpContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var ltlItemTitle = (Literal) e.Item.FindControl("ltlItemTitle");
            var ltlItemChannel = (Literal) e.Item.FindControl("ltlItemChannel");
            var ltlItemAddDate = (Literal) e.Item.FindControl("ltlItemAddDate");
            var ltlItemStatus = (Literal) e.Item.FindControl("ltlItemStatus");
            var ltlItemEditUrl = (Literal) e.Item.FindControl("ltlItemEditUrl");
            var ltlItemDeleteUrl = (Literal) e.Item.FindControl("ltlItemDeleteUrl");

            var contentInfo = new ContentInfo((DataRowView)e.Item.DataItem);

            ltlItemTitle.Text = WebUtils.GetContentTitle(SiteInfo, contentInfo, PageUrl);
            ltlItemChannel.Text = ChannelManager.GetChannelNameNavigation(SiteId, contentInfo.ChannelId);
            ltlItemAddDate.Text = DateUtils.GetDateAndTimeString(contentInfo.AddDate);
            ltlItemStatus.Text = CheckManager.GetCheckState(SiteInfo, contentInfo);

            if (!HasChannelPermissions(contentInfo.ChannelId, ConfigManager.ChannelPermissions.ContentEdit) &&
                AuthRequest.AdminName != contentInfo.AddUserName) return;

            ltlItemEditUrl.Text =
                $@"<a href=""{WebUtils.GetContentAddEditUrl(SiteId, _channelInfo, contentInfo.Id, PageUrl)}"">编辑</a>";

            var removeUrl = PageUtils.GetCmsUrl(SiteId, nameof(PageContentsGroup), new NameValueCollection
            {
                {"contentGroupName", _contentGroupName},
                {"contentId", contentInfo.Id.ToString()},
                {"remove", true.ToString()}
            });

            ltlItemDeleteUrl.Text =
                $@"<a href=""javascript:;"" onClick=""{AlertUtils.Warning("从此内容组移除", $"此操作将从内容组“{_contentGroupName}”移除该内容，确认吗？", "取 消", "移 除", $"location.href = '{removeUrl}';return false;")}"">从此内容组移除</a>";
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetCmsUrl(SiteId, nameof(PageContentsGroup), new NameValueCollection
                    {
                        {"contentGroupName", _contentGroupName}
                    });
                }
                return _pageUrl;
            }
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageContentGroup.GetRedirectUrl(SiteId));
        }
    }
}
