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
    public class CardRecharge : BackgroundBasePage
    {
        public DropDownList ddlCard;
        public PlaceHolder phKeyWord;
        public DropDownList ddlKeyWordType;
        public TextBox tbKeyWord;
        public TextBox tbRechargeAmount;
        public DropDownList ddlOperator;
        public TextBox tbDescription;

         
        private int cardID;
        private int cardSNID;
        
        public static string GetOpenWindowString(int publishmentSystemID,int cardID,int cardSNID)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("cardID", cardID.ToString());
            arguments.Add("cardSNID", cardSNID.ToString());
            return PageUtilityWX.GetOpenWindowString("会员卡充值", "modal_cardRecharge.aspx", arguments, 400,380);
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
                  
                var cardCashLogInfo = new CardCashLogInfo();
                cardCashLogInfo.PublishmentSystemID = PublishmentSystemID;
                cardCashLogInfo.UserName = cardSNInfo.UserName;
                cardCashLogInfo.CardID = cardSNInfo.CardID;
                cardCashLogInfo.CardSNID = cardSNInfo.ID;
                cardCashLogInfo.Amount = TranslateUtils.ToInt(tbRechargeAmount.Text);
                cardCashLogInfo.CurAmount = cardSNInfo.Amount + TranslateUtils.ToDecimal(tbRechargeAmount.Text);
                cardCashLogInfo.CashType = ECashTypeUtils.GetValue(ECashType.Recharge);
                cardCashLogInfo.Operator = ddlOperator.SelectedValue;
                cardCashLogInfo.Description = tbDescription.Text;
                cardCashLogInfo.AddDate = DateTime.Now;

                try
                {
                    DataProviderWX.CardCashLogDAO.Insert(cardCashLogInfo);
                    DataProviderWX.CardSNDAO.Recharge(cardSNInfo.ID, cardSNInfo.UserName, TranslateUtils.ToDecimal(tbRechargeAmount.Text));

                    tbRechargeAmount.Text = string.Empty;

                    SuccessMessage("充值成功！");

                    //JsUtils.OpenWindow.CloseModalPage(Page);
                    
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "充值失败！");
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
                        $"modal_cardRecharge.aspx?PublishmentSystemID={PublishmentSystemID}&cardID={ddlCard.SelectedValue}&cardSNID={cardSNID}&cardType={ddlCard.SelectedValue}");
                }
                return _pageUrl;
            }
        }
    }
}