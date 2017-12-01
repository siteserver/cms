using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalAppointmentItemAdd : BasePageCms
    {
        public Literal LtlPageTitle;
        public TextBox TbTitle;
        public CheckBox CbIsDescription;
        public TextBox TbDescriptionTitle;
        public TextBox TbDescription;
        public Literal LtlTopImageUrl;
        public HtmlInputHidden TopImageUrl;

        public CheckBox CbIsImageUrl;
        public TextBox TbImageUrlTitle;
        public TextBox TbContentImageUrl;
        public Literal LtlContentImageUrl;

        public CheckBox CbIsVideoUrl;
        public TextBox TbVideoUrlTitle;
        public TextBox TbContentVideoUrl;
        public Literal LtlContentVideoUrl;

        public CheckBox CbIsImageUrlCollection;
        public TextBox TbImageUrlCollectionTitle;
        public Literal LtlScript;
        public HtmlInputHidden ImageUrlCollection;
        public HtmlInputHidden LargeImageUrlCollection;
         
        public CheckBox CbIsMap;
        public TextBox TbMapTitle;
        public TextBox TbMapAddress;
        //public Button btnMapAddress;
        public Literal LtlMap;
       
        public CheckBox CbIsTel;
        public TextBox TbTelTitle;
        public TextBox TbTel;

        private int _appointmentId;
        private int _appointmentItemId;
         
        public static string GetOpenWindowStringToAdd(int publishmentSystemId, int appointmentId, int appointmentItemId)
        {
            return PageUtils.GetOpenWindowString("添加预约项目",
                PageUtils.GetWeiXinUrl(nameof(ModalAppointmentItemAdd), new NameValueCollection
                {
                    {"PublishmentSystemId", publishmentSystemId.ToString()},
                    {"appointmentID", appointmentId.ToString()},
                    {"appointmentItemID", appointmentItemId.ToString()}
                }));
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int appointmentId, int appointmentItemId)
        {
            return PageUtils.GetOpenWindowString("编辑预约项目",
                PageUtils.GetWeiXinUrl(nameof(ModalAppointmentItemAdd), new NameValueCollection
                {
                    {"PublishmentSystemId", publishmentSystemId.ToString()},
                    {"appointmentID", appointmentId.ToString()},
                    {"appointmentItemID", appointmentItemId.ToString()}
                }));
        }

        public string GetUploadUrl()
        {
            return string.Empty;
            //return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemId);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _appointmentId = Body.GetQueryInt("appointmentID");
            _appointmentItemId = Body.GetQueryInt("appointmentItemID");

            var selectImageClick = ModalSelectImage.GetOpenWindowString(PublishmentSystemInfo, TbContentImageUrl.ClientID);
            var uploadImageClick = ModalUploadImageSingle.GetOpenWindowStringToTextBox(PublishmentSystemId, TbContentImageUrl.ClientID);
            var cuttingImageClick = ModalCuttingImage.GetOpenWindowStringWithTextBox(PublishmentSystemId, TbContentImageUrl.ClientID);
            var previewImageClick = ModalMessage.GetOpenWindowStringToPreviewImage(PublishmentSystemId, TbContentImageUrl.ClientID);
            LtlContentImageUrl.Text = $@"
                      <a class=""btn"" href=""javascript:;"" onclick=""{selectImageClick};return false;"" title=""选择""><i class=""icon-th""></i></a>
                      <a class=""btn"" href=""javascript:;"" onclick=""{uploadImageClick};return false;"" title=""上传""><i class=""icon-arrow-up""></i></a>
                      <a class=""btn"" href=""javascript:;"" onclick=""{cuttingImageClick};return false;"" title=""裁切""><i class=""icon-crop""></i></a>
                      <a class=""btn"" href=""javascript:;"" onclick=""{previewImageClick};return false;"" title=""预览""><i class=""icon-eye-open""></i></a>";

            var selectVideoClick = ModalSelectVideo.GetOpenWindowString(PublishmentSystemInfo, TbContentVideoUrl.ClientID);
            var uploadVideoClick = ModalUploadVideo.GetOpenWindowStringToTextBox(PublishmentSystemId, TbContentVideoUrl.ClientID);
            var previewVideoClick = ModalMessage.GetOpenWindowStringToPreviewVideoByUrl(PublishmentSystemId, TbContentVideoUrl.ClientID);
            LtlContentVideoUrl.Text = $@"
                      <a class=""btn"" href=""javascript:;"" onclick=""{selectVideoClick};return false;"" title=""选择""><i class=""icon-th""></i></a>
                      <a class=""btn"" href=""javascript:;"" onclick=""{uploadVideoClick};return false;"" title=""上传""><i class=""icon-arrow-up""></i></a>
                      <a class=""btn"" href=""javascript:;"" onclick=""{previewVideoClick};return false;"" title=""预览""><i class=""icon-eye-open""></i></a>";

            if (!IsPostBack)
            {
                LtlTopImageUrl.Text =
                    $@"<img id=""preview_topImageUrl"" src=""{AppointmentManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                 
                if (_appointmentItemId > 0)
                {
                    var appointmentItemInfo = DataProviderWx.AppointmentItemDao.GetItemInfo(_appointmentItemId);
                    if (appointmentItemInfo != null)
                    {
                        TbTitle.Text = appointmentItemInfo.Title;
                        TopImageUrl.Value = appointmentItemInfo.TopImageUrl;
                        CbIsDescription.Checked = appointmentItemInfo.IsDescription;
                        TbDescriptionTitle.Text = appointmentItemInfo.DescriptionTitle;
                        TbDescription.Text = appointmentItemInfo.Description;
                        CbIsImageUrl.Checked = appointmentItemInfo.IsImageUrl;
                        TbImageUrlTitle.Text = appointmentItemInfo.ImageUrlTitle;
                        TbContentImageUrl.Text = appointmentItemInfo.ImageUrl;
                        CbIsVideoUrl.Checked = appointmentItemInfo.IsVideoUrl;
                        TbVideoUrlTitle.Text = appointmentItemInfo.VideoUrlTitle;
                        TbContentVideoUrl.Text = appointmentItemInfo.VideoUrl;
                        CbIsImageUrlCollection.Checked = appointmentItemInfo.IsImageUrlCollection;
                        TbImageUrlCollectionTitle.Text = appointmentItemInfo.ImageUrlCollectionTitle;
                        ImageUrlCollection.Value = appointmentItemInfo.ImageUrlCollection;
                        LargeImageUrlCollection.Value = appointmentItemInfo.LargeImageUrlCollection;
                        CbIsMap.Checked = appointmentItemInfo.IsMap;
                        TbMapTitle.Text = appointmentItemInfo.MapTitle;
                        TbMapAddress.Text = appointmentItemInfo.MapAddress;
                        CbIsTel.Checked = appointmentItemInfo.IsTel;
                        TbTelTitle.Text = appointmentItemInfo.TelTitle;
                        TbTel.Text = appointmentItemInfo.Tel;

                        if (!string.IsNullOrEmpty(appointmentItemInfo.TopImageUrl))
                        {
                            LtlTopImageUrl.Text =
                                $@"<img id=""preview_topImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                    PublishmentSystemInfo, appointmentItemInfo.TopImageUrl)}"" width=""370"" align=""middle"" />";
                        }
                        if (!string.IsNullOrEmpty(appointmentItemInfo.MapAddress))
                        { 
                            LtlMap.Text =
                                $@"<iframe style=""width:100%;height:100%;background-color:#ffffff;margin-bottom:15px;"" scrolling=""auto"" frameborder=""0"" width=""100%"" height=""100%"" src=""{MapManager.GetMapUrl(PublishmentSystemInfo, TbMapAddress.Text)}""></iframe>";
                        }
                        if (!string.IsNullOrEmpty(appointmentItemInfo.ImageUrlCollection))
                        { 
                            var scriptBuilder = new StringBuilder();
                            scriptBuilder.AppendFormat(@"
addImage('{0}','{1}');
", appointmentItemInfo.ImageUrlCollection, appointmentItemInfo.LargeImageUrlCollection);

                            LtlScript.Text = $@"
$(document).ready(function(){{
	{scriptBuilder}
}});
";
                        }
                    }
                }
             }

           // this.btnAddImageUrl.Attributes.Add("onclick", Modal.AppointmentItemPhotoUpload.GetOpenWindowStringToAdd(base.PublishmentSystemId, this.imageUrlCollection.Value));
         }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                var isAdd = false;
                var appointmentItemInfo = new AppointmentItemInfo();
                appointmentItemInfo.PublishmentSystemId = PublishmentSystemId;
                if (_appointmentItemId > 0)
                {
                    appointmentItemInfo = DataProviderWx.AppointmentItemDao.GetItemInfo(_appointmentItemId);
                }

                appointmentItemInfo.AppointmentId = _appointmentId;
                appointmentItemInfo.Title = TbTitle.Text;
                appointmentItemInfo.TopImageUrl = TopImageUrl.Value;
                appointmentItemInfo.IsDescription = CbIsDescription.Checked;
                appointmentItemInfo.DescriptionTitle = TbDescriptionTitle.Text;
                appointmentItemInfo.Description = TbDescription.Text;
                appointmentItemInfo.IsImageUrl = CbIsImageUrl.Checked;
                appointmentItemInfo.ImageUrlTitle = TbImageUrlTitle.Text;
                appointmentItemInfo.ImageUrl = TbContentImageUrl.Text;
                appointmentItemInfo.IsVideoUrl = CbIsVideoUrl.Checked;
                appointmentItemInfo.VideoUrlTitle = TbVideoUrlTitle.Text;
                appointmentItemInfo.VideoUrl = TbContentVideoUrl.Text;
                appointmentItemInfo.IsImageUrlCollection = CbIsImageUrlCollection.Checked;
                appointmentItemInfo.ImageUrlCollectionTitle = TbImageUrlCollectionTitle.Text;
                appointmentItemInfo.ImageUrlCollection = ImageUrlCollection.Value;
                appointmentItemInfo.LargeImageUrlCollection = LargeImageUrlCollection.Value;
                appointmentItemInfo.IsMap = CbIsMap.Checked;
                appointmentItemInfo.MapTitle = TbMapTitle.Text;
                appointmentItemInfo.MapAddress = TbMapAddress.Text;
                appointmentItemInfo.IsTel = CbIsTel.Checked;
                appointmentItemInfo.TelTitle = TbTelTitle.Text;
                appointmentItemInfo.Tel = TbTel.Text;

                if (_appointmentItemId > 0)
                {
                    DataProviderWx.AppointmentItemDao.Update(appointmentItemInfo);
                    Body.AddSiteLog(PublishmentSystemId, "修改预约项目", $"预约项目:{TbTitle.Text}");
                }
                else
                {
                    isAdd = true;
                    _appointmentItemId = DataProviderWx.AppointmentItemDao.Insert(appointmentItemInfo);
                    Body.AddSiteLog(PublishmentSystemId, "新增预约项目", $"预约项目:{TbTitle.Text}");
                }

                string scripts =
                    $"window.parent.addItem('{TbTitle.Text}', '{TbMapAddress.Text}','{TbTel.Text}','{PublishmentSystemId}','{_appointmentId}','{_appointmentItemId}','{isAdd}');";
                PageUtils.CloseModalPageWithoutRefresh(Page, scripts);
            }
            catch (Exception ex)
            {
                FailMessage(ex, "失败：" + ex.Message);
            }
        }

        public void Preview_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            { 
                LtlMap.Text =
                    $@"<iframe style=""width:100%;height:100%;background-color:#ffffff;margin-bottom:15px;"" scrolling=""auto"" frameborder=""0"" width=""100%"" height=""100%"" src=""{MapManager.GetMapUrl(PublishmentSystemInfo, TbMapAddress.Text)}""></iframe>";
            }
        }
       
    }
}
