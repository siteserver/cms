using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Permissions;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.User;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContentsGroup : BasePageCms
    {
        public Literal ltlContentGroupName;

        public Repeater rptContents;
        public SqlPager spContents;

        private string tableName;
        private NodeInfo nodeInfo;
        private string contentGroupName;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var publishmentSystemId = Body.GetQueryInt("publishmentSystemID");
            contentGroupName = Body.GetQueryString("contentGroupName");
            nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, publishmentSystemId);
            tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);

            if (Body.IsQueryExists("Remove"))
            {
                var groupName = Body.GetQueryString("ContentGroupName");
                var contentId = Body.GetQueryInt("ContentID");
                try
                {
                    var contentInfo = DataProvider.ContentDao.GetContentInfo(ETableStyle.BackgroundContent, tableName, contentId);
                    var groupList = TranslateUtils.StringCollectionToStringList(contentInfo.ContentGroupNameCollection);
                    if (groupList.Contains(groupName))
                        groupList.Remove(groupName);

                    contentInfo.ContentGroupNameCollection = TranslateUtils.ObjectCollectionToString(groupList);
                    DataProvider.ContentDao.Update(tableName, PublishmentSystemInfo, contentInfo);
                    Body.AddSiteLog(PublishmentSystemId, "移除内容", $"内容:{contentInfo.Title}");
                    SuccessMessage("移除成功");
                    AddWaitAndRedirectScript(PageUrl);
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "移除失败");
                }
            }

            spContents.ControlToPaginate = rptContents;
            rptContents.ItemDataBound += rptContents_ItemDataBound;
            spContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            spContents.SelectCommand = DataProvider.ContentDao.GetSelectCommendByContentGroup(tableName, contentGroupName, publishmentSystemId);
            spContents.SortField = "AddDate";
            spContents.SortMode = SortMode.DESC;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, AppManager.Cms.LeftMenu.Configuration.IdConfigurationGroupAndTags, "查看内容组", AppManager.Cms.Permission.WebSite.Configration);
                ltlContentGroupName.Text = "内容组：" + Body.GetQueryString("ContentGroupName");
                spContents.DataBind();
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlItemTitle = e.Item.FindControl("ltlItemTitle") as Literal;
                var ltlItemChannel = e.Item.FindControl("ltlItemChannel") as Literal;
                var ltlItemAuthor = e.Item.FindControl("ltlItemAuthor") as Literal;
                var ltlItemAddDate = e.Item.FindControl("ltlItemAddDate") as Literal;
                var ltlItemStatus = e.Item.FindControl("ltlItemStatus") as Literal;
                var ltlItemEditUrl = e.Item.FindControl("ltlItemEditUrl") as Literal;
                var ltlItemDeleteUrl = e.Item.FindControl("ltlItemDeleteUrl") as Literal;

                var contentInfo = new ContentInfo(e.Item.DataItem);

                ltlItemTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, PageUrl);
                ltlItemChannel.Text = NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId);
                ltlItemAuthor.Text = AdminManager.GetDisplayName(contentInfo.AddUserName, true);
                ltlItemAddDate.Text = DateUtils.GetDateAndTimeString(contentInfo.AddDate);
                ltlItemStatus.Text = LevelManager.GetCheckState(PublishmentSystemInfo, contentInfo.IsChecked, contentInfo.CheckedLevel);

                if (HasChannelPermissions(contentInfo.NodeId, AppManager.Cms.Permission.Channel.ContentEdit) || Body.AdministratorName == contentInfo.AddUserName)
                {
                    //编辑
                    ltlItemEditUrl.Text =
                        $"<a href=\"{WebUtils.GetContentAddEditUrl(PublishmentSystemId, nodeInfo, contentInfo.Id, PageUrl)}\">编辑</a>";

                    //移除
                    ltlItemDeleteUrl.Text = GetRemoveHtml(contentGroupName, contentInfo.Id);
                }
            }
        }

        public string GetRemoveHtml(string groupName, int contentId)
        {
            var urlGroup = PageUtils.GetCmsUrl(nameof(PageContentsGroup), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"contentGroupName", groupName},
                {"Remove", true.ToString()}
            });
            return
                $"<a href=\"{urlGroup}\" onClick=\"javascript:return confirm('此操作将从内容组“{groupName}”移除该内容，确认吗？');\">移除</a>";
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
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"contentGroupName", Body.GetQueryString("contentGroupName")}
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
