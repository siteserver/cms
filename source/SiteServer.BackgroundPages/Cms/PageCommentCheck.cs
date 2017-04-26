using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageCommentCheck : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnCheck;
        public Button BtnDelete;

        public readonly Dictionary<int, string> ContentTitles = new Dictionary<int, string>();

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageCommentCheck), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdContent, "评论审核", string.Empty);

                if (Body.GetQueryBool("Check"))
                {
                    var idList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("IDsCollection"));
                    try
                    {
                        DataProvider.CommentDao.Check(PublishmentSystemId, idList);
                        Body.AddSiteLog(PublishmentSystemId, 0, 0, "审核评论", string.Empty);
                        SuccessCheckMessage();
                    }
                    catch (Exception ex)
                    {
                        FailCheckMessage(ex);
                    }
                }
                else if (Body.GetQueryBool("Delete"))
                {
                    var idList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("IDsCollection"));
                    try
                    {
                        DataProvider.CommentDao.DeleteUnChecked(idList);
                        Body.AddSiteLog(PublishmentSystemId, 0, 0, "删除评论", string.Empty);
                        SuccessDeleteMessage();
                    }
                    catch (Exception ex)
                    {
                        FailDeleteMessage(ex);
                    }
                }

                SpContents.ControlToPaginate = RptContents;
                SpContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;

                SpContents.SelectCommand = DataProvider.CommentDao.GetSelectedCommendByCheck(PublishmentSystemId);

                SpContents.SortField = DataProvider.CommentDao.GetSortFieldName();
                SpContents.SortMode = SortMode.DESC;
                RptContents.ItemDataBound += RptContents_ItemDataBound;

                SpContents.DataBind();

                BtnCheck.Attributes.Add("onclick",
                        PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                            PageUtils.GetCmsUrl(nameof(PageCommentCheck), new NameValueCollection
                            {
                                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                                {"Check", true.ToString()}
                            }), "IDsCollection", "IDsCollection", "请选择需要审核的评论！", "此操作将审核所选评论，确认吗？"));

                if (HasChannelPermissions(PublishmentSystemId, AppManager.Cms.Permission.Channel.CommentDelete))
                {
                    BtnDelete.Attributes.Add("onclick",
                        PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                            PageUtils.GetCmsUrl(nameof(PageCommentCheck), new NameValueCollection
                            {
                                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                                {"Delete", true.ToString()}
                            }), "IDsCollection", "IDsCollection", "请选择需要删除的评论！", "此操作将删除所选评论，确认吗？"));
                }
                else
                {
                    BtnDelete.Visible = false;
                }
            }
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var ltlComment = e.Item.FindControl("ltlComment") as Literal;
            var ltlContent = e.Item.FindControl("ltlContent") as Literal;
            var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
            var ltlItemSelect = e.Item.FindControl("ltlItemSelect") as Literal;

            var commentInfo = new CommentInfo(e.Item.DataItem);

            if (ltlComment != null) ltlComment.Text = commentInfo.Content;

            if (ltlContent != null)
            {
                if (ContentTitles.ContainsKey(commentInfo.ContentId))
                {
                    ltlContent.Text = ContentTitles[commentInfo.ContentId];
                }
                else
                {
                    var linkUrl = PageActions.GetRedirectUrl(PublishmentSystemId, commentInfo.NodeId,
                        commentInfo.ContentId);
                    var tableName = NodeManager.GetTableName(PublishmentSystemInfo,
                        NodeManager.GetNodeInfo(PublishmentSystemId, PublishmentSystemId));
                    var linkText = BaiRongDataProvider.ContentDao.GetValue(tableName, commentInfo.ContentId,
                        ContentAttribute.Title);
                    ContentTitles[commentInfo.ContentId] =
                        ltlContent.Text = $@"<a href=""{linkUrl}"" target=""_blank"">{linkText}</a>";
                }
            }

            if (ltlAddDate != null)
            {
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(commentInfo.AddDate, EDateFormatType.Chinese,
                    ETimeFormatType.ShortTime);
            }

            if (ltlItemSelect != null)
            {
                ltlItemSelect.Text =
                    $@"<input type=""checkbox"" name=""IDsCollection"" value=""{commentInfo.Id}"" />";
            }
        }
    }
}
