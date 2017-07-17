using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;
using System.Collections.Specialized;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageAppointmentMultipleAdd : BasePageCms
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
        public TextBox TbContentDescription;
        public Literal LtlContentImageUrl;
        public Literal LtlContentResultTopImageUrl;
         
        public PlaceHolder PhStep3;
        public Repeater RptContents;
        public SqlPager SpContents;
        public Button BtnAdd;
        //public Button btnDelete;

        public PlaceHolder PhStep4;
        public Literal LtlAwardItems;
        public CheckBox CbIsFormRealName;
        public TextBox TbFormRealNameTitle;
        public CheckBox CbIsFormMobile;
        public TextBox TbFormMobileTitle;
        public CheckBox CbIsFormEmail;
        public TextBox TbFormEmailTitle;
        public CheckBox CbIsFormAddress;
        public TextBox TbFormAddressTitle;

        public PlaceHolder PhStep5;
        public TextBox TbEndTitle;
        public TextBox TbEndSummary;
        public Literal LtlEndImageUrl;

        public HtmlInputHidden ImageUrl;
        public HtmlInputHidden ContentImageUrl;
        public HtmlInputHidden ContentResultTopImageUrl;
        public HtmlInputHidden EndImageUrl;

        public Button BtnSubmit;
        public Button BtnReturn;

        private int _appointmentId = 0;
        private int _appointmentItemId;

        public static string GetRedirectUrl(int publishmentSystemId, int appointmentId, int appointmentItemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageAppointmentMultipleAdd), new NameValueCollection
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

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            SpContents.SelectCommand = DataProviderWx.AppointmentItemDao.GetSelectString(PublishmentSystemId, _appointmentId);
            SpContents.SortField = AlbumAttribute.Id;
            SpContents.SortMode = SortMode.ASC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

			if (!IsPostBack)
            {
                 
                var pageTitle = _appointmentId > 0 ? "编辑微预约" : "添加微预约";
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdAppointment, pageTitle, AppManager.WeiXin.Permission.WebSite.Appointment);
 
                LtlPageTitle.Text = pageTitle;
                  
                LtlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{AppointmentManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                LtlContentImageUrl.Text =
                    $@"<img id=""preview_contentImageUrl"" src=""{AppointmentManager.GetContentImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                LtlContentResultTopImageUrl.Text =
                    $@"<img id=""preview_contentResultTopImageUrl"" src=""{AppointmentManager.GetContentResultTopImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                LtlEndImageUrl.Text =
                    $@"<img id=""preview_endImageUrl"" src=""{AppointmentManager.GetEndImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";

                SpContents.DataBind();
                 
                BtnAdd.Attributes.Add("onclick", ModalAppointmentItemAdd.GetOpenWindowStringToAdd(PublishmentSystemId, _appointmentId, 0));

                //string urlDelete = PageUtils.AddQueryString(BackgroundAppointmentMultipleAdd.GetRedirectUrl(base.PublishmentSystemId, this.appointmentID,this.tbTitle.Text), "Delete", "True");
                //this.btnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的微预约项目", "此操作将删除所选微预约项目，确认吗？"));
               
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
                        TbSummary.Text = appointmentInfo.Summary;
                        if (!string.IsNullOrEmpty(appointmentInfo.ContentImageUrl))
                        {
                            LtlContentImageUrl.Text =
                                $@"<img id=""preview_contentImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                    PublishmentSystemInfo, appointmentInfo.ContentImageUrl)}"" width=""370"" align=""middle"" />";
                        }
                        if (!string.IsNullOrEmpty(appointmentInfo.ContentResultTopImageUrl))
                        {
                            LtlContentResultTopImageUrl.Text =
                                $@"<img id=""preview_contentResultTopImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                    PublishmentSystemInfo, appointmentInfo.ContentResultTopImageUrl)}"" width=""370"" align=""middle"" />";
                        }

                        TbContentDescription.Text = appointmentInfo.ContentDescription;

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
                        ContentResultTopImageUrl.Value = appointmentInfo.ContentResultTopImageUrl;
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
                            TbFormEmailTitle.Text = "邮箱";
                        }
                        else
                        {
                            CbIsFormEmail.Checked = false;
                            TbFormEmailTitle.Text = appointmentInfo.FormEmailTitle;
                        }
                        #endregion

                        _appointmentItemId = DataProviderWx.AppointmentItemDao.GetItemId(PublishmentSystemId, _appointmentId);

                        var configExtendInfoList = DataProviderWx.ConfigExtendDao.GetConfigExtendInfoList(PublishmentSystemId, appointmentInfo.Id, EKeywordTypeUtils.GetValue(EKeywordType.Appointment));
                        var itemBuilder = new StringBuilder();
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
                else if (PhStep5.Visible)
                {
                    selectedStep = 5;
                }

			    PhStep1.Visible = false;
			    PhStep2.Visible = false;
			    PhStep3.Visible = false;
			    PhStep4.Visible = false;
                PhStep5.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(TbKeywords.Text))
                    {
                        if (_appointmentId > 0)
                        {
                            var appointmentInfo = DataProviderWx.AppointmentDao.GetAppointmentInfo(_appointmentId);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, appointmentInfo.KeywordId, TbKeywords.Text, out conflictKeywords);
                        }
                        else
                        {
                            isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemId, TbKeywords.Text, out conflictKeywords);
                        }
                    }
                    
                    if (isConflict)
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                        PhStep1.Visible = true;
                    }
                    else
                    {
                        PhStep2.Visible = true;
                    }
                }
                else if (selectedStep == 2)
                {
                    PhStep3.Visible = true;
                }
                else if (selectedStep == 3)
                {
                    PhStep4.Visible = true;
                }
                else if (selectedStep == 4)
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
                        var configExtendInfo = new ConfigExtendInfo { Id = itemIdList[i], PublishmentSystemId = PublishmentSystemId, KeywordType = EKeywordTypeUtils.GetValue(EKeywordType.Appointment), FunctionId = _appointmentId, AttributeName = attributeNameList[i], IsVisible = isVisibleList[i] };

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
                        PhStep5.Visible = true;
                        BtnSubmit.Text = "确 认";
                    }
                    else
                    {
                        PhStep4.Visible = true;
                    }
                }
                else if (selectedStep == 5)
                {
                    var appointmentInfo = new AppointmentInfo();
                    appointmentInfo.PublishmentSystemId = PublishmentSystemId;

                    if (_appointmentId > 0)
                    {
                        appointmentInfo = DataProviderWx.AppointmentDao.GetAppointmentInfo(_appointmentId);
                        DataProviderWx.KeywordDao.Update(PublishmentSystemId, appointmentInfo.KeywordId,
                            EKeywordType.Appointment, EMatchType.Exact, TbKeywords.Text, !CbIsEnabled.Checked);
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
                    ;
                    appointmentInfo.Summary = TbSummary.Text;

                    appointmentInfo.ContentImageUrl = ContentImageUrl.Value;
                    appointmentInfo.ContentDescription = TbContentDescription.Text;
                    appointmentInfo.ContentResultTopImageUrl = ContentResultTopImageUrl.Value;
                    appointmentInfo.ContentIsSingle = false;

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

                            DataProviderWx.AppointmentItemDao.UpdateAppointmentId(PublishmentSystemId,
                                _appointmentId);

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
 
        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var appointmentItemInfo = new AppointmentItemInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlMapAddress = e.Item.FindControl("ltlMapAddress") as Literal;
                var ltlTel = e.Item.FindControl("ltlTel") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = appointmentItemInfo.Title;
                ltlMapAddress.Text = appointmentItemInfo.MapAddress;
                ltlTel.Text = appointmentItemInfo.Tel;

                var urlEdit = ModalAppointmentItemAdd.GetOpenWindowStringToEdit(PublishmentSystemId, _appointmentId, appointmentItemInfo.Id); 
                ltlEditUrl.Text = $@"<a href=""javascript:;"" onclick=""{urlEdit}"">编辑</a>";
            }
        }

        public string GetIdCollection()
        {
            return Request.QueryString["IDCollection"];
        }
         
	}
}
