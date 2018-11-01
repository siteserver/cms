using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageContentTags : BasePageCms
    {
		public Repeater RptContents;
        public SqlPager SpContents;

		public Button BtnAddTag;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (AuthRequest.IsQueryExists("Delete"))
            {
                var tagName = AuthRequest.GetQueryString("TagName");

                try
                {
                    var contentIdList = DataProvider.TagDao.GetContentIdListByTag(tagName, SiteId);
                    if (contentIdList.Count > 0)
                    {
                        foreach (var contentId in contentIdList)
                        {
                            var tuple = DataProvider.ContentDao.GetValue(SiteInfo.TableName, contentId, ContentAttribute.Tags);
                            if (tuple != null)
                            {
                                var contentTagList = TranslateUtils.StringCollectionToStringList(tuple.Item2);
                                contentTagList.Remove(tagName);
                                DataProvider.ContentDao.Update(SiteInfo.TableName, tuple.Item1, contentId, ContentAttribute.Tags, TranslateUtils.ObjectCollectionToString(contentTagList));
                            }
                        }
                    }
                    DataProvider.TagDao.DeleteTag(tagName, SiteId);
                    AuthRequest.AddSiteLog(SiteId, "删除内容标签", $"内容标签:{tagName}");
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = SiteInfo.Additional.PageSize;

            SpContents.SelectCommand = DataProvider.TagDao.GetSqlString(SiteId, 0, true, 0);
            SpContents.SortField = nameof(TagInfo.UseNum);
            SpContents.SortMode = SortMode.DESC;

            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Configration);

            SpContents.DataBind();

            var showPopWinString = ModalContentTagAdd.GetOpenWindowStringToAdd(SiteId);
            BtnAddTag.Attributes.Add("onclick", showPopWinString);
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var tag = SqlUtils.EvalString(e.Item.DataItem, nameof(TagInfo.Tag));
            var level = SqlUtils.EvalInt(e.Item.DataItem, nameof(TagInfo.Level));
            var useNum = SqlUtils.EvalInt(e.Item.DataItem, nameof(TagInfo.UseNum));

            var ltlTagName = (Literal)e.Item.FindControl("ltlTagName");
            var ltlCount = (Literal)e.Item.FindControl("ltlCount");
            var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");
            var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

            var cssClass = "tag_popularity_1";
            if (level == 2)
            {
                cssClass = "tag_popularity_2";
            }
            else if (level == 3)
            {
                cssClass = "tag_popularity_3";
            }

            ltlTagName.Text = $@"<span class=""{cssClass}"">{tag}</span>";
            ltlCount.Text = useNum.ToString();

            var showPopWinString = ModalContentTagAdd.GetOpenWindowStringToEdit(SiteId, tag);
            ltlEditUrl.Text = $"<a href=\"javascript:;\" onClick=\"{showPopWinString}\">编辑</a>";

            var urlDelete = PageUtils.GetCmsUrl(SiteId, nameof(PageContentTags), new NameValueCollection
            {
                {"TagName", tag},
                {"Delete", true.ToString()}
            });
            ltlDeleteUrl.Text =
                $"<a href=\"{urlDelete}\" onClick=\"javascript:return confirm('此操作将删除内容标签“{tag}”，确认吗？');\">删除</a>";
        }
	}
}
