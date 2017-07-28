using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageAppointmentSingleAdd : BasePageCms
    {
        public Literal ltlPageTitle;

        public PlaceHolder phStep1;
        public TextBox tbKeywords;
        public TextBox tbTitle;
        public TextBox tbSummary;
        public DateTimeTextBox dtbStartDate;
        public DateTimeTextBox dtbEndDate;
        public CheckBox cbIsEnabled;
        public Literal ltlImageUrl;

        public PlaceHolder phStep2;
        public TextBox tbItemTitle;
        public CheckBox cbIsDescription;
        public TextBox tbDescriptionTitle;
        public TextBox tbDescription;
        public Literal ltlTopImageUrl;
        public Literal ltlResultTopImageUrl;
        public HtmlInputHidden topImageUrl;
        public HtmlInputHidden resultTopImageUrl;

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
        public Button btnMapAddress;
        public Literal ltlMap;
        public PlaceHolder phMap;
        
        public CheckBox cbIsTel;
        public TextBox tbTelTitle;
        public TextBox tbTel;
        public PlaceHolder phTel;

        public PlaceHolder phStep3;
        public Literal ltlAwardItems;
        public CheckBox cbIsFormRealName;
        public TextBox tbFormRealNameTitle;
        public CheckBox cbIsFormMobile;
        public TextBox tbFormMobileTitle;
        public CheckBox cbIsFormEmail;
        public TextBox tbFormEmailTitle;
        public CheckBox cbIsFormAddress;
        public TextBox tbFormAddressTitle;

        public PlaceHolder phStep4;
        public TextBox tbEndTitle;
        public TextBox tbEndSummary;
        public Literal ltlEndImageUrl;

        public HtmlInputHidden imageUrl;
        public HtmlInputHidden contentImageUrl;
        public HtmlInputHidden endImageUrl;

        public Button btnSubmit;
        public Button btnReturn;

        private int appointmentID ;
        private int appointmentItemID ;
        public static string GetRedirectUrl(int publishmentSystemId, int appointmentID, int appointmentItemID)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageAppointmentSingleAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"appointmentID", appointmentID.ToString()},
                {"appointmentItemID", appointmentItemID.ToString()}
            });
        }

        public string GetUploadUrl()
        {
            return string.Empty;
            //return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
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
                var pageTitle = appointmentID > 0 ? "编辑微预约" : "添加微预约";
                BreadCrumb(AppManager.WeiXin.LeftMenu.IdFunction, AppManager.WeiXin.LeftMenu.Function.IdAppointment, pageTitle, AppManager.WeiXin.Permission.WebSite.Appointment);
                ltlPageTitle.Text = pageTitle;
                  
                ltlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{AppointmentManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                ltlTopImageUrl.Text =
                    $@"<img id=""preview_topImageUrl"" src=""{AppointmentManager.GetItemTopImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                ltlResultTopImageUrl.Text =
                    $@"<img id=""preview_resultTopImageUrl"" src=""{AppointmentManager.GetContentResultTopImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                ltlContentImageUrl.Text =
                    $@"<img id=""preview_contentImageUrl"" src=""{AppointmentManager.GetContentImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                
                ltlEndImageUrl.Text =
                    $@"<img id=""preview_endImageUrl"" src=""{AppointmentManager.GetEndImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                 
                if (appointmentID == 0)
                { 
                    dtbEndDate.DateTime = DateTime.Now.AddMonths(1);
                }
                else
                {
                    var appointmentInfo = DataProviderWX.AppointmentDAO.GetAppointmentInfo(appointmentID);
                    
                    if (appointmentInfo != null)
                    {
                        tbKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(appointmentInfo.KeywordID);
                        cbIsEnabled.Checked = !appointmentInfo.IsDisabled;
                        dtbStartDate.DateTime = appointmentInfo.StartDate;
                        dtbEndDate.DateTime = appointmentInfo.EndDate;
                        tbTitle.Text = appointmentInfo.Title;
                        if (!string.IsNullOrEmpty(appointmentInfo.ImageUrl))
                        {
                            ltlImageUrl.Text =
                                $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                    PublishmentSystemInfo, appointmentInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                        }
                        if (!string.IsNullOrEmpty(appointmentInfo.ContentResultTopImageUrl))
                        {
                            ltlResultTopImageUrl.Text =
                                $@"<img id=""preview_resultTopImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                    PublishmentSystemInfo, appointmentInfo.ContentResultTopImageUrl)}"" width=""370"" align=""middle"" />";
                        }

                        tbSummary.Text = appointmentInfo.Summary;

                        tbEndTitle.Text = appointmentInfo.EndTitle;
                        tbEndSummary.Text = appointmentInfo.EndSummary;
                        if (!string.IsNullOrEmpty(appointmentInfo.EndImageUrl))
                        {
                            ltlEndImageUrl.Text =
                                $@"<img id=""preview_endImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                    PublishmentSystemInfo, appointmentInfo.EndImageUrl)}"" width=""370"" align=""middle"" />";
                        }

                        imageUrl.Value = appointmentInfo.ImageUrl;
                        contentImageUrl.Value = appointmentInfo.ContentImageUrl;
                        resultTopImageUrl.Value = appointmentInfo.ContentResultTopImageUrl;
                        endImageUrl.Value = appointmentInfo.EndImageUrl;
                        #region 拓展属性
                        #region 姓名
                        if (appointmentInfo.IsFormRealName == "True")
                        {
                            cbIsFormRealName.Checked = true;
                            tbFormRealNameTitle.Text = appointmentInfo.FormRealNameTitle;
                        }
                        else if (string.IsNullOrEmpty(appointmentInfo.IsFormRealName))
                        {
                            cbIsFormRealName.Checked = true;
                            tbFormRealNameTitle.Text = "姓名";
                        }
                        else
                        {
                            cbIsFormRealName.Checked = false;
                            tbFormRealNameTitle.Text = appointmentInfo.FormRealNameTitle;
                        }
                        #endregion
                        #region 电话
                        if (appointmentInfo.IsFormMobile == "True")
                        {
                            cbIsFormMobile.Checked = true;
                            tbFormMobileTitle.Text = appointmentInfo.FormMobileTitle;
                        }
                        else if (string.IsNullOrEmpty(appointmentInfo.IsFormMobile))
                        {
                            cbIsFormMobile.Checked = true;
                            tbFormMobileTitle.Text = "电话";
                        }
                        else
                        {
                            cbIsFormMobile.Checked = false;
                            tbFormMobileTitle.Text = appointmentInfo.FormMobileTitle;
                        }
                        #endregion
                        #region 邮箱
                        if (appointmentInfo.IsFormEmail == "True")
                        {
                            cbIsFormEmail.Checked = true;
                            tbFormEmailTitle.Text = appointmentInfo.FormEmailTitle;
                        }
                        else if (string.IsNullOrEmpty(appointmentInfo.IsFormEmail))
                        {
                            cbIsFormEmail.Checked = true;
                            tbFormEmailTitle.Text = "电话";
                        }
                        else
                        {
                            cbIsFormEmail.Checked = false;
                            tbFormEmailTitle.Text = appointmentInfo.FormEmailTitle;
                        }
                        #endregion

                        appointmentItemID = DataProviderWX.AppointmentItemDAO.GetItemID(PublishmentSystemId, appointmentID);

                        var configExtendInfoList = DataProviderWX.ConfigExtendDAO.GetConfigExtendInfoList(PublishmentSystemId, appointmentInfo.ID, EKeywordTypeUtils.GetValue(EKeywordType.Appointment));
                        var itemBuilder=new StringBuilder();
                        foreach (var configExtendInfo in configExtendInfoList)
                        {
                            if (string.IsNullOrEmpty(configExtendInfo.IsVisible))
                            {
                                configExtendInfo.IsVisible = "checked=checked";
                            }
                            else if (configExtendInfo.IsVisible == "True")
                            {
                                configExtendInfo.IsVisible = "checked=checked";
                            }
                            else
                            {
                                configExtendInfo.IsVisible = "";
                            }
                            itemBuilder.AppendFormat(@"{{id: '{0}', attributeName: '{1}',isVisible:'{2}'}},", configExtendInfo.ID, configExtendInfo.AttributeName, configExtendInfo.IsVisible);
                        }
                        if (itemBuilder.Length > 0) itemBuilder.Length--;
                        ltlAwardItems.Text =
                            $@"itemController.itemCount = {configExtendInfoList.Count};itemController.items = [{itemBuilder
                                .ToString()}];";
                        #endregion
                    }
                }

                if (appointmentItemID > 0)
                {
                    var appointmentItemInfo = DataProviderWX.AppointmentItemDAO.GetItemInfo(appointmentItemID);
                    if (appointmentItemInfo != null)
                    {
                        tbItemTitle.Text = appointmentItemInfo.Title;
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
                                $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
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
	{scriptBuilder}
}});
";
                        }
                    }
                }
               
                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageAppointment.GetRedirectUrl(PublishmentSystemId)}"";return false");
			}
		}

		public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                var selectedStep = 0;
                if (phStep1.Visible)
                {
                    selectedStep = 1;
                }
                else if (phStep2.Visible)
                {
                    selectedStep = 2;
                }
                else if (phStep3.Visible)
                {
                    selectedStep = 3;
                }
                else if (phStep4.Visible)
                {
                    selectedStep = 4;
                }

			    phStep1.Visible = phStep2.Visible = phStep3.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(tbKeywords.Text))
                    {
                        if (appointmentID > 0)
                        {
                            var appointmentInfo = DataProviderWX.AppointmentDAO.GetAppointmentInfo(appointmentID);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, appointmentInfo.KeywordID, PageUtils.FilterXss(tbKeywords.Text), out conflictKeywords);
                        }
                        else
                        {
                            isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemId, PageUtils.FilterXss(tbKeywords.Text), out conflictKeywords);
                        }
                    }
                    
                    if (isConflict)
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                    }
                    else
                    {
                        phStep2.Visible = true;
                    }
                }
                else if (selectedStep == 2)
                {
                    var isItemReady = true;
                    var appointmentItemInfo = new AppointmentItemInfo();
                    appointmentItemInfo.PublishmentSystemID = PublishmentSystemId;
                    if (appointmentItemID > 0)
                    {
                        appointmentItemInfo = DataProviderWX.AppointmentItemDAO.GetItemInfo(appointmentItemID);
                    }

                    appointmentItemInfo.AppointmentID = appointmentID;
                    appointmentItemInfo.Title =PageUtils.FilterXss(tbItemTitle.Text);
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

                    try
                    {
                        if (appointmentItemID > 0)
                        {
                            DataProviderWX.AppointmentItemDAO.Update(appointmentItemInfo);
                            Body.AddLog(PublishmentSystemId, "修改预约项目", $"预约项目:{tbTitle.Text}");
                        }
                        else
                        {
                            appointmentItemID = DataProviderWX.AppointmentItemDAO.Insert(appointmentItemInfo);

                            Body.AddLog(PublishmentSystemId, "新增预约项目", $"预约项目:{tbTitle.Text}");
                        }
                    }
                    catch (Exception ex)
                    {
                        isItemReady = false;
                        FailMessage(ex, "微预约项目设置失败！");
                    }

                    if (isItemReady)
                    {
                        phStep3.Visible = true;
                    }
                    else
                    {
                        phStep2.Visible = true;
                    }
                }
                else if (selectedStep == 3)
                {
                    var isItemReady = true;
                    var itemCount = TranslateUtils.ToInt(Request.Form["itemCount"]);

                    var itemIDList = TranslateUtils.StringCollectionToIntList(Request.Form["itemID"]);
                    var attributeNameList = TranslateUtils.StringCollectionToStringList(Request.Form["itemAttributeName"]);

                    var itemIsVisible = "off";
                    if (!string.IsNullOrEmpty(Request.Form["itemIsVisible"]))
                    {
                        itemIsVisible = Request.Form["itemIsVisible"];
                    }

                    var isVisibleList = TranslateUtils.StringCollectionToStringList(itemIsVisible);

                    if (isVisibleList.Count < itemIDList.Count)
                    {
                        for (var i = isVisibleList.Count; i < itemIDList.Count; i++)
                        {
                            isVisibleList.Add("off");
                        }
                    }

                    var configExtendInfoList = new List<ConfigExtendInfo>();
                    for (var i = 0; i < itemCount; i++)
                    {
                        var configExtendInfo = new ConfigExtendInfo { ID = itemIDList[i], PublishmentSystemID = PublishmentSystemId, KeywordType =EKeywordTypeUtils.GetValue( EKeywordType.Appointment), FunctionID = appointmentID, AttributeName = attributeNameList[i], IsVisible = isVisibleList[i] };

                        if (string.IsNullOrEmpty(configExtendInfo.AttributeName))
                        {
                            FailMessage("保存失败，属性名称为必填项");
                            isItemReady = false;
                        }
                        if (string.IsNullOrEmpty(configExtendInfo.IsVisible))
                        {
                            FailMessage("保存失败，是否必填为显示项");
                            isItemReady = false;
                        }

                        if (configExtendInfo.IsVisible == "on")
                        {
                            configExtendInfo.IsVisible = "True";
                        }
                        else
                        {
                            configExtendInfo.IsVisible = "False";
                        }

                        configExtendInfoList.Add(configExtendInfo);
                    }

                    if (isItemReady)
                    {
                        DataProviderWX.ConfigExtendDAO.DeleteAllNotInIDList(PublishmentSystemId, appointmentID, itemIDList);

                        foreach (var configExtendInfo in configExtendInfoList)
                        {
                            if (configExtendInfo.ID > 0)
                            {
                                DataProviderWX.ConfigExtendDAO.Update(configExtendInfo);
                            }
                            else
                            {
                                DataProviderWX.ConfigExtendDAO.Insert(configExtendInfo);
                            }
                        }
                    }

                    if (isItemReady)
                    {
                        phStep4.Visible = true;
                        btnSubmit.Text = "确 认";
                    }
                    else
                    {
                        phStep3.Visible = true;
                    }

                }
                else if (selectedStep==4)
                {
                    var appointmentInfo = new AppointmentInfo();
                    appointmentInfo.PublishmentSystemID = PublishmentSystemId;

                    if (appointmentID > 0)
                    {
                        appointmentInfo = DataProviderWX.AppointmentDAO.GetAppointmentInfo(appointmentID);
                        DataProviderWX.KeywordDAO.Update(PublishmentSystemId, appointmentInfo.KeywordID, EKeywordType.Appointment, EMatchType.Exact, tbKeywords.Text, !cbIsEnabled.Checked);
                    }
                    else
                    {
                        var keywordInfo = new KeywordInfo();

                        keywordInfo.KeywordID = 0;
                        keywordInfo.PublishmentSystemID = PublishmentSystemId;
                        keywordInfo.Keywords = tbKeywords.Text;
                        keywordInfo.IsDisabled = !cbIsEnabled.Checked;
                        keywordInfo.KeywordType = EKeywordType.Appointment;
                        keywordInfo.MatchType = EMatchType.Exact;
                        keywordInfo.Reply = string.Empty;
                        keywordInfo.AddDate = DateTime.Now;
                        keywordInfo.Taxis = 0;
                        
                        appointmentInfo.KeywordID = DataProviderWX.KeywordDAO.Insert(keywordInfo);
                    }

                    appointmentInfo.StartDate = dtbStartDate.DateTime;
                    appointmentInfo.EndDate = dtbEndDate.DateTime;
                    appointmentInfo.Title = tbTitle.Text;
                    appointmentInfo.ImageUrl = imageUrl.Value;
                    appointmentInfo.ContentResultTopImageUrl = resultTopImageUrl.Value;
                    appointmentInfo.Summary = tbSummary.Text;
                    appointmentInfo.ContentIsSingle = true;
                    appointmentInfo.EndTitle = tbEndTitle.Text;
                    appointmentInfo.EndImageUrl = endImageUrl.Value;
                    appointmentInfo.EndSummary = tbEndSummary.Text;

                    appointmentInfo.IsFormRealName = cbIsFormRealName.Checked ? "True" : "False";
                    appointmentInfo.FormRealNameTitle = tbFormRealNameTitle.Text;
                    appointmentInfo.IsFormMobile = cbIsFormMobile.Checked ? "True" : "False";
                    appointmentInfo.FormMobileTitle = tbFormMobileTitle.Text;
                    appointmentInfo.IsFormEmail = cbIsFormEmail.Checked ? "True" : "False";
                    appointmentInfo.FormEmailTitle = tbFormEmailTitle.Text;

                    try
                    {
                        if (appointmentID > 0)
                        {
                            DataProviderWX.AppointmentDAO.Update(appointmentInfo);

                            Body.AddLog(PublishmentSystemId, "修改微预约", $"微预约:{tbTitle.Text}");
                            SuccessMessage("修改微预约成功！");
                        }
                        else
                        {
                            appointmentID = DataProviderWX.AppointmentDAO.Insert(appointmentInfo);
                            DataProviderWX.AppointmentItemDAO.UpdateAppointmentID(PublishmentSystemId, appointmentID);
                            DataProviderWX.ConfigExtendDAO.UpdateFuctionID(PublishmentSystemId, appointmentID);
                            Body.AddLog(PublishmentSystemId, "添加微预约", $"微预约:{tbTitle.Text}");
                            SuccessMessage("添加微预约成功！");
                        }

                        AddWaitAndRedirectScript(PageAppointment.GetRedirectUrl(PublishmentSystemId));
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "微预约设置失败！");
                    }

                    btnSubmit.Visible = false;
                    btnReturn.Visible = false;
                }
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
