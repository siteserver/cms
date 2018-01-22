using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Model;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContentsGroup : BasePageCms
    {
        public Literal LtlContentGroupName;

        public Repeater RptContents;
        public SqlPager SpContents;

        private string _tableName;
        private NodeInfo _nodeInfo;
        private string _contentGroupName;

        public static string GetRedirectUrl(int publishmentSystemId, string contentGroupName)
        {
            return PageUtils.GetCmsUrl(nameof(PageContentsGroup), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"contentGroupName", contentGroupName}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var publishmentSystemId = Body.GetQueryInt("publishmentSystemId");
            _contentGroupName = Body.GetQueryString("contentGroupName");
            _nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, publishmentSystemId);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);

            if (Body.IsQueryExists("remove"))
            {
                var contentId = Body.GetQueryInt("contentId");

                var contentInfo = DataProvider.ContentDao.GetContentInfo(_tableName, contentId);
                var groupList = TranslateUtils.StringCollectionToStringList(contentInfo.ContentGroupNameCollection);
                if (groupList.Contains(_contentGroupName))
                {
                    groupList.Remove(_contentGroupName);
                }

                contentInfo.ContentGroupNameCollection = TranslateUtils.ObjectCollectionToString(groupList);
                DataProvider.ContentDao.Update(_tableName, PublishmentSystemInfo, contentInfo);
                Body.AddSiteLog(PublishmentSystemId, "移除内容", $"内容:{contentInfo.Title}");
                SuccessMessage("移除成功");
                AddWaitAndRedirectScript(PageUrl);
            }

            SpContents.ControlToPaginate = RptContents;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            SpContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            SpContents.SelectCommand = DataProvider.ContentDao.GetSelectCommendByContentGroup(_tableName, _contentGroupName, publishmentSystemId);
            SpContents.SortField = "AddDate";
            SpContents.SortMode = SortMode.DESC;

            if (IsPostBack) return;

            VerifySitePermissions(AppManager.Permissions.WebSite.Configration);
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

            var contentInfo = new ContentInfo(e.Item.DataItem);

            ltlItemTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, PageUrl);
            ltlItemChannel.Text = NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId);
            ltlItemAddDate.Text = DateUtils.GetDateAndTimeString(contentInfo.AddDate);
            ltlItemStatus.Text = CheckManager.GetCheckState(PublishmentSystemInfo, contentInfo.IsChecked,
                contentInfo.CheckedLevel);

            if (!HasChannelPermissions(contentInfo.NodeId, AppManager.Permissions.Channel.ContentEdit) &&
                Body.AdminName != contentInfo.AddUserName) return;

            ltlItemEditUrl.Text =
                $@"<a href=""{WebUtils.GetContentAddEditUrl(PublishmentSystemId, _nodeInfo, contentInfo.Id, PageUrl)}"">编辑</a>";

            var removeUrl = PageUtils.GetCmsUrl(nameof(PageContentsGroup), new NameValueCollection
            {
                {"publishmentSystemId", PublishmentSystemId.ToString()},
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
                    _pageUrl = PageUtils.GetCmsUrl(nameof(PageContentsGroup), new NameValueCollection
                    {
                        {"publishmentSystemId", PublishmentSystemId.ToString()},
                        {"contentGroupName", _contentGroupName}
                    });
                }
                return _pageUrl;
            }
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageContentGroup.GetRedirectUrl(PublishmentSystemId));
        }
    }
}
