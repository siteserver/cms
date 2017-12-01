using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageAlbum : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnAdd;
        public Button BtnDelete;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageAlbum), new NameValueCollection
            {
                {"PublishmentSystemId", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWx.AlbumDao.Delete(PublishmentSystemId,list) ;
                        SuccessMessage("微相册删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "微相册删除失败！");
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            SpContents.SelectCommand = DataProviderWx.AlbumDao.GetSelectString(PublishmentSystemId);
            SpContents.SortField = AlbumAttribute.Id;
            SpContents.SortMode = SortMode.ASC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdAlbum, "微相册", AppManager.WeiXin.Permission.WebSite.Album);
                SpContents.DataBind();

                var urlAdd = PageAlbumAdd.GetRedirectUrl(PublishmentSystemId, 0);
                BtnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的微相册", "此操作将删除所选微相册，确认吗？"));
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var albumInfo = new AlbumInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlKeywords = e.Item.FindControl("ltlKeywords") as Literal;
                var ltlPvCount = e.Item.FindControl("ltlPVCount") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlAlbumContentUrl = e.Item.FindControl("ltlAlbumContentUrl") as Literal;
                var ltlPreviewUrl = e.Item.FindControl("ltlPreviewUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = albumInfo.Title;
                ltlKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(albumInfo.KeywordId);
                ltlPvCount.Text = albumInfo.PvCount.ToString();

                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(!albumInfo.IsDisabled);

                var urlAlbumContent = PageAlbumContent.GetRedirectUrl(PublishmentSystemId, albumInfo.Id);

                ltlAlbumContentUrl.Text = $@"<a href=""{urlAlbumContent}"">微相册</a>";

                var urlPreview = AlbumManager.GetAlbumUrl(PublishmentSystemInfo, albumInfo, string.Empty);
                //urlPreview = BackgroundPreview.GetRedirectUrlToMobile(urlPreview);
                ltlPreviewUrl.Text = $@"<a href=""{urlPreview}"" target=""_blank"">预览</a>";

                var urlEdit = PageAlbumAdd.GetRedirectUrl(PublishmentSystemId, albumInfo.Id);
                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">编辑</a>";
            }
        }
    }
}
