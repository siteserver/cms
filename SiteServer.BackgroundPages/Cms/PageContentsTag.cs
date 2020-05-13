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
                var channelInfo = ChannelManager.GetChannelInfo(SiteId, channelId);

                var contentInfo = ContentManager.GetContentInfo(SiteInfo, channelInfo, contentId);
                
                var tagList = TranslateUtils.StringCollectionToStringList(contentInfo.Tags, ' ');
                if (tagList.Contains(_tag))
                {
                    tagList.Remove(_tag);
                }

                contentInfo.Tags = TranslateUtils.ObjectCollectionToString(tagList, " ");
                DataProvider.ContentDao.Update(SiteInfo, channelInfo, contentInfo);

                TagUtils.RemoveTags(SiteId, contentId);

                AuthRequest.AddSiteLog(SiteId, "移除内容", $"内容:{contentInfo.Title}");
                SuccessMessage("移除成功");
                AddWaitAndRedirectScript(PageUrl);
            }

            SpContents.ControlToPaginate = RptContents;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            SpContents.ItemsPerPage = SiteInfo.Additional.PageSize;
            SpContents.SelectCommand = DataProvider.ContentDao.GetSqlStringByContentTag(SiteInfo.TableName, _tag, siteId);
            SpContents.SortField = ContentAttribute.AddDate;
            SpContents.SortMode = SortMode.DESC;

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.SitePermissions.ConfigGroups);
            LtlContentTag.Text = "标签：" + _tag;
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
