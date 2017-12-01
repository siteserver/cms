using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageContentTags : BasePageCms
    {
		public DataGrid DgContents;
        public LinkButton LbPageFirst;
        public LinkButton LbPageLast;
        public LinkButton LbPageNext;
        public LinkButton LbPagePrevious;
        public Literal LtlCurrentPage;
		public Button AddTag;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("Delete"))
            {
                var tagName = Body.GetQueryString("TagName");

                try
                {
                    var contentIdList = BaiRongDataProvider.TagDao.GetContentIdListByTag(tagName, PublishmentSystemId);
                    if (contentIdList.Count > 0)
                    {
                        foreach (var contentId in contentIdList)
                        {
                            var contentTags = BaiRongDataProvider.ContentDao.GetValue(PublishmentSystemInfo.AuxiliaryTableForContent, contentId, ContentAttribute.Tags);
                            var contentTagArrayList = TranslateUtils.StringCollectionToStringList(contentTags);
                            contentTagArrayList.Remove(tagName);
                            BaiRongDataProvider.ContentDao.SetValue(PublishmentSystemInfo.AuxiliaryTableForContent, contentId, ContentAttribute.Tags, TranslateUtils.ObjectCollectionToString(contentTagArrayList));
                        }
                    }
                    BaiRongDataProvider.TagDao.DeleteTag(tagName, PublishmentSystemId);
                    Body.AddSiteLog(PublishmentSystemId, "删除内容标签", $"内容标签:{tagName}");
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            if (IsPostBack) return;

            BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, "内容标签管理", AppManager.Permissions.WebSite.Configration);

            BindGrid();

            var showPopWinString = ModalContentTagAdd.GetOpenWindowStringToAdd(PublishmentSystemId);
            AddTag.Attributes.Add("onclick", showPopWinString);
        }

        private void DgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
            var tagInfo = (TagInfo)e.Item.DataItem;

            var ltlTagName = (Literal)e.Item.FindControl("ltlTagName");
            var ltlCount = (Literal)e.Item.FindControl("ltlCount");
            var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");
            var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

            var cssClass = "tag_popularity_1";
            if (tagInfo.Level == 2)
            {
                cssClass = "tag_popularity_2";
            }
            else if (tagInfo.Level == 3)
            {
                cssClass = "tag_popularity_3";
            }

            ltlTagName.Text = $@"<span class=""{cssClass}"">{tagInfo.Tag}</span>";
            ltlCount.Text = tagInfo.UseNum.ToString();

            var showPopWinString = ModalContentTagAdd.GetOpenWindowStringToEdit(PublishmentSystemId, tagInfo.Tag);
            ltlEditUrl.Text = $"<a href=\"javascript:;\" onClick=\"{showPopWinString}\">编辑</a>";

            var urlDelete = PageUtils.GetCmsUrl(nameof(PageContentTags), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"TagName", tagInfo.Tag},
                {"Delete", true.ToString()}
            });
            ltlDeleteUrl.Text =
                $"<a href=\"{urlDelete}\" onClick=\"javascript:return confirm('此操作将删除内容标签“{tagInfo.Tag}”，确认吗？');\">删除</a>";
        }

        public void MyDataGrid_Page(object sender, DataGridPageChangedEventArgs e)
        {
            DgContents.CurrentPageIndex = e.NewPageIndex;
            BindGrid();
        }

        private void BindGrid()
        {
            try
            {
                DgContents.PageSize = StringUtils.Constants.PageSize;
                DgContents.DataSource = BaiRongDataProvider.TagDao.GetTagInfoList(PublishmentSystemId, 0, true, 0);
                DgContents.ItemDataBound += DgContents_ItemDataBound;
                DgContents.DataBind();

                if (DgContents.CurrentPageIndex > 0)
                {
                    LbPageFirst.Enabled = true;
                    LbPagePrevious.Enabled = true;
                }
                else
                {
                    LbPageFirst.Enabled = false;
                    LbPagePrevious.Enabled = false;
                }

                if (DgContents.CurrentPageIndex + 1 == DgContents.PageCount)
                {
                    LbPageLast.Enabled = false;
                    LbPageNext.Enabled = false;
                }
                else
                {
                    LbPageLast.Enabled = true;
                    LbPageNext.Enabled = true;
                }

                LtlCurrentPage.Text = $"{DgContents.CurrentPageIndex + 1}/{DgContents.PageCount}";
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }

        }

        protected void NavigationButtonClick(object sender, EventArgs e)
        {
            var button = (LinkButton)sender;
            var direction = button.CommandName;

            switch (direction.ToUpper())
            {
                case "FIRST":
                    DgContents.CurrentPageIndex = 0;
                    break;
                case "PREVIOUS":
                    DgContents.CurrentPageIndex =
                        Math.Max(DgContents.CurrentPageIndex - 1, 0);
                    break;
                case "NEXT":
                    DgContents.CurrentPageIndex =
                        Math.Min(DgContents.CurrentPageIndex + 1,
                        DgContents.PageCount - 1);
                    break;
                case "LAST":
                    DgContents.CurrentPageIndex = DgContents.PageCount - 1;
                    break;
            }
            BindGrid();
        }
	}
}
