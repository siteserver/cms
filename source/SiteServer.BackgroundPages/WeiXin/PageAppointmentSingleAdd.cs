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
        public Literal LtlPageTitle;

        public PlaceHolder PhStep1;
        public TextBox TbKeywords;
        public TextBox TbTitle;
        public TextBox TbSummary;
        public DateTimeTextBox DtbStartDate;
        public DateTimeTextBox DtbEndDate;
        public CheckBox CbIsEnabled;
        public Literal LtlImageUrl;

        public PlaceHolder PhStep2;
        public TextBox TbItemTitle;
        public CheckBox CbIsDescription;
        public TextBox TbDescriptionTitle;
        public TextBox TbDescription;
        public Literal LtlTopImageUrl;
        public Literal LtlResultTopImageUrl;
        public HtmlInputHidden TopImageUrl;
        public HtmlInputHidden ResultTopImageUrl;

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
        public Button BtnMapAddress;
        public Literal LtlMap;
        public PlaceHolder PhMap;
        
        public CheckBox CbIsTel;
        public TextBox TbTelTitle;
        public TextBox TbTel;
        public PlaceHolder PhTel;

        public PlaceHolder PhStep3;
        public Literal LtlAwardItems;
        public CheckBox CbIsFormRealName;
        public TextBox TbFormRealNameTitle;
        public CheckBox CbIsFormMobile;
        public TextBox TbFormMobileTitle;
        public CheckBox CbIsFormEmail;
        public TextBox TbFormEmailTitle;
        public CheckBox CbIsFormAddress;
        public TextBox TbFormAddressTitle;

        public PlaceHolder PhStep4;
        public TextBox TbEndTitle;
        public TextBox TbEndSummary;
        public Literal LtlEndImageUrl;

        public HtmlInputHidden ImageUrl;
        public HtmlInputHidden ContentImageUrl;
        public HtmlInputHidden EndImageUrl;

        public Button BtnSubmit;
        public Button BtnReturn;

        private int _appointmentId ;
        private int _appointmentItemId ;
        public static string GetRedirectUrl(int publishmentSystemId, int appointmentId, int appointmentItemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageAppointmentSingleAdd), new NameValueCollection
            {
                {"PublishmentSystemId", publishmentSystemId.ToString()},
                {"appointmentID", appointmentId.ToString()},
                {"appointmentItemID", appointmentItemId.ToString()}
            });
        }

        public string GetUploadUrl()
        {
            return string.Empty;
            //return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemId);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemId");
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
                var pageTitle = _appointmentId > 0 ? "编辑微预约" : "添加微预约";
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdAppointment, pageTitle, AppManager.WeiXin.Permission.WebSite.Appointment);
                LtlPageTitle.Text = pageTitle;
                  
                LtlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{AppointmentManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                LtlTopImageUrl.Text =
                    $@"<img id=""preview_topImageUrl"" src=""{AppointmentManager.GetItemTopImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                LtlResultTopImageUrl.Text =
                    $@"<img id=""preview_resultTopImageUrl"" src=""{AppointmentManager.GetContentResultTopImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                LtlContentImageUrl.Text =
                    $@"<img id=""preview_contentImageUrl"" src=""{AppointmentManager.GetContentImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                
                LtlEndImageUrl.Text =
                    $@"<img id=""preview_endImageUrl"" src=""{AppointmentManager.GetEndImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                 
                if (_appointmentId == 0)
                { 
                    DtbEndDate.DateTime = DateTime.Now.AddMonths(1);
                }
                else
                {
                    var appointmentInfo = DataProviderWx.AppointmentDao.GetAppointmentInfo(_appointmentId);
                    
                    if (appointmentInfo != null)
                    {
                        TbKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(appointmentInfo.KeywordId);
                        CbIsEnabled.Checked = !appointmentInfo.IsDisabled;
                        DtbStartDate.DateTime = appointmentInfo.StartDate;
                        DtbEndDate.DateTime = appointmentInfo.EndDate;
                        TbTitle.Text = appointmentInfo.Title;
                        if (!string.IsNullOrEmpty(appointmentInfo.ImageUrl))
                        {
                            LtlImageUrl.Text =
                                $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                    PublishmentSystemInfo, appointmentInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                        }
                        if (!string.IsNullOrEmpty(appointmentInfo.ContentResultTopImageUrl))
                        {
                            LtlResultTopImageUrl.Text =
                                $@"<img id=""preview_resultTopImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                    PublishmentSystemInfo, appointmentInfo.ContentResultTopImageUrl)}"" width=""370"" align=""middle"" />";
                        }

                        TbSummary.Text = appointmentInfo.Summary;

                        TbEndTitle.Text = appointmentInfo.EndTitle;
                        TbEndSummary.Text = appointmentInfo.EndSummary;
                        if (!string.IsNullOrEmpty(appointmentInfo.EndImageUrl))
                        {
                            LtlEndImageUrl.Text =
                                $@"<img id=""preview_endImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                    PublishmentSystemInfo, appointmentInfo.EndImageUrl)}"" width=""370"" align=""middle"" />";
                        }

                        ImageUrl.Value = appointmentInfo.ImageUrl;
                        ContentImageUrl.Value = appointmentInfo.ContentImageUrl;
                        ResultTopImageUrl.Value = appointmentInfo.ContentResultTopImageUrl;
                        EndImageUrl.Value = appointmentInfo.EndImageUrl;
                        #region 拓展属性
                        #region 姓名
                        if (appointmentInfo.IsFormRealName == "True")
                        {
                            CbIsFormRealName.Checked = true;
                            TbFormRealNameTitle.Text = appointmentInfo.FormRealNameTitle;
                        }
                        else if (string.IsNullOrEmpty(appointmentInfo.IsFormRealName))
                        {
                            CbIsFormRealName.Checked = true;
                            TbFormRealNameTitle.Text = "姓名";
                        }
                        else
                        {
                            CbIsFormRealName.Checked = false;
                            TbFormRealNameTitle.Text = appointmentInfo.FormRealNameTitle;
                        }
                        #endregion
                        #region 电话
                        if (appointmentInfo.IsFormMobile == "True")
                        {
                            CbIsFormMobile.Checked = true;
                            TbFormMobileTitle.Text = appointmentInfo.FormMobileTitle;
                        }
                        else if (string.IsNullOrEmpty(appointmentInfo.IsFormMobile))
                        {
                            CbIsFormMobile.Checked = true;
                            TbFormMobileTitle.Text = "电话";
                        }
                        else
                        {
                            CbIsFormMobile.Checked = false;
                            TbFormMobileTitle.Text = appointmentInfo.FormMobileTitle;
                        }
                        #endregion
                        #region 邮箱
                        if (appointmentInfo.IsFormEmail == "True")
                        {
                            CbIsFormEmail.Checked = true;
                            TbFormEmailTitle.Text = appointmentInfo.FormEmailTitle;
                        }
                        else if (string.IsNullOrEmpty(appointmentInfo.IsFormEmail))
                        {
                            CbIsFormEmail.Checked = true;
                            TbFormEmailTitle.Text = "电话";
                        }
                        else
                        {
                            CbIsFormEmail.Checked = false;
                            TbFormEmailTitle.Text = appointmentInfo.FormEmailTitle;
                        }
                        #endregion

                        _appointmentItemId = DataProviderWx.AppointmentItemDao.GetItemId(PublishmentSystemId, _appointmentId);

                        var configExtendInfoList = DataProviderWx.ConfigExtendDao.GetConfigExtendInfoList(PublishmentSystemId, appointmentInfo.Id, EKeywordTypeUtils.GetValue(EKeywordType.Appointment));
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
                            itemBuilder.AppendFormat(@"{{id: '{0}', attributeName: '{1}',isVisible:'{2}'}},", configExtendInfo.Id, configExtendInfo.AttributeName, configExtendInfo.IsVisible);
                        }
                        if (itemBuilder.Length > 0) itemBuilder.Length--;
                        LtlAwardItems.Text =
                            $@"itemController.itemCount = {configExtendInfoList.Count};itemController.items = [{itemBuilder}];";
                        #endregion
                    }
                }

                if (_appointmentItemId > 0)
                {
                    var appointmentItemInfo = DataProviderWx.AppointmentItemDao.GetItemInfo(_appointmentItemId);
                    if (appointmentItemInfo != null)
                    {
                        TbItemTitle.Text = appointmentItemInfo.Title;
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
                                $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
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
               
                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageAppointment.GetRedirectUrl(PublishmentSystemId)}"";return false");
			}
		}

		public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                var selectedStep = 0;
                if (PhStep1.Visible)
                {
                    selectedStep = 1;
                }
                else if (PhStep2.Visible)
                {
                    selectedStep = 2;
                }
                else if (PhStep3.Visible)
                {
                    selectedStep = 3;
                }
                else if (PhStep4.Visible)
                {
                    selectedStep = 4;
                }

			    PhStep1.Visible = PhStep2.Visible = PhStep3.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(TbKeywords.Text))
                    {
                        if (_appointmentId > 0)
                        {
                            var appointmentInfo = DataProviderWx.AppointmentDao.GetAppointmentInfo(_appointmentId);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, appointmentInfo.KeywordId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
                        }
                        else
                        {
                            isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
                        }
                    }
                    
                    if (isConflict)
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                    }
                    else
                    {
                        PhStep2.Visible = true;
                    }
                }
                else if (selectedStep == 2)
                {
                    var isItemReady = true;
                    var appointmentItemInfo = new AppointmentItemInfo();
                    appointmentItemInfo.PublishmentSystemId = PublishmentSystemId;
                    if (_appointmentItemId > 0)
                    {
                        appointmentItemInfo = DataProviderWx.AppointmentItemDao.GetItemInfo(_appointmentItemId);
                    }

                    appointmentItemInfo.AppointmentId = _appointmentId;
                    appointmentItemInfo.Title =PageUtils.FilterXss(TbItemTitle.Text);
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

                    try
                    {
                        if (_appointmentItemId > 0)
                        {
                            DataProviderWx.AppointmentItemDao.Update(appointmentItemInfo);
                            Body.AddSiteLog(PublishmentSystemId, "修改预约项目", $"预约项目:{TbTitle.Text}");
                        }
                        else
                        {
                            _appointmentItemId = DataProviderWx.AppointmentItemDao.Insert(appointmentItemInfo);

                            Body.AddSiteLog(PublishmentSystemId, "新增预约项目", $"预约项目:{TbTitle.Text}");
                        }
                    }
                    catch (Exception ex)
                    {
                        isItemReady = false;
                        FailMessage(ex, "微预约项目设置失败！");
                    }

                    if (isItemReady)
                    {
                        PhStep3.Visible = true;
                    }
                    else
                    {
                        PhStep2.Visible = true;
                    }
                }
                else if (selectedStep == 3)
                {
                    var isItemReady = true;
                    var itemCount = TranslateUtils.ToInt(Request.Form["itemCount"]);

                    var itemIdList = TranslateUtils.StringCollectionToIntList(Request.Form["itemID"]);
                    var attributeNameList = TranslateUtils.StringCollectionToStringList(Request.Form["itemAttributeName"]);

                    var itemIsVisible = "off";
                    if (!string.IsNullOrEmpty(Request.Form["itemIsVisible"]))
                    {
                        itemIsVisible = Request.Form["itemIsVisible"];
                    }

                    var isVisibleList = TranslateUtils.StringCollectionToStringList(itemIsVisible);

                    if (isVisibleList.Count < itemIdList.Count)
                    {
                        for (var i = isVisibleList.Count; i < itemIdList.Count; i++)
                        {
                            isVisibleList.Add("off");
                        }
                    }

                    var configExtendInfoList = new List<ConfigExtendInfo>();
                    for (var i = 0; i < itemCount; i++)
                    {
                        var configExtendInfo = new ConfigExtendInfo { Id = itemIdList[i], PublishmentSystemId = PublishmentSystemId, KeywordType =EKeywordTypeUtils.GetValue( EKeywordType.Appointment), FunctionId = _appointmentId, AttributeName = attributeNameList[i], IsVisible = isVisibleList[i] };

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
                        DataProviderWx.ConfigExtendDao.DeleteAllNotInIdList(PublishmentSystemId, _appointmentId, itemIdList);

                        foreach (var configExtendInfo in configExtendInfoList)
                        {
                            if (configExtendInfo.Id > 0)
                            {
                                DataProviderWx.ConfigExtendDao.Update(configExtendInfo);
                            }
                            else
                            {
                                DataProviderWx.ConfigExtendDao.Insert(configExtendInfo);
                            }
                        }
                    }

                    if (isItemReady)
                    {
                        PhStep4.Visible = true;
                        BtnSubmit.Text = "确 认";
                    }
                    else
                    {
                        PhStep3.Visible = true;
                    }

                }
                else if (selectedStep==4)
                {
                    var appointmentInfo = new AppointmentInfo();
                    appointmentInfo.PublishmentSystemId = PublishmentSystemId;

                    if (_appointmentId > 0)
                    {
                        appointmentInfo = DataProviderWx.AppointmentDao.GetAppointmentInfo(_appointmentId);
                        DataProviderWx.KeywordDao.Update(PublishmentSystemId, appointmentInfo.KeywordId, EKeywordType.Appointment, EMatchType.Exact, TbKeywords.Text, !CbIsEnabled.Checked);
                    }
                    else
                    {
                        var keywordInfo = new KeywordInfo();

                        keywordInfo.KeywordId = 0;
                        keywordInfo.PublishmentSystemId = PublishmentSystemId;
                        keywordInfo.Keywords = TbKeywords.Text;
                        keywordInfo.IsDisabled = !CbIsEnabled.Checked;
                        keywordInfo.KeywordType = EKeywordType.Appointment;
                        keywordInfo.MatchType = EMatchType.Exact;
                        keywordInfo.Reply = string.Empty;
                        keywordInfo.AddDate = DateTime.Now;
                        keywordInfo.Taxis = 0;
                        
                        appointmentInfo.KeywordId = DataProviderWx.KeywordDao.Insert(keywordInfo);
                    }

                    appointmentInfo.StartDate = DtbStartDate.DateTime;
                    appointmentInfo.EndDate = DtbEndDate.DateTime;
                    appointmentInfo.Title = TbTitle.Text;
                    appointmentInfo.ImageUrl = ImageUrl.Value;
                    appointmentInfo.ContentResultTopImageUrl = ResultTopImageUrl.Value;
                    appointmentInfo.Summary = TbSummary.Text;
                    appointmentInfo.ContentIsSingle = true;
                    appointmentInfo.EndTitle = TbEndTitle.Text;
                    appointmentInfo.EndImageUrl = EndImageUrl.Value;
                    appointmentInfo.EndSummary = TbEndSummary.Text;

                    appointmentInfo.IsFormRealName = CbIsFormRealName.Checked ? "True" : "False";
                    appointmentInfo.FormRealNameTitle = TbFormRealNameTitle.Text;
                    appointmentInfo.IsFormMobile = CbIsFormMobile.Checked ? "True" : "False";
                    appointmentInfo.FormMobileTitle = TbFormMobileTitle.Text;
                    appointmentInfo.IsFormEmail = CbIsFormEmail.Checked ? "True" : "False";
                    appointmentInfo.FormEmailTitle = TbFormEmailTitle.Text;

                    try
                    {
                        if (_appointmentId > 0)
                        {
                            DataProviderWx.AppointmentDao.Update(appointmentInfo);

                            Body.AddSiteLog(PublishmentSystemId, "修改微预约", $"微预约:{TbTitle.Text}");
                            SuccessMessage("修改微预约成功！");
                        }
                        else
                        {
                            _appointmentId = DataProviderWx.AppointmentDao.Insert(appointmentInfo);
                            DataProviderWx.AppointmentItemDao.UpdateAppointmentId(PublishmentSystemId, _appointmentId);
                            DataProviderWx.ConfigExtendDao.UpdateFuctionId(PublishmentSystemId, _appointmentId);
                            Body.AddSiteLog(PublishmentSystemId, "添加微预约", $"微预约:{TbTitle.Text}");
                            SuccessMessage("添加微预约成功！");
                        }

                        AddWaitAndRedirectScript(PageAppointment.GetRedirectUrl(PublishmentSystemId));
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "微预约设置失败！");
                    }

                    BtnSubmit.Visible = false;
                    BtnReturn.Visible = false;
                }
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
