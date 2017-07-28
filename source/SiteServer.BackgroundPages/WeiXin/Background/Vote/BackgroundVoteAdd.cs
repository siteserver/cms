using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using BaiRong.Controls;
using System.Collections.Generic;
using System.Text;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.WeiXin.BackgroundPages
{
	public class BackgroundVoteAdd : BackgroundBasePageWX
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
        public DropDownList ddlContentIsImageOption;
        public DropDownList ddlContentIsCheckBox;
        public Literal ltlContentImageUrl;
        public Literal ltlVoteItems;

        public PlaceHolder phStep3;
        public TextBox tbEndTitle;
        public TextBox tbEndSummary;
        public Literal ltlEndImageUrl;

        public HtmlInputHidden imageUrl;
        public HtmlInputHidden contentImageUrl;
        public HtmlInputHidden endImageUrl;

        public Button btnSubmit;
        public Button btnReturn;

        private int voteID;

        public static string GetRedirectUrl(int publishmentSystemID, int voteID)
        {
            return PageUtils.GetWXUrl(
                $"background_voteAdd.aspx?publishmentSystemID={publishmentSystemID}&voteID={voteID}");
        }

        public string GetUploadUrl()
        {
            return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            voteID = TranslateUtils.ToInt(GetQueryString("voteID"));

			if (!IsPostBack)
            {
                var pageTitle = voteID > 0 ? "编辑投票活动" : "添加投票活动";
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Vote, pageTitle, AppManager.WeiXin.Permission.WebSite.Vote);
                ltlPageTitle.Text = pageTitle;

                var listItem = new ListItem("文字类型投票", "false");
                ddlContentIsImageOption.Items.Add(listItem);
                listItem = new ListItem("图文类型投票", "true");
                ddlContentIsImageOption.Items.Add(listItem);

                ddlContentIsImageOption.Attributes.Add("onchange", "itemController.isImageOptionChange(this)");
                EBooleanUtils.AddListItems(ddlContentIsCheckBox, "多选", "单选");
                ControlUtils.SelectListItems(ddlContentIsCheckBox, false.ToString());

                ltlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{VoteManager.GetImageUrl(PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                ltlContentImageUrl.Text =
                    $@"<img id=""preview_contentImageUrl"" src=""{VoteManager.GetContentImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                ltlEndImageUrl.Text =
                    $@"<img id=""preview_endImageUrl"" src=""{VoteManager.GetEndImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";

                var selectImageClick = CMS.BackgroundPages.Modal.SelectImage.GetOpenWindowString(PublishmentSystemInfo, "itemImageUrl_");
                var uploadImageClick = CMS.BackgroundPages.Modal.UploadImageSingle.GetOpenWindowStringToTextBox(PublishmentSystemID, "itemImageUrl_");
                var cuttingImageClick = CMS.BackgroundPages.Modal.CuttingImage.GetOpenWindowStringWithTextBox(PublishmentSystemID, "itemImageUrl_");
                var previewImageClick = CMS.BackgroundPages.Modal.Message.GetOpenWindowStringToPreviewImage(PublishmentSystemID, "itemImageUrl_");
                ltlVoteItems.Text =
                    $@"itemController.selectImageClickString = ""{selectImageClick}"";itemController.uploadImageClickString = ""{uploadImageClick}"";itemController.cuttingImageClickString = ""{cuttingImageClick}"";itemController.previewImageClickString = ""{previewImageClick}"";";

                if (voteID == 0)
                {
                    ltlVoteItems.Text += "itemController.isImageOption = false;itemController.itemCount = 2;itemController.items = [{}, {}];";
                    dtbEndDate.DateTime = DateTime.Now.AddMonths(1);
                }
                else
                {
                    var voteInfo = DataProviderWX.VoteDAO.GetVoteInfo(voteID);

                    tbKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(voteInfo.KeywordID);
                    cbIsEnabled.Checked = !voteInfo.IsDisabled;
                    dtbStartDate.DateTime = voteInfo.StartDate;
                    dtbEndDate.DateTime = voteInfo.EndDate;
                    tbTitle.Text = voteInfo.Title;
                    if (!string.IsNullOrEmpty(voteInfo.ImageUrl))
                    {
                        ltlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, voteInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    tbSummary.Text = voteInfo.Summary;
                    if (!string.IsNullOrEmpty(voteInfo.ContentImageUrl))
                    {
                        ltlContentImageUrl.Text =
                            $@"<img id=""preview_contentImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, voteInfo.ContentImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    tbContentDescription.Text = voteInfo.ContentDescription;
                    //ControlUtils.SelectListItems(this.ddlContentResultVisible, voteInfo.ContentResultVisible);
                    ControlUtils.SelectListItems(ddlContentIsImageOption, voteInfo.ContentIsImageOption.ToString().ToLower());
                    ControlUtils.SelectListItems(ddlContentIsCheckBox, voteInfo.ContentIsCheckBox.ToString());

                    var voteItemInfoList = DataProviderWX.VoteItemDAO.GetVoteItemInfoList(voteID);
                    var itemBuilder = new StringBuilder();
                    foreach (var itemInfo in voteItemInfoList)
                    {
                        itemBuilder.AppendFormat(@"{{id: '{0}', title: '{1}', imageUrl: '{2}', navigationUrl: '{3}', voteNum: '{4}'}},", itemInfo.ID, itemInfo.Title, itemInfo.ImageUrl, itemInfo.NavigationUrl, itemInfo.VoteNum);
                    }
                    if (itemBuilder.Length > 0) itemBuilder.Length--;

                    ltlVoteItems.Text += $@"
itemController.isImageOption = {voteInfo.ContentIsImageOption.ToString().ToLower()};itemController.itemCount = {voteItemInfoList
                        .Count};itemController.items = [{itemBuilder.ToString()}];";

                    tbEndTitle.Text = voteInfo.EndTitle;
                    tbEndSummary.Text = voteInfo.EndSummary;
                    if (!string.IsNullOrEmpty(voteInfo.EndImageUrl))
                    {
                        ltlEndImageUrl.Text =
                            $@"<img id=""preview_endImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, voteInfo.EndImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    imageUrl.Value = voteInfo.ImageUrl;
                    contentImageUrl.Value = voteInfo.ContentImageUrl;
                    endImageUrl.Value = voteInfo.EndImageUrl;
                }

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundVote.GetRedirectUrl(PublishmentSystemID)}"";return false");
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

                phStep1.Visible = phStep2.Visible = phStep3.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(tbKeywords.Text))
                    {
                        if (voteID > 0)
                        {
                            var voteInfo = DataProviderWX.VoteDAO.GetVoteInfo(voteID);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemID, voteInfo.KeywordID, PageUtils.FilterXSS(tbKeywords.Text), out conflictKeywords);
                        }
                        else
                        {
                            isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemID, PageUtils.FilterXSS(tbKeywords.Text), out conflictKeywords);
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
                    var isItemReady = true;
                    var itemCount = TranslateUtils.ToInt(Request.Form["itemCount"]);

                    if (itemCount < 2)
                    {
                        FailMessage("投票保存失败，至少设置两个投票项");
                        isItemReady = false;
                    }
                    else
                    {
                        var isImageOption = TranslateUtils.ToBool(ddlContentIsImageOption.SelectedValue);

                        var itemIDList = TranslateUtils.StringCollectionToIntList(Request.Form["itemID"]);
                        var titleList = TranslateUtils.StringCollectionToStringList(Request.Form["itemTitle"]);
                        var imageUrlList = TranslateUtils.StringCollectionToStringList(Request.Form["itemImageUrl"]);
                        var navigationUrlList = TranslateUtils.StringCollectionToStringList(Request.Form["itemNavigationUrl"]);
                        var voteNumList = TranslateUtils.StringCollectionToIntList(Request.Form["itemVoteNum"]);
                        var voteItemInfoList = new List<VoteItemInfo>();
                        for (var i = 0; i < itemCount; i++)
                        {
                            var imageUrl = string.Empty;
                            if (isImageOption)
                            {
                                imageUrl = imageUrlList[i];
                            }
                            var itemInfo = new VoteItemInfo { ID = itemIDList[i], VoteID = voteID, PublishmentSystemID = PublishmentSystemID, Title = titleList[i], ImageUrl = imageUrl, NavigationUrl = navigationUrlList[i], VoteNum = voteNumList[i] };

                            if (isImageOption && string.IsNullOrEmpty(itemInfo.ImageUrl))
                            {
                                FailMessage("投票保存失败，图片地址为必填项");
                                isItemReady = false;
                            }
                            if (string.IsNullOrEmpty(itemInfo.Title))
                            {
                                FailMessage("投票保存失败，选项标题为必填项");
                                isItemReady = false;
                            }

                            voteItemInfoList.Add(itemInfo);
                        }

                        if (isItemReady)
                        {
                            //DataProviderWX.VoteItemDAO.DeleteAll(base.PublishmentSystemID, this.voteID);
                            
                            foreach (var itemInfo in voteItemInfoList)
                            {
                               var newVoteItemInfo=DataProviderWX.VoteItemDAO.GetVoteItemInfo(itemInfo.ID);
                               if (itemInfo.ID>0)
                                {
                                    itemInfo.VoteNum = newVoteItemInfo.VoteNum;
                                    DataProviderWX.VoteItemDAO.Update(itemInfo);
                                }
                                else
                                {
                                    DataProviderWX.VoteItemDAO.Insert(itemInfo);
                                }
                            }
                        }
                    }

                    if (isItemReady)
                    {
                        phStep3.Visible = true;
                        btnSubmit.Text = "确 认";
                    }
                    else
                    {
                        phStep2.Visible = true;
                    }
                }
                else if (selectedStep == 3)
                {
                    var voteInfo = new VoteInfo();
                    if (voteID > 0)
                    {
                        voteInfo = DataProviderWX.VoteDAO.GetVoteInfo(voteID);
                    }
                    voteInfo.PublishmentSystemID = PublishmentSystemID;

                    voteInfo.KeywordID = DataProviderWX.KeywordDAO.GetKeywordID(PublishmentSystemID, voteID > 0, tbKeywords.Text, EKeywordType.Vote, voteInfo.KeywordID);
                    voteInfo.IsDisabled = !cbIsEnabled.Checked;

                    voteInfo.StartDate = dtbStartDate.DateTime;
                    voteInfo.EndDate = dtbEndDate.DateTime;
                    voteInfo.Title = PageUtils.FilterXSS(tbTitle.Text);
                    voteInfo.ImageUrl = imageUrl.Value; ;
                    voteInfo.Summary = tbSummary.Text;

                    voteInfo.ContentImageUrl = contentImageUrl.Value;
                    voteInfo.ContentDescription = tbContentDescription.Text;
                    voteInfo.ContentIsImageOption = TranslateUtils.ToBool(ddlContentIsImageOption.SelectedValue).ToString();
                    voteInfo.ContentIsCheckBox = TranslateUtils.ToBool(ddlContentIsCheckBox.SelectedValue).ToString();
                    voteInfo.ContentResultVisible = EVoteResultVisibleUtils.GetValue(EVoteResultVisible.After);

                    voteInfo.EndTitle = tbEndTitle.Text;
                    voteInfo.EndImageUrl = endImageUrl.Value;
                    voteInfo.EndSummary = tbEndSummary.Text;

                    try
                    {
                        if (voteID > 0)
                        {
                            DataProviderWX.VoteDAO.Update(voteInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "修改投票活动",
                                $"投票活动:{tbTitle.Text}");
                            SuccessMessage("修改投票活动成功！");
                        }
                        else
                        {
                            voteID = DataProviderWX.VoteDAO.Insert(voteInfo);

                            DataProviderWX.VoteItemDAO.UpdateVoteID(PublishmentSystemID, voteID);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "添加投票活动",
                                $"投票活动:{tbTitle.Text}");
                            SuccessMessage("添加投票活动成功！");
                        }

                        var redirectUrl = PageUtils.GetWXUrl(
                            $"background_vote.aspx?publishmentSystemID={PublishmentSystemID}");
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "投票活动设置失败！");
                    }

                    btnSubmit.Visible = false;
                    btnReturn.Visible = false;
                }
			}
		}
	}
}
