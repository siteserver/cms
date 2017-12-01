using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;

namespace SiteServer.BackgroundPages.WeiXin
{
	public class PageAlbumContent : BasePageCms
	{
        public Repeater RptParentAlbumContents;
         
        public Button BtnAddAlbumContent;

        public int AlbumId;

        public static string GetRedirectUrl(int publishmentSystemId, int albumId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageAlbumContent), new NameValueCollection
            {
                {"PublishmentSystemId", publishmentSystemId.ToString()},
                {"albumID", albumId.ToString()}
            });
        }

        public static string GetRedirectUrl(int publishmentSystemId, int albumId,int id)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageAlbumContent), new NameValueCollection
            {
                {"PublishmentSystemId", publishmentSystemId.ToString()},
                {"albumID", albumId.ToString()},
                {"id", id.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            AlbumId = Body.GetQueryInt("albumID");

			if (Request.QueryString["delete"] != null)
			{
                var id = TranslateUtils.ToInt(Request.QueryString["id"]);
			
				try
				{
                    DataProviderWx.AlbumContentDao.Delete(PublishmentSystemId, id);
                    DataProviderWx.AlbumContentDao.DeleteByParentId(PublishmentSystemId, id);
					SuccessDeleteMessage();
				}
				catch(Exception ex)
				{
                    FailDeleteMessage(ex);
				}
			}

			if (!IsPostBack)
            { 
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdAlbum, "相册管理", AppManager.WeiXin.Permission.WebSite.Album);
                RptParentAlbumContents.DataSource = DataProviderWx.AlbumContentDao.GetDataSource(PublishmentSystemId, AlbumId);
                RptParentAlbumContents.ItemDataBound += rptParentContents_ItemDataBound;
                RptParentAlbumContents.DataBind();

                BtnAddAlbumContent.Attributes.Add("onclick", ModalAlbumContentAdd.GetOpenWindowStringToAdd(PublishmentSystemId, AlbumId,0));
                
			}
		}

        void rptParentContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var id = SqlUtils.EvalInt(e.Item.DataItem, "ID");
                var albumId = SqlUtils.EvalInt(e.Item.DataItem, "AlbumID");
                var parentId = SqlUtils.EvalInt(e.Item.DataItem, "ParentID");
                var title = SqlUtils.EvalString(e.Item.DataItem, "Title");
                var imageUrl = SqlUtils.EvalString(e.Item.DataItem, "ImageUrl");
                var largeImageUrl = SqlUtils.EvalString(e.Item.DataItem, "LargeImageUrl");

                var albumInfo = DataProviderWx.AlbumDao.GetAlbumInfo(albumId);
                var keywordInfo = DataProviderWx.KeywordDao.GetKeywordInfo(albumInfo.KeywordId);
                var count=DataProviderWx.AlbumContentDao.GetCount(PublishmentSystemId, id);

                var ltlkeywrods = e.Item.FindControl("ltlkeywrods") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlLargeImageUrl = e.Item.FindControl("ltlLargeImageUrl") as Literal;

                var dlAlbumContents = e.Item.FindControl("dlAlbumContents") as DataList;


                var ltlAddUrl = e.Item.FindControl("ltlAddUrl") as Literal;
                var ltlDeleteUrl = e.Item.FindControl("ltlDeleteUrl") as Literal;

                ltlkeywrods.Text =
                    $@" <a href=""javascript:;"" onclick=""{ModalAlbumContentAdd.GetOpenWindowStringToEdit(PublishmentSystemId, this.AlbumId, id)}"">编辑相册</a>";
                ltlAddDate.Text =DateUtils.GetDateString(keywordInfo.AddDate);

                ltlTitle.Text = $@"<a href=""{"javascript:;"}"" target=""_blank"">{title}&nbsp;({count})</a>";
                ltlLargeImageUrl.Text =
                    $@"<img src=""{PageUtility.ParseNavigationUrl(PublishmentSystemInfo, largeImageUrl)}"" class=""appmsg_thumb"">";


                dlAlbumContents.DataSource = DataProviderWx.AlbumContentDao.GetDataSource(PublishmentSystemId, this.AlbumId, id);
                dlAlbumContents.ItemDataBound +=dlContents_ItemDataBound;
                dlAlbumContents.DataBind();


                ltlAddUrl.Text =
                    $@"<a class=""js_edit"" href=""javascript:;"" onclick=""{ModalAlbumContentPhotoUpload.GetOpenWindowStringToAdd(PublishmentSystemId, this.AlbumId, id)}""><i class=""icon18_common edit_gray"">上传照片</i></a>";
                
                ltlDeleteUrl.Text =
                    $@"<a class=""js_delno_extra"" href=""{GetRedirectUrl(PublishmentSystemId,
                        this.AlbumId)}&delete=true&id={id}"" onclick=""javascript:return confirm('此操作将删除相册“{title}”，确认吗？');""><i class=""icon18_common del_gray"">删除</i></a>";

            }
        }

        void dlContents_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                
                var imageUrl = SqlUtils.EvalString(e.Item.DataItem, "ImageUrl");
                var largeImageUrl = SqlUtils.EvalString(e.Item.DataItem, "LargeImageUrl");

                var tbContentImageUrl = e.Item.FindControl("tbContentImageUrl") as TextBox;
                var ltlImageUrl = e.Item.FindControl("ltlImageUrl") as Literal;

                if (string.IsNullOrEmpty(imageUrl))
                {
                    ltlImageUrl.Text = @"<i class=""appmsg_thumb default"">缩略图</i>";
                }
                else
                {
                    var previewImageClick = ModalMessage.GetOpenWindowStringToPreviewImage(PublishmentSystemId, tbContentImageUrl.ClientID);
                    tbContentImageUrl.Text = PageUtility.ParseNavigationUrl(PublishmentSystemInfo, largeImageUrl);
                    ltlImageUrl.Text =
                        $@"<a  href=""javascript:;"" onclick=""{previewImageClick}""> <img class=""img-rounded"" style=""width:80px;"" src=""{PageUtility
                            .ParseNavigationUrl(PublishmentSystemInfo, imageUrl)}""> </a>";
                }
            }
        }        
	}
}
