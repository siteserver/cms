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
        public Literal ltlPageTitle;
        public TextBox tbTitle;
        public CheckBox cbIsDescription;
        public TextBox tbDescriptionTitle;
        public TextBox tbDescription;
        public Literal ltlTopImageUrl;
        public HtmlInputHidden topImageUrl;

        public CheckBox cbIsImageUrl;
        public TextBox tbImageUrlTitle;
        public TextBox tbContentImageUrl;
        public Literal ltlContentImageUrl;

        public CheckBox cbIsVideoUrl;
        public TextBox tbVideoUrlTitle;
        public TextBox tbContentVideoUrl;
        public Literal ltlContentVideoUrl;

        public CheckBox cbIsImageUrlCollection;
        public TextBox tbImageUrlCollectionTitle;
        public Literal ltlScript;
        public HtmlInputHidden imageUrlCollection;
        public HtmlInputHidden largeImageUrlCollection;
         
        public CheckBox cbIsMap;
        public TextBox tbMapTitle;
        public TextBox tbMapAddress;
        //public Button btnMapAddress;
        public Literal ltlMap;
       
        public CheckBox cbIsTel;
        public TextBox tbTelTitle;
        public TextBox tbTel;

        private int appointmentID;
        private int appointmentItemID;
         
        public static string GetOpenWindowStringToAdd(int publishmentSystemId, int appointmentID, int appointmentItemID)
        {
            return PageUtils.GetOpenWindowString("添加预约项目",
                PageUtils.GetWeiXinUrl(nameof(ModalAppointmentItemAdd), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"appointmentID", appointmentID.ToString()},
                    {"appointmentItemID", appointmentItemID.ToString()}
                }));
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int appointmentID, int appointmentItemID)
        {
            return PageUtils.GetOpenWindowString("编辑预约项目",
                PageUtils.GetWeiXinUrl(nameof(ModalAppointmentItemAdd), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"appointmentID", appointmentID.ToString()},
                    {"appointmentItemID", appointmentItemID.ToString()}
                }));
        }

        public string GetUploadUrl()
        {
            return string.Empty;
            //return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            appointmentID = Body.GetQueryInt("appointmentID");
            appointmentItemID = Body.GetQueryInt("appointmentItemID");

            var selectImageClick = ModalSelectImage.GetOpenWindowString(PublishmentSystemInfo, tbContentImageUrl.ClientID);
            var uploadImageClick = ModalUploadImageSingle.GetOpenWindowStringToTextBox(PublishmentSystemId, tbContentImageUrl.ClientID);
            var cuttingImageClick = ModalCuttingImage.GetOpenWindowStringWithTextBox(PublishmentSystemId, tbContentImageUrl.ClientID);
            var previewImageClick = ModalMessage.GetOpenWindowStringToPreviewImage(PublishmentSystemId, tbContentImageUrl.ClientID);
            ltlContentImageUrl.Text = $@"
                      <a class=""btn"" href=""javascript:;"" onclick=""{selectImageClick};return false;"" title=""选择""><i class=""icon-th""></i></a>
                      <a class=""btn"" href=""javascript:;"" onclick=""{uploadImageClick};return false;"" title=""上传""><i class=""icon-arrow-up""></i></a>
                      <a class=""btn"" href=""javascript:;"" onclick=""{cuttingImageClick};return false;"" title=""裁切""><i class=""icon-crop""></i></a>
                      <a class=""btn"" href=""javascript:;"" onclick=""{previewImageClick};return false;"" title=""预览""><i class=""icon-eye-open""></i></a>";

            var selectVideoClick = ModalSelectVideo.GetOpenWindowString(PublishmentSystemInfo, tbContentVideoUrl.ClientID);
            var uploadVideoClick = ModalUploadVideo.GetOpenWindowStringToTextBox(PublishmentSystemId, tbContentVideoUrl.ClientID);
            var previewVideoClick = ModalMessage.GetOpenWindowStringToPreviewVideoByUrl(PublishmentSystemId, tbContentVideoUrl.ClientID);
            ltlContentVideoUrl.Text = $@"
                      <a class=""btn"" href=""javascript:;"" onclick=""{selectVideoClick};return false;"" title=""选择""><i class=""icon-th""></i></a>
                      <a class=""btn"" href=""javascript:;"" onclick=""{uploadVideoClick};return false;"" title=""上传""><i class=""icon-arrow-up""></i></a>
                      <a class=""btn"" href=""javascript:;"" onclick=""{previewVideoClick};return false;"" title=""预览""><i class=""icon-eye-open""></i></a>";

            if (!IsPostBack)
            {
                ltlTopImageUrl.Text =
                    $@"<img id=""preview_topImageUrl"" src=""{AppointmentManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                 
                if (appointmentItemID > 0)
                {
                    var appointmentItemInfo = DataProviderWX.AppointmentItemDAO.GetItemInfo(appointmentItemID);
                    if (appointmentItemInfo != null)
                    {
                        tbTitle.Text = appointmentItemInfo.Title;
                        topImageUrl.Value = appointmentItemInfo.TopImageUrl;
                        cbIsDescription.Checked = appointmentItemInfo.IsDescription;
                        tbDescriptionTitle.Text = appointmentItemInfo.DescriptionTitle;
                        tbDescription.Text = appointmentItemInfo.Description;
                        cbIsImageUrl.Checked = appointmentItemInfo.IsImageUrl;
                        tbImageUrlTitle.Text = appointmentItemInfo.ImageUrlTitle;
                        tbContentImageUrl.Text = appointmentItemInfo.ImageUrl;
                        cbIsVideoUrl.Checked = appointmentItemInfo.IsVideoUrl;
                        tbVideoUrlTitle.Text = appointmentItemInfo.VideoUrlTitle;
                        tbContentVideoUrl.Text = appointmentItemInfo.VideoUrl;
                        cbIsImageUrlCollection.Checked = appointmentItemInfo.IsImageUrlCollection;
                        tbImageUrlCollectionTitle.Text = appointmentItemInfo.ImageUrlCollectionTitle;
                        imageUrlCollection.Value = appointmentItemInfo.ImageUrlCollection;
                        largeImageUrlCollection.Value = appointmentItemInfo.LargeImageUrlCollection;
                        cbIsMap.Checked = appointmentItemInfo.IsMap;
                        tbMapTitle.Text = appointmentItemInfo.MapTitle;
                        tbMapAddress.Text = appointmentItemInfo.MapAddress;
                        cbIsTel.Checked = appointmentItemInfo.IsTel;
                        tbTelTitle.Text = appointmentItemInfo.TelTitle;
                        tbTel.Text = appointmentItemInfo.Tel;

                        if (!string.IsNullOrEmpty(appointmentItemInfo.TopImageUrl))
                        {
                            ltlTopImageUrl.Text =
                                $@"<img id=""preview_topImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                    PublishmentSystemInfo, appointmentItemInfo.TopImageUrl)}"" width=""370"" align=""middle"" />";
                        }
                        if (!string.IsNullOrEmpty(appointmentItemInfo.MapAddress))
                        { 
                            ltlMap.Text =
                                $@"<iframe style=""width:100%;height:100%;background-color:#ffffff;margin-bottom:15px;"" scrolling=""auto"" frameborder=""0"" width=""100%"" height=""100%"" src=""{MapManager.GetMapUrl(PublishmentSystemInfo, tbMapAddress.Text)}""></iframe>";
                        }
                        if (!string.IsNullOrEmpty(appointmentItemInfo.ImageUrlCollection))
                        { 
                            var scriptBuilder = new StringBuilder();
                            scriptBuilder.AppendFormat(@"
addImage('{0}','{1}');
", appointmentItemInfo.ImageUrlCollection, appointmentItemInfo.LargeImageUrlCollection);

                            ltlScript.Text = $@"
$(document).ready(function(){{
	{scriptBuilder.ToString()}
}});
";
                        }
                    }
                }
             }

           // this.btnAddImageUrl.Attributes.Add("onclick", Modal.AppointmentItemPhotoUpload.GetOpenWindowStringToAdd(base.PublishmentSystemID, this.imageUrlCollection.Value));
         }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                var isAdd = false;
                var appointmentItemInfo = new AppointmentItemInfo();
                appointmentItemInfo.PublishmentSystemID = PublishmentSystemId;
                if (appointmentItemID > 0)
                {
                    appointmentItemInfo = DataProviderWX.AppointmentItemDAO.GetItemInfo(appointmentItemID);
                }

                appointmentItemInfo.AppointmentID = appointmentID;
                appointmentItemInfo.Title = tbTitle.Text;
                appointmentItemInfo.TopImageUrl = topImageUrl.Value;
                appointmentItemInfo.IsDescription = cbIsDescription.Checked;
                appointmentItemInfo.DescriptionTitle = tbDescriptionTitle.Text;
                appointmentItemInfo.Description = tbDescription.Text;
                appointmentItemInfo.IsImageUrl = cbIsImageUrl.Checked;
                appointmentItemInfo.ImageUrlTitle = tbImageUrlTitle.Text;
                appointmentItemInfo.ImageUrl = tbContentImageUrl.Text;
                appointmentItemInfo.IsVideoUrl = cbIsVideoUrl.Checked;
                appointmentItemInfo.VideoUrlTitle = tbVideoUrlTitle.Text;
                appointmentItemInfo.VideoUrl = tbContentVideoUrl.Text;
                appointmentItemInfo.IsImageUrlCollection = cbIsImageUrlCollection.Checked;
                appointmentItemInfo.ImageUrlCollectionTitle = tbImageUrlCollectionTitle.Text;
                appointmentItemInfo.ImageUrlCollection = imageUrlCollection.Value;
                appointmentItemInfo.LargeImageUrlCollection = largeImageUrlCollection.Value;
                appointmentItemInfo.IsMap = cbIsMap.Checked;
                appointmentItemInfo.MapTitle = tbMapTitle.Text;
                appointmentItemInfo.MapAddress = tbMapAddress.Text;
                appointmentItemInfo.IsTel = cbIsTel.Checked;
                appointmentItemInfo.TelTitle = tbTelTitle.Text;
                appointmentItemInfo.Tel = tbTel.Text;

                if (appointmentItemID > 0)
                {
                    DataProviderWX.AppointmentItemDAO.Update(appointmentItemInfo);
                    Body.AddLog(PublishmentSystemId, "修改预约项目", $"预约项目:{tbTitle.Text}");
                }
                else
                {
                    isAdd = true;
                    appointmentItemID = DataProviderWX.AppointmentItemDAO.Insert(appointmentItemInfo);
                    Body.AddLog(PublishmentSystemId, "新增预约项目", $"预约项目:{tbTitle.Text}");
                }

                string scripts =
                    $"window.parent.addItem('{tbTitle.Text}', '{tbMapAddress.Text}','{tbTel.Text}','{PublishmentSystemId}','{appointmentID}','{appointmentItemID}','{isAdd.ToString()}');";
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
                ltlMap.Text =
                    $@"<iframe style=""width:100%;height:100%;background-color:#ffffff;margin-bottom:15px;"" scrolling=""auto"" frameborder=""0"" width=""100%"" height=""100%"" src=""{MapManager.GetMapUrl(PublishmentSystemInfo, tbMapAddress.Text)}""></iframe>";
            }
        }
       
    }
}
