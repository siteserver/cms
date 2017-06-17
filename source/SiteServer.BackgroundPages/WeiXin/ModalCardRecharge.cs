using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalCardRecharge : BasePageCms
    {
        public DropDownList DdlCard;
        public PlaceHolder PhKeyWord;
        public DropDownList DdlKeyWordType;
        public TextBox TbKeyWord;
        public TextBox TbRechargeAmount;
        public DropDownList DdlOperator;
        public TextBox TbDescription;

         
        private int _cardId;
        private int _cardSnid;
        
        public static string GetOpenWindowString(int publishmentSystemId,int cardId, int cardSnid)
        {
            return PageUtils.GetOpenWindowString("会员卡充值",
                PageUtils.GetWeiXinUrl(nameof(ModalCardRecharge), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"cardId", cardId.ToString()},
                    {"cardSnid", cardSnid.ToString()}
                }), 400, 380);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _cardId = Body.GetQueryInt("cardID");
            _cardSnid = Body.GetQueryInt("cardSNID");

            if (!IsPostBack)
            {
                var cardInfoList = DataProviderWx.CardDao.GetCardInfoList(PublishmentSystemId);
                foreach (var cardInfo in cardInfoList)
                {
                    if (_cardId <= 0)
                    {
                        _cardId = cardInfo.Id;
                    }
                    DdlCard.Items.Add(new ListItem(cardInfo.CardTitle, cardInfo.Id.ToString()));
                }

                DdlKeyWordType.Items.Add(new ListItem("卡号", "cardSN"));
                DdlKeyWordType.Items.Add(new ListItem("手机", "mobile"));


                var operatorInfoList = new List<CardOperatorInfo>();
                var theCardInfo = DataProviderWx.CardDao.GetCardInfo(_cardId);
                if (theCardInfo != null)
                {
                    operatorInfoList = TranslateUtils.JsonToObject(theCardInfo.ShopOperatorList, operatorInfoList) as List<CardOperatorInfo>;
                    if (operatorInfoList != null)
                    {
                        foreach (var operaotorInfo in operatorInfoList)
                        {
                            DdlOperator.Items.Add(new ListItem(operaotorInfo.UserName, operaotorInfo.UserName));
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Body.GetQueryString("cardType")))
                {
                    ControlUtils.SelectListItems(DdlCard, Body.GetQueryString("cardType"));
                }
                if (_cardSnid > 0)
                {
                    PhKeyWord.Visible = false;
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (IsPostBack && IsValid)
            { 
                UserInfo userInfo = null;
                CardSnInfo cardSnInfo = null;
                if (_cardSnid > 0)
                {
                    cardSnInfo = DataProviderWx.CardSnDao.GetCardSnInfo(_cardSnid);
                }
                else
                {
                    if (DdlKeyWordType.SelectedValue == "cardSN")
                    {
                        cardSnInfo = DataProviderWx.CardSnDao.GetCardSnInfo(PublishmentSystemId, TranslateUtils.ToInt(DdlCard.SelectedValue), TbKeyWord.Text, string.Empty);
                    }
                    else if (DdlKeyWordType.SelectedValue == "mobile")
                    {
                        var userId = BaiRongDataProvider.UserDao.GetUserIdByEmailOrMobile(string.Empty, TbKeyWord.Text);
                        userInfo = BaiRongDataProvider.UserDao.GetUserInfo(userId);
                        if (userInfo != null)
                        {
                            cardSnInfo = DataProviderWx.CardSnDao.GetCardSnInfo(PublishmentSystemId, TranslateUtils.ToInt(DdlCard.SelectedValue), string.Empty, userInfo.UserName);
                        }
                    }
                }
               
                if (cardSnInfo == null)
                {
                    FailMessage("会员卡不存在");
                    return;
                }
                  
                var cardCashLogInfo = new CardCashLogInfo();
                cardCashLogInfo.PublishmentSystemId = PublishmentSystemId;
                cardCashLogInfo.UserName = cardSnInfo.UserName;
                cardCashLogInfo.CardId = cardSnInfo.CardId;
                cardCashLogInfo.CardSnId = cardSnInfo.Id;
                cardCashLogInfo.Amount = TranslateUtils.ToInt(TbRechargeAmount.Text);
                cardCashLogInfo.CurAmount = cardSnInfo.Amount + TranslateUtils.ToDecimal(TbRechargeAmount.Text);
                cardCashLogInfo.CashType = ECashTypeUtils.GetValue(ECashType.Recharge);
                cardCashLogInfo.Operator = DdlOperator.SelectedValue;
                cardCashLogInfo.Description = TbDescription.Text;
                cardCashLogInfo.AddDate = DateTime.Now;

                try
                {
                    DataProviderWx.CardCashLogDao.Insert(cardCashLogInfo);
                    DataProviderWx.CardSnDao.Recharge(cardSnInfo.Id, cardSnInfo.UserName, TranslateUtils.ToDecimal(TbRechargeAmount.Text));

                    TbRechargeAmount.Text = string.Empty;

                    SuccessMessage("充值成功！");

                    //PageUtils.CloseModalPage(Page);
                    
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "充值失败！");
                }
            }
        }

        public void Refrush(object sender, EventArgs e)
        {
            _cardId = TranslateUtils.ToInt(DdlCard.SelectedValue);

            PageUtils.Redirect(PageUrl);
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetWeiXinUrl(nameof(ModalCardRecharge), new NameValueCollection
                    {
                        {"publishmentSystemId", PublishmentSystemId.ToString()},
                        {"cardId", DdlCard.SelectedValue},
                        {"cardSnid", _cardSnid.ToString()},
                        {"cardType", DdlCard.SelectedValue}
                    });
                }
                return _pageUrl;
            }
        }
    }
}