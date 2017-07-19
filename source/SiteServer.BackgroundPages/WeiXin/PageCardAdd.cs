using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageCardAdd : BasePageCms
    {
        public Literal LtlPageTitle;
        public PlaceHolder PhStep1;
        public TextBox TbKeywords;
        public TextBox TbTitle;
        public TextBox TbSummary;
        public CheckBox CbIsEnabled;
        public Literal LtlImageUrl;
        public HtmlInputHidden ImageUrl;

        public PlaceHolder PhStep2;
        public TextBox TbCardTitle;
        public TextBox TbCardTitleColor;
        public TextBox TbCardSnColor;
        public Literal LtlContentFrontImageUrl;
        public HtmlInputHidden ContentFrontImageUrl;
        public Literal LtlContentBackImageUrl;
        public HtmlInputHidden ContentBackImageUrl;

        public PlaceHolder PhStep3;
        public TextBox TbShopName;
        public TextBox TbShopAddress;

        public Literal LtlMap;
        public HtmlInputHidden ShopPosition;
        public TextBox TbShopTel;
        public TextBox TbShopManage;
        public TextBox TbShopPassword;

        public Button BtnSubmit;
        public Button BtnReturn;

        private int _cardId;

        public static string GetRedirectUrl(int publishmentSystemId, int cardId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageCardAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"cardId", cardId.ToString()}
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
            _cardId = Body.GetQueryInt("CardID");

            if (!IsPostBack)
            {
                var pageTitle = _cardId > 0 ? "编辑会员卡" : "添加会员卡";
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdCard, pageTitle, AppManager.WeiXin.Permission.WebSite.Card);
                LtlPageTitle.Text = pageTitle;

                LtlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{CardManager.GetImageUrl(PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                LtlContentFrontImageUrl.Text =
                    $@"<img id=""preview_contentFrontImageUrl"" src=""{CardManager.GetContentFrontImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                LtlContentBackImageUrl.Text =
                    $@"<img id=""preview_contentBackImageUrl"" src=""{CardManager.GetContentBackImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";

                if (_cardId > 0)
                {
                    var cardInfo = DataProviderWx.CardDao.GetCardInfo(_cardId);

                    TbKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(cardInfo.KeywordId);
                    CbIsEnabled.Checked = !cardInfo.IsDisabled;
                    TbTitle.Text = cardInfo.Title;
                    if (!string.IsNullOrEmpty(cardInfo.ImageUrl))
                    {
                        LtlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, cardInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    if (!string.IsNullOrEmpty(cardInfo.ContentFrontImageUrl))
                    {
                        LtlContentFrontImageUrl.Text =
                            $@"<img id=""preview_contentFrontImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, cardInfo.ContentFrontImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    if (!string.IsNullOrEmpty(cardInfo.ContentBackImageUrl))
                    {
                        LtlContentBackImageUrl.Text =
                            $@"<img id=""preview_contentBackImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, cardInfo.ContentBackImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    TbSummary.Text = cardInfo.Summary;

                    TbCardTitle.Text = cardInfo.CardTitle;
                    TbCardTitleColor.Text = cardInfo.CardTitleColor;
                    TbCardSnColor.Text = cardInfo.CardNoColor;

                    TbShopName.Text = cardInfo.ShopName;
                    if (!string.IsNullOrEmpty(cardInfo.ShopPosition))
                    {
                        ShopPosition.Value = cardInfo.ShopPosition;

                    }
                    if (!string.IsNullOrEmpty(cardInfo.ShopAddress))
                    {
                        TbShopAddress.Text = cardInfo.ShopAddress;
                        LtlMap.Text =
                            $@"<iframe style=""width:100%;height:100%;background-color:#ffffff;margin-bottom:15px;"" scrolling=""auto"" frameborder=""0"" width=""100%"" height=""100%"" src=""{MapManager.GetMapUrl(PublishmentSystemInfo, cardInfo.ShopAddress)}""></iframe>";
                    }
                    TbShopTel.Text = cardInfo.ShopTel;
                    TbShopPassword.Text = cardInfo.ShopPassword;

                    ImageUrl.Value = cardInfo.ImageUrl;
                    ContentFrontImageUrl.Value = cardInfo.ContentFrontImageUrl;
                    ContentBackImageUrl.Value = cardInfo.ContentBackImageUrl;

                }

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageCard.GetRedirectUrl(PublishmentSystemId)}"";return false");
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
                        if (_cardId > 0)
                        {
                            var cardInfo = DataProviderWx.CardDao.GetCardInfo(_cardId);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, cardInfo.KeywordId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
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
                    PhStep3.Visible = true;
                    BtnSubmit.Text = "确 认";
                }
                else if (selectedStep == 3)
                {
                    var cardInfo = new CardInfo();
                    if (_cardId > 0)
                    {
                        cardInfo = DataProviderWx.CardDao.GetCardInfo(_cardId);
                    }
                    else
                    {
                        cardInfo.ShopOperatorList = "[{\"UserName\":" + TbShopManage.Text + ",\"Password\":" + TbShopPassword.Text + ",\"ID\":0}]";
                    }
                    cardInfo.PublishmentSystemId = PublishmentSystemId;
                    cardInfo.KeywordId = DataProviderWx.KeywordDao.GetKeywordId(PublishmentSystemId, _cardId > 0, PageUtils.FilterXss(TbKeywords.Text), EKeywordType.Card, cardInfo.KeywordId);
                    cardInfo.IsDisabled = !CbIsEnabled.Checked;

                    cardInfo.Title = PageUtils.FilterXss(TbTitle.Text);
                    cardInfo.ImageUrl = ImageUrl.Value; 
                    cardInfo.Summary = TbSummary.Text;

                    cardInfo.CardTitle = TbCardTitle.Text;
                    cardInfo.CardTitleColor = TbCardTitleColor.Text;
                    cardInfo.CardNoColor = TbCardSnColor.Text;
                    cardInfo.ContentFrontImageUrl = ContentFrontImageUrl.Value;
                    cardInfo.ContentBackImageUrl = ContentBackImageUrl.Value;

                    cardInfo.ShopName = TbShopName.Text;
                    cardInfo.ShopAddress = TbShopAddress.Text;
                    if (!string.IsNullOrEmpty(ShopPosition.Value))
                    {
                        cardInfo.ShopPosition = ShopPosition.Value;
                    }
                    cardInfo.ShopTel = TbShopTel.Text;
                    cardInfo.ShopPassword = TbShopPassword.Text;

                    try
                    {
                        if (_cardId > 0)
                        {
                            DataProviderWx.CardDao.Update(cardInfo);

                            LogUtils.AddAdminLog(Body.AdministratorName, "修改会员卡", $"会员卡:{TbTitle.Text}");
                            SuccessMessage("修改会员卡成功！");
                        }
                        else
                        {
                            _cardId = DataProviderWx.CardDao.Insert(cardInfo);

                            LogUtils.AddAdminLog(Body.AdministratorName, "添加会员卡", $"会员卡:{TbTitle.Text}");
                            SuccessMessage("添加会员卡成功！");
                        }

                        var redirectUrl = PageCard.GetRedirectUrl(PublishmentSystemId);
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
