using System;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;
using Content = SiteServer.Abstractions.Content;
using WebUtils = SiteServer.BackgroundPages.Core.WebUtils;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContentsGroup : BasePageCms
    {
        public Literal LtlContentGroupName;

        public Repeater RptContents;
        public SqlPager SpContents;

        private string _tableName;
        private Channel _channel;
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
            _channel = ChannelManager.GetChannelAsync(siteId, siteId).GetAwaiter().GetResult();
            _tableName = ChannelManager.GetTableNameAsync(Site, _channel).GetAwaiter().GetResult();

            if (AuthRequest.IsQueryExists("remove"))
            {
                var contentId = AuthRequest.GetQueryInt("contentId");

                var contentInfo = DataProvider.ContentRepository.GetAsync(Site, _channel, contentId).GetAwaiter().GetResult();
                var groupList = contentInfo.GroupNames;
                if (groupList.Contains(_contentGroupName))
                {
                    groupList.Remove(_contentGroupName);
                }

                contentInfo.GroupNames = groupList;
                DataProvider.ContentRepository.UpdateAsync(Site, _channel, contentInfo).GetAwaiter().GetResult();
                AuthRequest.AddSiteLogAsync(SiteId, "移除内容", $"内容:{contentInfo.Title}").GetAwaiter().GetResult();
                SuccessMessage("移除成功");
                AddWaitAndRedirectScript(PageUrl);
            }

            SpContents.ControlToPaginate = RptContents;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            SpContents.ItemsPerPage = Site.PageSize;
            SpContents.SelectCommand = DataProvider.ContentRepository.GetSqlStringByContentGroup(_tableName, _contentGroupName, siteId);
            SpContents.SortField = ContentAttribute.AddDate;
            SpContents.SortMode = SortMode.DESC;

            if (IsPostBack) return;

            VerifySitePermissions(Constants.SitePermissions.ConfigGroups);
            LtlContentGroupName.Text = "内容组：" + _contentGroupName;
            SpContents.DataBind();
        }

        public Content GetContent(DataRow row)
        {
            if (row == null) return null;

            var content = new Content();

            var dict = row.Table.Columns
                .Cast<DataColumn>()
                .ToDictionary(c => c.ColumnName, c => row[c]);

            foreach (var key in dict.Keys)
            {
                content.Set(key, dict[key]);
            }

            return content;
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

            var rowView = (DataRowView)e.Item.DataItem;
            var contentInfo = GetContent(rowView.Row);

            ltlItemTitle.Text = WebUtils.GetContentTitle(Site, contentInfo, PageUrl);
            ltlItemChannel.Text = ChannelManager.GetChannelNameNavigationAsync(SiteId, contentInfo.ChannelId).GetAwaiter().GetResult();
            ltlItemAddDate.Text = DateUtils.GetDateAndTimeString(contentInfo.AddDate);
            ltlItemStatus.Text = CheckManager.GetCheckState(Site, contentInfo);

            if (!HasChannelPermissions(contentInfo.ChannelId, Constants.ChannelPermissions.ContentEdit) &&
                AuthRequest.AdminName != contentInfo.AddUserName) return;

            ltlItemEditUrl.Text =
                $@"<a href=""{WebUtils.GetContentAddEditUrl(SiteId, _channel.Id, contentInfo.Id, PageUrl)}"">编辑</a>";

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
