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
    public class ModalCardConsume : BasePageCms
    {
        public DropDownList DdlCard;
        public PlaceHolder PhKeyWord;
        public DropDownList DdlKeyWordType;
        public TextBox TbKeyWord;
        public TextBox TbConsumeAmount;
        public DropDownList DdlConsumeType;
        public DropDownList DdlOperator;

        private int _cardId;
        private int _cardSnid;
         
        public static string GetOpenWindowString(int publishmentSystemId, int cardId, int cardSnid)
        {
            return PageUtils.GetOpenWindowString("会员消费",
                PageUtils.GetWeiXinUrl(nameof(ModalCardConsume), new NameValueCollection
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

                EConsumeTypeUtils.AddListItems(DdlConsumeType);
                ControlUtils.SelectListItems(DdlConsumeType, EConsumeTypeUtils.GetValue(EConsumeType.CardAmount));

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
                if (Body.IsQueryExists("cardType"))
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

                var consumeType = EConsumeTypeUtils.GetEnumType(DdlConsumeType.SelectedValue);

                if (consumeType == EConsumeType.CardAmount)
                {
                    var amount = DataProviderWx.CardSnDao.GetAmount(cardSnInfo.Id, cardSnInfo.UserName);
                    if (amount < TranslateUtils.ToDecimal(TbConsumeAmount.Text))
                    {
                        FailMessage("会员卡余额不足");
                        return;
                    }
                }

                var cardCashLogInfo = new CardCashLogInfo();
                cardCashLogInfo.PublishmentSystemId = PublishmentSystemId;
                cardCashLogInfo.UserName = cardSnInfo.UserName;
                cardCashLogInfo.CardId = cardSnInfo.CardId;
                cardCashLogInfo.CardSnId = cardSnInfo.Id;
                cardCashLogInfo.Amount = TranslateUtils.ToDecimal(TbConsumeAmount.Text);
                cardCashLogInfo.CurAmount = cardSnInfo.Amount;
                if (consumeType == EConsumeType.CardAmount)
                {
                    cardCashLogInfo.CurAmount = cardSnInfo.Amount - TranslateUtils.ToInt(TbConsumeAmount.Text);
                }

                cardCashLogInfo.CashType = ECashTypeUtils.GetValue(ECashType.Consume);
                cardCashLogInfo.ConsumeType = DdlConsumeType.SelectedValue;
                cardCashLogInfo.Operator = DdlOperator.SelectedValue;
                cardCashLogInfo.AddDate = DateTime.Now;

                try
                {
                    DataProviderWx.CardCashLogDao.Insert(cardCashLogInfo);

                    if (consumeType == EConsumeType.CardAmount)
                    {
                        DataProviderWx.CardSnDao.Consume(cardSnInfo.Id, cardSnInfo.UserName, TranslateUtils.ToDecimal(TbConsumeAmount.Text));

                        if (PublishmentSystemInfo.Additional.WxCardIsClaimCardCredits)
                        {
                            var amount = TranslateUtils.ToDecimal(TbConsumeAmount.Text);
                            var consumeAmount = PublishmentSystemInfo.Additional.WxCardConsumeAmount;
                            var giveCredits = PublishmentSystemInfo.Additional.WxCardGiveCredits;

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
                    TbConsumeAmount.Text = string.Empty;

                    SuccessMessage("操作成功！");

                    //PageUtils.CloseModalPage(Page);
                   
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "操作失败！");
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
                    _pageUrl = PageUtils.GetWeiXinUrl(nameof(ModalCardConsume), new NameValueCollection
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