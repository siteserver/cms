using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
	public class PageVoteAdd : BasePageCms
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
        public DropDownList DdlContentIsImageOption;
        public DropDownList DdlContentIsCheckBox;
        public Literal LtlContentImageUrl;
        public Literal LtlVoteItems;

        public PlaceHolder PhStep3;
        public TextBox TbEndTitle;
        public TextBox TbEndSummary;
        public Literal LtlEndImageUrl;

        public HtmlInputHidden ImageUrl;
        public HtmlInputHidden ContentImageUrl;
        public HtmlInputHidden EndImageUrl;

        public Button BtnSubmit;
        public Button BtnReturn;

        private int _voteId;

        public static string GetRedirectUrl(int publishmentSystemId, int voteId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageVoteAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"voteId", voteId.ToString()}
            });
        }

        public string GetUploadUrl()
        {
            return AjaxUpload.GetImageUrlUploadUrl(PublishmentSystemId);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemId");
            _voteId = Body.GetQueryInt("voteID");

			if (!IsPostBack)
            {
                var pageTitle = _voteId > 0 ? "编辑投票活动" : "添加投票活动";
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdVote, pageTitle, AppManager.WeiXin.Permission.WebSite.Vote);
                LtlPageTitle.Text = pageTitle;

                var listItem = new ListItem("文字类型投票", "false");
                DdlContentIsImageOption.Items.Add(listItem);
                listItem = new ListItem("图文类型投票", "true");
                DdlContentIsImageOption.Items.Add(listItem);

                DdlContentIsImageOption.Attributes.Add("onchange", "itemController.isImageOptionChange(this)");
                EBooleanUtils.AddListItems(DdlContentIsCheckBox, "多选", "单选");
                ControlUtils.SelectListItems(DdlContentIsCheckBox, false.ToString());

                LtlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{VoteManager.GetImageUrl(PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                LtlContentImageUrl.Text =
                    $@"<img id=""preview_contentImageUrl"" src=""{VoteManager.GetContentImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                LtlEndImageUrl.Text =
                    $@"<img id=""preview_endImageUrl"" src=""{VoteManager.GetEndImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";

                var selectImageClick = ModalSelectImage.GetOpenWindowString(PublishmentSystemInfo, "itemImageUrl_");
                var uploadImageClick = ModalUploadImageSingle.GetOpenWindowStringToTextBox(PublishmentSystemId, "itemImageUrl_");
                var cuttingImageClick = ModalCuttingImage.GetOpenWindowStringWithTextBox(PublishmentSystemId, "itemImageUrl_");
                var previewImageClick = ModalMessage.GetOpenWindowStringToPreviewImage(PublishmentSystemId, "itemImageUrl_");
                LtlVoteItems.Text =
                    $@"itemController.selectImageClickString = ""{selectImageClick}"";itemController.uploadImageClickString = ""{uploadImageClick}"";itemController.cuttingImageClickString = ""{cuttingImageClick}"";itemController.previewImageClickString = ""{previewImageClick}"";";

                if (_voteId == 0)
                {
                    LtlVoteItems.Text += "itemController.isImageOption = false;itemController.itemCount = 2;itemController.items = [{}, {}];";
                    DtbEndDate.DateTime = DateTime.Now.AddMonths(1);
                }
                else
                {
                    var voteInfo = DataProviderWx.VoteDao.GetVoteInfo(_voteId);

                    TbKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(voteInfo.KeywordId);
                    CbIsEnabled.Checked = !voteInfo.IsDisabled;
                    DtbStartDate.DateTime = voteInfo.StartDate;
                    DtbEndDate.DateTime = voteInfo.EndDate;
                    TbTitle.Text = voteInfo.Title;
                    if (!string.IsNullOrEmpty(voteInfo.ImageUrl))
                    {
                        LtlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, voteInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    TbSummary.Text = voteInfo.Summary;
                    if (!string.IsNullOrEmpty(voteInfo.ContentImageUrl))
                    {
                        LtlContentImageUrl.Text =
                            $@"<img id=""preview_contentImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, voteInfo.ContentImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    TbContentDescription.Text = voteInfo.ContentDescription;
                    //ControlUtils.SelectListItems(this.ddlContentResultVisible, voteInfo.ContentResultVisible);
                    ControlUtils.SelectListItems(DdlContentIsImageOption, voteInfo.ContentIsImageOption.ToLower());
                    ControlUtils.SelectListItems(DdlContentIsCheckBox, voteInfo.ContentIsCheckBox);

                    var voteItemInfoList = DataProviderWx.VoteItemDao.GetVoteItemInfoList(_voteId);
                    var itemBuilder = new StringBuilder();
                    foreach (var itemInfo in voteItemInfoList)
                    {
                        itemBuilder.AppendFormat(@"{{id: '{0}', title: '{1}', imageUrl: '{2}', navigationUrl: '{3}', voteNum: '{4}'}},", itemInfo.Id, itemInfo.Title, itemInfo.ImageUrl, itemInfo.NavigationUrl, itemInfo.VoteNum);
                    }
                    if (itemBuilder.Length > 0) itemBuilder.Length--;

                    LtlVoteItems.Text += $@"
itemController.isImageOption = {voteInfo.ContentIsImageOption.ToLower()};itemController.itemCount = {voteItemInfoList
                        .Count};itemController.items = [{itemBuilder}];";

                    TbEndTitle.Text = voteInfo.EndTitle;
                    TbEndSummary.Text = voteInfo.EndSummary;
                    if (!string.IsNullOrEmpty(voteInfo.EndImageUrl))
                    {
                        LtlEndImageUrl.Text =
                            $@"<img id=""preview_endImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, voteInfo.EndImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    ImageUrl.Value = voteInfo.ImageUrl;
                    ContentImageUrl.Value = voteInfo.ContentImageUrl;
                    EndImageUrl.Value = voteInfo.EndImageUrl;
                }

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageVote.GetRedirectUrl(PublishmentSystemId)}"";return false");
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

                PhStep1.Visible = PhStep2.Visible = PhStep3.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(TbKeywords.Text))
                    {
                        if (_voteId > 0)
                        {
                            var voteInfo = DataProviderWx.VoteDao.GetVoteInfo(_voteId);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, voteInfo.KeywordId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
                        }
                        else
                        {
                            isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
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
                    var isItemReady = true;
                    var itemCount = TranslateUtils.ToInt(Request.Form["itemCount"]);

                    if (itemCount < 2)
                    {
                        FailMessage("投票保存失败，至少设置两个投票项");
                        isItemReady = false;
                    }
                    else
                    {
                        var isImageOption = TranslateUtils.ToBool(DdlContentIsImageOption.SelectedValue);

                        var itemIdList = TranslateUtils.StringCollectionToIntList(Request.Form["itemID"]);
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
                            var itemInfo = new VoteItemInfo { Id = itemIdList[i], VoteId = _voteId, PublishmentSystemId = PublishmentSystemId, Title = titleList[i], ImageUrl = imageUrl, NavigationUrl = navigationUrlList[i], VoteNum = voteNumList[i] };

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
                            //DataProviderWx.VoteItemDao.DeleteAll(base.PublishmentSystemId, this.voteID);
                            
                            foreach (var itemInfo in voteItemInfoList)
                            {
                               var newVoteItemInfo=DataProviderWx.VoteItemDao.GetVoteItemInfo(itemInfo.Id);
                               if (itemInfo.Id>0)
                                {
                                    itemInfo.VoteNum = newVoteItemInfo.VoteNum;
                                    DataProviderWx.VoteItemDao.Update(itemInfo);
                                }
                                else
                                {
                                    DataProviderWx.VoteItemDao.Insert(itemInfo);
                                }
                            }
                        }
                    }

                    if (isItemReady)
                    {
                        PhStep3.Visible = true;
                        BtnSubmit.Text = "确 认";
                    }
                    else
                    {
                        PhStep2.Visible = true;
                    }
                }
                else if (selectedStep == 3)
                {
                    var voteInfo = new VoteInfo();
                    if (_voteId > 0)
                    {
                        voteInfo = DataProviderWx.VoteDao.GetVoteInfo(_voteId);
                    }
                    voteInfo.PublishmentSystemId = PublishmentSystemId;

                    voteInfo.KeywordId = DataProviderWx.KeywordDao.GetKeywordId(PublishmentSystemId, _voteId > 0, TbKeywords.Text, EKeywordType.Vote, voteInfo.KeywordId);
                    voteInfo.IsDisabled = !CbIsEnabled.Checked;

                    voteInfo.StartDate = DtbStartDate.DateTime;
                    voteInfo.EndDate = DtbEndDate.DateTime;
                    voteInfo.Title = PageUtils.FilterXss(TbTitle.Text);
                    voteInfo.ImageUrl = ImageUrl.Value; ;
                    voteInfo.Summary = TbSummary.Text;

                    voteInfo.ContentImageUrl = ContentImageUrl.Value;
                    voteInfo.ContentDescription = TbContentDescription.Text;
                    voteInfo.ContentIsImageOption = TranslateUtils.ToBool(DdlContentIsImageOption.SelectedValue).ToString();
                    voteInfo.ContentIsCheckBox = TranslateUtils.ToBool(DdlContentIsCheckBox.SelectedValue).ToString();
                    voteInfo.ContentResultVisible = EVoteResultVisibleUtils.GetValue(EVoteResultVisible.After);

                    voteInfo.EndTitle = TbEndTitle.Text;
                    voteInfo.EndImageUrl = EndImageUrl.Value;
                    voteInfo.EndSummary = TbEndSummary.Text;

                    try
                    {
                        if (_voteId > 0)
                        {
                            DataProviderWx.VoteDao.Update(voteInfo);

                            LogUtils.AddAdminLog(Body.AdministratorName, "修改投票活动", $"投票活动:{TbTitle.Text}");
                            SuccessMessage("修改投票活动成功！");
                        }
                        else
                        {
                            _voteId = DataProviderWx.VoteDao.Insert(voteInfo);

                            DataProviderWx.VoteItemDao.UpdateVoteId(PublishmentSystemId, _voteId);

                            LogUtils.AddAdminLog(Body.AdministratorName, "添加投票活动", $"投票活动:{TbTitle.Text}");
                            SuccessMessage("添加投票活动成功！");
                        }

                        var redirectUrl = PageVote.GetRedirectUrl(PublishmentSystemId);
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "投票活动设置失败！");
                    }

                    BtnSubmit.Visible = false;
                    BtnReturn.Visible = false;
                }
			}
		}
	}
}
