using System;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using Content = SiteServer.CMS.Model.Content;
using WebUtils = SiteServer.BackgroundPages.Core.WebUtils;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContentsTag : BasePageCms
    {
        public Literal LtlContentTag;

        public Repeater RptContents;
        public SqlPager SpContents;

        private string _tag;

        public static string GetRedirectUrl(int siteId, string tag)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageContentsTag), new NameValueCollection
            {
                {"tag", tag}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var siteId = AuthRequest.GetQueryInt("siteId");
            _tag = AuthRequest.GetQueryString("tag");

            if (AuthRequest.IsQueryExists("remove"))
            {
                var channelId = AuthRequest.GetQueryInt("channelId");
                var contentId = AuthRequest.GetQueryInt("contentId");
                var channelInfo = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();

                var contentInfo = DataProvider.ContentDao.GetAsync(Site, channelInfo, contentId).GetAwaiter().GetResult();
                
                var tagList = TranslateUtils.StringCollectionToStringList(contentInfo.Tags, ' ');
                if (tagList.Contains(_tag))
                {
                    tagList.Remove(_tag);
                }

                contentInfo.Tags = TranslateUtils.ObjectCollectionToString(tagList, " ");
                DataProvider.ContentDao.UpdateAsync(Site, channelInfo, contentInfo).GetAwaiter().GetResult();

                ContentTagUtils.RemoveTagsAsync(SiteId, contentId).GetAwaiter().GetResult();

                AuthRequest.AddSiteLogAsync(SiteId, "移除内容", $"内容:{contentInfo.Title}").GetAwaiter().GetResult();
                SuccessMessage("移除成功");
                AddWaitAndRedirectScript(PageUrl);
            }

            SpContents.ControlToPaginate = RptContents;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            SpContents.ItemsPerPage = Site.PageSize;
            SpContents.SelectCommand = DataProvider.ContentDao.GetSqlStringByContentTag(Site.TableName, _tag, siteId);
            SpContents.SortField = ContentAttribute.AddDate;
            SpContents.SortMode = SortMode.DESC;

            if (IsPostBack) return;

            VerifySitePermissions(Constants.WebSitePermissions.Configuration);
            LtlContentTag.Text = "标签：" + _tag;
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
                $@"<a href=""{WebUtils.GetContentAddEditUrl(SiteId, contentInfo.ChannelId, contentInfo.Id, PageUrl)}"">编辑</a>";

            var removeUrl = PageUtils.GetCmsUrl(SiteId, nameof(PageContentsTag), new NameValueCollection
            {
                {"tag", _tag},
                {"channelId", contentInfo.ChannelId.ToString()},
                {"contentId", contentInfo.Id.ToString()},
                {"remove", true.ToString()}
            });

            ltlItemDeleteUrl.Text =
                $@"<a href=""javascript:;"" onClick=""{AlertUtils.Warning("从此标签移除", $"此操作将从标签“{_tag}”移除该内容，确认吗？", "取 消", "移 除", $"location.href = '{removeUrl}';return false;")}"">从此标签移除</a>";
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetCmsUrl(SiteId, nameof(PageContentsTag), new NameValueCollection
                    {
                        {"tag", _tag}
                    });
                }
                return _pageUrl;
            }
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageContentTags.GetRedirectUrl(SiteId));
        }
    }
}
