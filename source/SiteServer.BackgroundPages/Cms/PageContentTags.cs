using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageContentTags : BasePageCms
    {
		public DataGrid dgContents;
        public LinkButton pageFirst;
        public LinkButton pageLast;
        public LinkButton pageNext;
        public LinkButton pagePrevious;
        public Label currentPage;
		public Button AddTag;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("Delete"))
            {
                var tagName = Body.GetQueryString("TagName");

                try
                {
                    var contentIDArrayList = BaiRongDataProvider.TagDao.GetContentIdListByTag(tagName, PublishmentSystemId);
                    if (contentIDArrayList.Count > 0)
                    {
                        foreach (int contentID in contentIDArrayList)
                        {
                            var contentTags = BaiRongDataProvider.ContentDao.GetValue(PublishmentSystemInfo.AuxiliaryTableForContent, contentID, ContentAttribute.Tags);
                            var contentTagArrayList = TranslateUtils.StringCollectionToStringList(contentTags);
                            contentTagArrayList.Remove(tagName);
                            BaiRongDataProvider.ContentDao.SetValue(PublishmentSystemInfo.AuxiliaryTableForContent, contentID, ContentAttribute.Tags, TranslateUtils.ObjectCollectionToString(contentTagArrayList));
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

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, AppManager.Cms.LeftMenu.Configuration.IdConfigurationGroupAndTags, "内容标签管理", AppManager.Cms.Permission.WebSite.Configration);

                BindGrid();

                var showPopWinString = ModalContentTagAdd.GetOpenWindowStringToAdd(PublishmentSystemId);
                AddTag.Attributes.Add("onclick", showPopWinString);
			}
		}

        private void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var tagInfo = e.Item.DataItem as TagInfo;

                var ltlTagName = e.Item.FindControl("ltlTagName") as Literal;
                var ltlCount = e.Item.FindControl("ltlCount") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;
                var ltlDeleteUrl = e.Item.FindControl("ltlDeleteUrl") as Literal;

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
        }

        public void MyDataGrid_Page(object sender, DataGridPageChangedEventArgs e)
        {
            dgContents.CurrentPageIndex = e.NewPageIndex;
            BindGrid();
        }

        private void BindGrid()
        {
            try
            {
                dgContents.PageSize = StringUtils.Constants.PageSize;
                dgContents.DataSource = BaiRongDataProvider.TagDao.GetTagInfoList(PublishmentSystemId, 0, true, 0);
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();

                if (dgContents.CurrentPageIndex > 0)
                {
                    pageFirst.Enabled = true;
                    pagePrevious.Enabled = true;
                }
                else
                {
                    pageFirst.Enabled = false;
                    pagePrevious.Enabled = false;
                }

                if (dgContents.CurrentPageIndex + 1 == dgContents.PageCount)
                {
                    pageLast.Enabled = false;
                    pageNext.Enabled = false;
                }
                else
                {
                    pageLast.Enabled = true;
                    pageNext.Enabled = true;
                }

                currentPage.Text = $"{dgContents.CurrentPageIndex + 1}/{dgContents.PageCount}";
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
                    dgContents.CurrentPageIndex = 0;
                    break;
                case "PREVIOUS":
                    dgContents.CurrentPageIndex =
                        Math.Max(dgContents.CurrentPageIndex - 1, 0);
                    break;
                case "NEXT":
                    dgContents.CurrentPageIndex =
                        Math.Min(dgContents.CurrentPageIndex + 1,
                        dgContents.PageCount - 1);
                    break;
                case "LAST":
                    dgContents.CurrentPageIndex = dgContents.PageCount - 1;
                    break;
                default:
                    break;
            }
            BindGrid();
        }
	}
}
