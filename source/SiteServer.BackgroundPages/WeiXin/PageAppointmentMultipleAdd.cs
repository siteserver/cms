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
        public TextBox tbContentDescription;
        public Literal ltlContentImageUrl;
        public Literal ltlContentResultTopImageUrl;
         
        public PlaceHolder phStep3;
        public Repeater rptContents;
        public SqlPager spContents;
        public Button btnAdd;
        //public Button btnDelete;

        public PlaceHolder phStep4;
        public Literal ltlAwardItems;
        public CheckBox cbIsFormRealName;
        public TextBox tbFormRealNameTitle;
        public CheckBox cbIsFormMobile;
        public TextBox tbFormMobileTitle;
        public CheckBox cbIsFormEmail;
        public TextBox tbFormEmailTitle;
        public CheckBox cbIsFormAddress;
        public TextBox tbFormAddressTitle;

        public PlaceHolder phStep5;
        public TextBox tbEndTitle;
        public TextBox tbEndSummary;
        public Literal ltlEndImageUrl;

        public HtmlInputHidden imageUrl;
        public HtmlInputHidden contentImageUrl;
        public HtmlInputHidden contentResultTopImageUrl;
        public HtmlInputHidden endImageUrl;

        public Button btnSubmit;
        public Button btnReturn;

        private int appointmentID = 0;
        private int appointmentItemID;

        public static string GetRedirectUrl(int publishmentSystemId, int appointmentID, int appointmentItemID)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageAppointmentMultipleAdd), new NameValueCollection
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

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 30;
            spContents.SelectCommand = DataProviderWX.AppointmentItemDAO.GetSelectString(PublishmentSystemId, appointmentID);
            spContents.SortField = AlbumAttribute.ID;
            spContents.SortMode = SortMode.ASC;
            rptContents.ItemDataBound += rptContents_ItemDataBound;

			if (!IsPostBack)
            {
                 
                var pageTitle = appointmentID > 0 ? "编辑微预约" : "添加微预约";
                BreadCrumb(AppManager.WeiXin.LeftMenu.IdFunction, AppManager.WeiXin.LeftMenu.Function.IdAppointment, pageTitle, AppManager.WeiXin.Permission.WebSite.Appointment);
 
                ltlPageTitle.Text = pageTitle;
                  
                ltlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{AppointmentManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                ltlContentImageUrl.Text =
                    $@"<img id=""preview_contentImageUrl"" src=""{AppointmentManager.GetContentImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                ltlContentResultTopImageUrl.Text =
                    $@"<img id=""preview_contentResultTopImageUrl"" src=""{AppointmentManager.GetContentResultTopImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                ltlEndImageUrl.Text =
                    $@"<img id=""preview_endImageUrl"" src=""{AppointmentManager.GetEndImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";

                spContents.DataBind();
                 
                btnAdd.Attributes.Add("onclick", ModalAppointmentItemAdd.GetOpenWindowStringToAdd(PublishmentSystemId, appointmentID, 0));

                //string urlDelete = PageUtils.AddQueryString(BackgroundAppointmentMultipleAdd.GetRedirectUrl(base.PublishmentSystemID, this.appointmentID,this.tbTitle.Text), "Delete", "True");
                //this.btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的微预约项目", "此操作将删除所选微预约项目，确认吗？"));
               
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
                        tbSummary.Text = appointmentInfo.Summary;
                        if (!string.IsNullOrEmpty(appointmentInfo.ContentImageUrl))
                        {
                            ltlContentImageUrl.Text =
                                $@"<img id=""preview_contentImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                    PublishmentSystemInfo, appointmentInfo.ContentImageUrl)}"" width=""370"" align=""middle"" />";
                        }
                        if (!string.IsNullOrEmpty(appointmentInfo.ContentResultTopImageUrl))
                        {
                            ltlContentResultTopImageUrl.Text =
                                $@"<img id=""preview_contentResultTopImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                    PublishmentSystemInfo, appointmentInfo.ContentResultTopImageUrl)}"" width=""370"" align=""middle"" />";
                        }

                        tbContentDescription.Text = appointmentInfo.ContentDescription;

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
                        contentResultTopImageUrl.Value = appointmentInfo.ContentResultTopImageUrl;
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
                            tbFormEmailTitle.Text = "邮箱";
                        }
                        else
                        {
                            cbIsFormEmail.Checked = false;
                            tbFormEmailTitle.Text = appointmentInfo.FormEmailTitle;
                        }
                        #endregion

                        appointmentItemID = DataProviderWX.AppointmentItemDAO.GetItemID(PublishmentSystemId, appointmentID);

                        var configExtendInfoList = DataProviderWX.ConfigExtendDAO.GetConfigExtendInfoList(PublishmentSystemId, appointmentInfo.ID, EKeywordTypeUtils.GetValue(EKeywordType.Appointment));
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
                            itemBuilder.AppendFormat(@"{{id: '{0}', attributeName: '{1}',isVisible:'{2}'}},", configExtendInfo.ID, configExtendInfo.AttributeName, configExtendInfo.IsVisible);
                        }
                        if (itemBuilder.Length > 0) itemBuilder.Length--;
                        ltlAwardItems.Text =
                            $@"itemController.itemCount = {configExtendInfoList.Count};itemController.items = [{itemBuilder
                                .ToString()}];";
                        #endregion
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
                else if (phStep5.Visible)
                {
                    selectedStep = 5;
                }

			    phStep1.Visible = false;
			    phStep2.Visible = false;
			    phStep3.Visible = false;
			    phStep4.Visible = false;
                phStep5.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(tbKeywords.Text))
                    {
                        if (appointmentID > 0)
                        {
                            var appointmentInfo = DataProviderWX.AppointmentDAO.GetAppointmentInfo(appointmentID);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, appointmentInfo.KeywordID, tbKeywords.Text, out conflictKeywords);
                        }
                        else
                        {
                            isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemId, tbKeywords.Text, out conflictKeywords);
                        }
                    }
                    
                    if (isConflict)
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                        phStep1.Visible = true;
                    }
                    else
                    {
                        phStep2.Visible = true;
                    }
                }
                else if (selectedStep == 2)
                {
                    phStep3.Visible = true;
                }
                else if (selectedStep == 3)
                {
                    phStep4.Visible = true;
                }
                else if (selectedStep == 4)
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
                        var configExtendInfo = new ConfigExtendInfo { ID = itemIDList[i], PublishmentSystemID = PublishmentSystemId, KeywordType = EKeywordTypeUtils.GetValue(EKeywordType.Appointment), FunctionID = appointmentID, AttributeName = attributeNameList[i], IsVisible = isVisibleList[i] };

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
                        phStep5.Visible = true;
                        btnSubmit.Text = "确 认";
                    }
                    else
                    {
                        phStep4.Visible = true;
                    }
                }
                else if (selectedStep == 5)
                {
                    var appointmentInfo = new AppointmentInfo();
                    appointmentInfo.PublishmentSystemID = PublishmentSystemId;

                    if (appointmentID > 0)
                    {
                        appointmentInfo = DataProviderWX.AppointmentDAO.GetAppointmentInfo(appointmentID);
                        DataProviderWX.KeywordDAO.Update(PublishmentSystemId, appointmentInfo.KeywordID,
                            EKeywordType.Appointment, EMatchType.Exact, tbKeywords.Text, !cbIsEnabled.Checked);
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
                    ;
                    appointmentInfo.Summary = tbSummary.Text;

                    appointmentInfo.ContentImageUrl = contentImageUrl.Value;
                    appointmentInfo.ContentDescription = tbContentDescription.Text;
                    appointmentInfo.ContentResultTopImageUrl = contentResultTopImageUrl.Value;
                    appointmentInfo.ContentIsSingle = false;

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

                            DataProviderWX.AppointmentItemDAO.UpdateAppointmentID(PublishmentSystemId,
                                appointmentID);

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

                var urlEdit = ModalAppointmentItemAdd.GetOpenWindowStringToEdit(PublishmentSystemId, appointmentID, appointmentItemInfo.ID); 
                ltlEditUrl.Text = $@"<a href=""javascript:;"" onclick=""{urlEdit}"">编辑</a>";
            }
        }

        public string GetIDCollection()
        {
            return Request.QueryString["IDCollection"];
        }
         
	}
}
