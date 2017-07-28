using System;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using System.Collections.Generic;


namespace SiteServer.WeiXin.BackgroundPages.Modal
{
    public class CardConsume : BackgroundBasePage
    {
        public DropDownList ddlCard;
        public PlaceHolder phKeyWord;
        public DropDownList ddlKeyWordType;
        public TextBox tbKeyWord;
        public TextBox tbConsumeAmount;
        public DropDownList ddlConsumeType;
        public DropDownList ddlOperator;

        private int cardID;
        private int cardSNID;
         
        public static string GetOpenWindowString(int publishmentSystemID, int cardID, int cardSNID)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("cardID", cardID.ToString());
            arguments.Add("cardSNID", cardSNID.ToString());
            return PageUtilityWX.GetOpenWindowString("会员消费", "modal_cardConsume.aspx", arguments, 400, 380);
        }
        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            cardID = TranslateUtils.ToInt(GetQueryString("cardID"));
            cardSNID = TranslateUtils.ToInt(GetQueryString("cardSNID"));
            
            if (!IsPostBack)
            {
                var cardInfoList = DataProviderWX.CardDAO.GetCardInfoList(PublishmentSystemID);
                foreach (var cardInfo in cardInfoList)
                {
                    if (cardID <= 0)
                    {
                        cardID = cardInfo.ID;
                    }
                   ddlCard.Items.Add(new ListItem(cardInfo.CardTitle, cardInfo.ID.ToString()));
                }

                ddlKeyWordType.Items.Add(new ListItem("卡号", "cardSN"));
                ddlKeyWordType.Items.Add(new ListItem("手机", "mobile"));

                EConsumeTypeUtils.AddListItems(ddlConsumeType);
                ControlUtils.SelectListItems(ddlConsumeType, EConsumeTypeUtils.GetValue(EConsumeType.CardAmount));

                var operatorInfoList = new List<CardOperatorInfo>();
                var theCardInfo = DataProviderWX.CardDAO.GetCardInfo(cardID);
                if (theCardInfo != null)
                {
                    operatorInfoList = TranslateUtils.JsonToObject(theCardInfo.ShopOperatorList, operatorInfoList) as List<CardOperatorInfo>;
                    if (operatorInfoList != null)
                    {
                        foreach (var operaotorInfo in operatorInfoList)
                        {
                            ddlOperator.Items.Add(new ListItem(operaotorInfo.UserName, operaotorInfo.UserName));
                        }
                    }
                }
                if (!string.IsNullOrEmpty(GetQueryString("cardType")))
                {
                    ControlUtils.SelectListItems(ddlCard, GetQueryString("cardType"));
                }

                if (cardSNID > 0)
                {
                    phKeyWord.Visible = false;
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (IsPostBack && IsValid)
            {
                UserInfo userInfo = null;
                CardSNInfo cardSNInfo = null;

                if (cardSNID > 0)
                {
                    cardSNInfo = DataProviderWX.CardSNDAO.GetCardSNInfo(cardSNID);
                }
                else
                {
                    if (ddlKeyWordType.SelectedValue == "cardSN")
                    {
                        cardSNInfo = DataProviderWX.CardSNDAO.GetCardSNInfo(PublishmentSystemID, TranslateUtils.ToInt(ddlCard.SelectedValue), tbKeyWord.Text, string.Empty);
                    }
                    else if (ddlKeyWordType.SelectedValue == "mobile")
                    {
                        var userID = BaiRongDataProvider.UserDao.GetUserIdByEmailOrMobile(string.Empty, tbKeyWord.Text);
                        userInfo = BaiRongDataProvider.UserDao.GetUserInfo(userID);
                        if (userInfo != null)
                        {
                            cardSNInfo = DataProviderWX.CardSNDAO.GetCardSNInfo(PublishmentSystemID, TranslateUtils.ToInt(ddlCard.SelectedValue), string.Empty, userInfo.UserName);
                        }
                    }
                }

                if (cardSNInfo == null)
                {
                    FailMessage("会员卡不存在");
                    return;
                }

                var consumeType = EConsumeTypeUtils.GetEnumType(ddlConsumeType.SelectedValue);

                if (consumeType == EConsumeType.CardAmount)
                {
                    var amount = DataProviderWX.CardSNDAO.GetAmount(cardSNInfo.ID, cardSNInfo.UserName);
                    if (amount < TranslateUtils.ToDecimal(tbConsumeAmount.Text))
                    {
                        FailMessage("会员卡余额不足");
                        return;
                    }
                }

                var cardCashLogInfo = new CardCashLogInfo();
                cardCashLogInfo.PublishmentSystemID = PublishmentSystemID;
                cardCashLogInfo.UserName = cardSNInfo.UserName;
                cardCashLogInfo.CardID = cardSNInfo.CardID;
                cardCashLogInfo.CardSNID = cardSNInfo.ID;
                cardCashLogInfo.Amount = TranslateUtils.ToDecimal(tbConsumeAmount.Text);
                cardCashLogInfo.CurAmount = cardSNInfo.Amount;
                if (consumeType == EConsumeType.CardAmount)
                {
                    cardCashLogInfo.CurAmount = cardSNInfo.Amount - TranslateUtils.ToInt(tbConsumeAmount.Text);
                }

                cardCashLogInfo.CashType = ECashTypeUtils.GetValue(ECashType.Consume);
                cardCashLogInfo.ConsumeType = ddlConsumeType.SelectedValue;
                cardCashLogInfo.Operator = ddlOperator.SelectedValue;
                cardCashLogInfo.AddDate = DateTime.Now;

                try
                {
                    DataProviderWX.CardCashLogDAO.Insert(cardCashLogInfo);

                    if (consumeType == EConsumeType.CardAmount)
                    {
                        DataProviderWX.CardSNDAO.Consume(cardSNInfo.ID, cardSNInfo.UserName, TranslateUtils.ToDecimal(tbConsumeAmount.Text));

                        if (PublishmentSystemInfo.Additional.Card_IsClaimCardCredits)
                        {
                            var amount = TranslateUtils.ToDecimal(tbConsumeAmount.Text);
                            var consumeAmount = PublishmentSystemInfo.Additional.Card_ConsumeAmount;
                            var giveCredits = PublishmentSystemInfo.Additional.Card_GiveCredits;

                            //var userCreditsLogInfo = new UserCreditsLogInfo();
                            //userCreditsLogInfo.UserName = cardSNInfo.UserName;
                            //userCreditsLogInfo.ProductId = AppManager.WeiXin.AppID;
                            //userCreditsLogInfo.Num = (int)Math.Round(amount * (giveCredits / consumeAmount), 0);
                            //userCreditsLogInfo.AddDate = DateTime.Now;
                            //userCreditsLogInfo.IsIncreased = true;
                            //userCreditsLogInfo.Action = "消费送积分";

                            //BaiRongDataProvider.UserCreditsLogDao.Insert(userCreditsLogInfo);
                            //BaiRongDataProvider.UserDao.AddCredits(cardSNInfo.UserName, (int)Math.Round(amount * (giveCredits / consumeAmount), 0));
                        }
                    }
                    tbConsumeAmount.Text = string.Empty;

                    SuccessMessage("操作成功！");

                    //JsUtils.OpenWindow.CloseModalPage(Page);
                   
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "操作失败！");
                }
            }
        }
        public void Refrush(object sender, EventArgs e)
        {
            cardID = TranslateUtils.ToInt(ddlCard.SelectedValue);

            PageUtils.Redirect(PageUrl);
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetWXUrl(
                        $"modal_cardConsume.aspx?PublishmentSystemID={PublishmentSystemID}&cardID={ddlCard.SelectedValue}&cardSNID={cardSNID}&cardType={ddlCard.SelectedValue}");
                }
                return _pageUrl;
            }
        }
    }
}