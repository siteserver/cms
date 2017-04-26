using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;

namespace SiteServer.BackgroundPages.WeiXin
{
	public class PageAlbumContent : BasePageCms
	{
        public Repeater rptParentAlbumContents;
         
        public Button btnAddAlbumContent;

        public int albumID;

        public static string GetRedirectUrl(int publishmentSystemId, int albumID)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageAlbumContent), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"albumID", albumID.ToString()}
            });
        }

        public static string GetRedirectUrl(int publishmentSystemId, int albumID,int id)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageAlbumContent), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"albumID", albumID.ToString()},
                {"id", id.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            albumID = Body.GetQueryInt("albumID");

			if (Request.QueryString["delete"] != null)
			{
                var id = TranslateUtils.ToInt(Request.QueryString["id"]);
			
				try
				{
                    DataProviderWX.AlbumContentDAO.Delete(PublishmentSystemId, id);
                    DataProviderWX.AlbumContentDAO.DeleteByParentID(PublishmentSystemId, id);
					SuccessDeleteMessage();
				}
				catch(Exception ex)
				{
                    FailDeleteMessage(ex);
				}
			}

			if (!IsPostBack)
            { 
                BreadCrumb(AppManager.WeiXin.LeftMenu.IdFunction, AppManager.WeiXin.LeftMenu.Function.IdAlbum, "相册管理", AppManager.WeiXin.Permission.WebSite.Album);
                rptParentAlbumContents.DataSource = DataProviderWX.AlbumContentDAO.GetDataSource(PublishmentSystemId, albumID);
                rptParentAlbumContents.ItemDataBound += rptParentContents_ItemDataBound;
                rptParentAlbumContents.DataBind();

                btnAddAlbumContent.Attributes.Add("onclick", ModalAlbumContentAdd.GetOpenWindowStringToAdd(PublishmentSystemId, albumID,0));
                
			}
		}

        void rptParentContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var id = TranslateUtils.EvalInt(e.Item.DataItem, "ID");
                var albumID = TranslateUtils.EvalInt(e.Item.DataItem, "AlbumID");
                var parentID = TranslateUtils.EvalInt(e.Item.DataItem, "ParentID");
                var title = TranslateUtils.EvalString(e.Item.DataItem, "Title");
                var imageUrl = TranslateUtils.EvalString(e.Item.DataItem, "ImageUrl");
                var largeImageUrl = TranslateUtils.EvalString(e.Item.DataItem, "LargeImageUrl");

                var albumInfo = DataProviderWX.AlbumDAO.GetAlbumInfo(albumID);
                var keywordInfo = DataProviderWX.KeywordDAO.GetKeywordInfo(albumInfo.KeywordID);
                var count=DataProviderWX.AlbumContentDAO.GetCount(PublishmentSystemId, id);

                var ltlkeywrods = e.Item.FindControl("ltlkeywrods") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlLargeImageUrl = e.Item.FindControl("ltlLargeImageUrl") as Literal;

                var dlAlbumContents = e.Item.FindControl("dlAlbumContents") as DataList;


                var ltlAddUrl = e.Item.FindControl("ltlAddUrl") as Literal;
                var ltlDeleteUrl = e.Item.FindControl("ltlDeleteUrl") as Literal;

                ltlkeywrods.Text =
                    $@" <a href=""javascript:;"" onclick=""{ModalAlbumContentAdd.GetOpenWindowStringToEdit(PublishmentSystemId, this.albumID, id)}"">编辑相册</a>";
                ltlAddDate.Text =DateUtils.GetDateString(keywordInfo.AddDate);

                ltlTitle.Text = $@"<a href=""{"javascript:;"}"" target=""_blank"">{title}&nbsp;({count})</a>";
                ltlLargeImageUrl.Text =
                    $@"<img src=""{PageUtility.ParseNavigationUrl(PublishmentSystemInfo, largeImageUrl)}"" class=""appmsg_thumb"">";


                dlAlbumContents.DataSource = DataProviderWX.AlbumContentDAO.GetDataSource(PublishmentSystemId, this.albumID, id);
                dlAlbumContents.ItemDataBound +=dlContents_ItemDataBound;
                dlAlbumContents.DataBind();


                ltlAddUrl.Text =
                    $@"<a class=""js_edit"" href=""javascript:;"" onclick=""{ModalAlbumContentPhotoUpload.GetOpenWindowStringToAdd(PublishmentSystemId, this.albumID, id)}""><i class=""icon18_common edit_gray"">上传照片</i></a>";
                
                ltlDeleteUrl.Text =
                    $@"<a class=""js_delno_extra"" href=""{GetRedirectUrl(PublishmentSystemId,
                        this.albumID)}&delete=true&id={id}"" onclick=""javascript:return confirm('此操作将删除相册“{title}”，确认吗？');""><i class=""icon18_common del_gray"">删除</i></a>";

            }
        }

        void dlContents_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                
                var imageUrl = TranslateUtils.EvalString(e.Item.DataItem, "ImageUrl");
                var largeImageUrl = TranslateUtils.EvalString(e.Item.DataItem, "LargeImageUrl");

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
