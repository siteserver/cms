using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundCardAdd : BackgroundBasePageWX
    {

        public Literal ltlPageTitle;
        public PlaceHolder phStep1;
        public TextBox tbKeywords;
        public TextBox tbTitle;
        public TextBox tbSummary;
        public CheckBox cbIsEnabled;
        public Literal ltlImageUrl;
        public HtmlInputHidden imageUrl;

        public PlaceHolder phStep2;
        public TextBox tbCardTitle;
        public TextBox tbCardTitleColor;
        public TextBox tbCardSNColor;
        public Literal ltlContentFrontImageUrl;
        public HtmlInputHidden contentFrontImageUrl;
        public Literal ltlContentBackImageUrl;
        public HtmlInputHidden contentBackImageUrl;

        public PlaceHolder phStep3;
        public TextBox tbShopName;
        public TextBox tbShopAddress;

        public Literal ltlMap;
        public HtmlInputHidden shopPosition;
        public TextBox tbShopTel;
        public TextBox tbShopManage;
        public TextBox tbShopPassword;

        public Button btnSubmit;
        public Button btnReturn;

        private int cardID;

        public static string GetRedirectUrl(int publishmentSystemID, int CardID)
        {
            return PageUtils.GetWXUrl(
                $"background_cardAdd.aspx?publishmentSystemID={publishmentSystemID}&CardID={CardID}");
        }

        public string GetUploadUrl()
        {
            return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            cardID = TranslateUtils.ToInt(GetQueryString("CardID"));

            if (!IsPostBack)
            {
                var pageTitle = cardID > 0 ? "编辑会员卡" : "添加会员卡";
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Card, pageTitle, AppManager.WeiXin.Permission.WebSite.Card);
                ltlPageTitle.Text = pageTitle;

                ltlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{CardManager.GetImageUrl(PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                ltlContentFrontImageUrl.Text =
                    $@"<img id=""preview_contentFrontImageUrl"" src=""{CardManager.GetContentFrontImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                ltlContentBackImageUrl.Text =
                    $@"<img id=""preview_contentBackImageUrl"" src=""{CardManager.GetContentBackImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";

                if (cardID > 0)
                {
                    var cardInfo = DataProviderWX.CardDAO.GetCardInfo(cardID);

                    tbKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(cardInfo.KeywordID);
                    cbIsEnabled.Checked = !cardInfo.IsDisabled;
                    tbTitle.Text = cardInfo.Title;
                    if (!string.IsNullOrEmpty(cardInfo.ImageUrl))
                    {
                        ltlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, cardInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    if (!string.IsNullOrEmpty(cardInfo.ContentFrontImageUrl))
                    {
                        ltlContentFrontImageUrl.Text =
                            $@"<img id=""preview_contentFrontImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, cardInfo.ContentFrontImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    if (!string.IsNullOrEmpty(cardInfo.ContentBackImageUrl))
                    {
                        ltlContentBackImageUrl.Text =
                            $@"<img id=""preview_contentBackImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, cardInfo.ContentBackImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    tbSummary.Text = cardInfo.Summary;

                    tbCardTitle.Text = cardInfo.CardTitle;
                    tbCardTitleColor.Text = cardInfo.CardTitleColor;
                    tbCardSNColor.Text = cardInfo.CardNoColor;

                    tbShopName.Text = cardInfo.ShopName;
                    if (!string.IsNullOrEmpty(cardInfo.ShopPosition))
                    {
                        shopPosition.Value = cardInfo.ShopPosition;

                    }
                    if (!string.IsNullOrEmpty(cardInfo.ShopAddress))
                    {
                        tbShopAddress.Text = cardInfo.ShopAddress;
                        ltlMap.Text =
                            $@"<iframe style=""width:100%;height:100%;background-color:#ffffff;margin-bottom:15px;"" scrolling=""auto"" frameborder=""0"" width=""100%"" height=""100%"" src=""{MapManager
                                .GetMapUrl(cardInfo.ShopAddress)}""></iframe>";
                    }
                    tbShopTel.Text = cardInfo.ShopTel;
                    tbShopPassword.Text = cardInfo.ShopPassword;

                    imageUrl.Value = cardInfo.ImageUrl;
                    contentFrontImageUrl.Value = cardInfo.ContentFrontImageUrl;
                    contentBackImageUrl.Value = cardInfo.ContentBackImageUrl;

                }

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundCard.GetRedirectUrl(PublishmentSystemID)}"";return false");
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
                        if (cardID > 0)
                        {
                            var cardInfo = DataProviderWX.CardDAO.GetCardInfo(cardID);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemID, cardInfo.KeywordID, PageUtils.FilterXSS(tbKeywords.Text), out conflictKeywords);
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
                    phStep3.Visible = true;
                    btnSubmit.Text = "确 认";
                }
                else if (selectedStep == 3)
                {
                    var cardInfo = new CardInfo();
                    if (cardID > 0)
                    {
                        cardInfo = DataProviderWX.CardDAO.GetCardInfo(cardID);
                    }
                    else
                    {
                        cardInfo.ShopOperatorList = "[{\"UserName\":" + tbShopManage.Text + ",\"Password\":" + tbShopPassword.Text + ",\"ID\":0}]";
                    }
                    cardInfo.PublishmentSystemID = PublishmentSystemID;
                    cardInfo.KeywordID = DataProviderWX.KeywordDAO.GetKeywordID(PublishmentSystemID, cardID > 0, PageUtils.FilterXSS(tbKeywords.Text), EKeywordType.Card, cardInfo.KeywordID);
                    cardInfo.IsDisabled = !cbIsEnabled.Checked;

                    cardInfo.Title = PageUtils.FilterXSS(tbTitle.Text);
                    cardInfo.ImageUrl = imageUrl.Value; 
                    cardInfo.Summary = tbSummary.Text;

                    cardInfo.CardTitle = tbCardTitle.Text;
                    cardInfo.CardTitleColor = tbCardTitleColor.Text;
                    cardInfo.CardNoColor = tbCardSNColor.Text;
                    cardInfo.ContentFrontImageUrl = contentFrontImageUrl.Value;
                    cardInfo.ContentBackImageUrl = contentBackImageUrl.Value;

                    cardInfo.ShopName = tbShopName.Text;
                    cardInfo.ShopAddress = tbShopAddress.Text;
                    if (!string.IsNullOrEmpty(shopPosition.Value))
                    {
                        cardInfo.ShopPosition = shopPosition.Value;
                    }
                    cardInfo.ShopTel = tbShopTel.Text;
                    cardInfo.ShopPassword = tbShopPassword.Text;

                    try
                    {
                        if (cardID > 0)
                        {
                            DataProviderWX.CardDAO.Update(cardInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "修改会员卡",
                                $"会员卡:{tbTitle.Text}");
                            SuccessMessage("修改会员卡成功！");
                        }
                        else
                        {
                            cardID = DataProviderWX.CardDAO.Insert(cardInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "添加会员卡",
                                $"会员卡:{tbTitle.Text}");
                            SuccessMessage("添加会员卡成功！");
                        }

                        var redirectUrl = PageUtils.GetWXUrl(
                            $"background_card.aspx?publishmentSystemID={PublishmentSystemID}&CardID={cardID}");
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "会员卡设置失败！");
                    }
                }
            }
        }
    }
}
