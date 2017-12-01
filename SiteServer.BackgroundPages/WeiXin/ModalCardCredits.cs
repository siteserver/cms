using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalCardCredits : BasePageCms
    {
        public DropDownList DdlCard;
        public PlaceHolder PhKeyWord;
        public DropDownList DdlKeyWordType;
        public TextBox TbKeyWord;
        public DropDownList DdlOperatType;
        public TextBox TbCredits;

        private int _cardId;
        private int _cardSnid;
        public static string GetOpenWindowString(int publishmentSystemId, int cardId, int cardSnid)
        {
            return PageUtils.GetOpenWindowString("会员卡充值",
                PageUtils.GetWeiXinUrl(nameof(ModalCardCredits), new NameValueCollection
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
                    DdlCard.Items.Add(new ListItem(cardInfo.CardTitle, cardInfo.Id.ToString()));
                }

                DdlKeyWordType.Items.Add(new ListItem("卡号", "cardSN"));
                DdlKeyWordType.Items.Add(new ListItem("手机", "mobile"));

                DdlOperatType.Items.Add(new ListItem("增加","add"));
                DdlOperatType.Items.Add(new ListItem("减少", "reduce"));

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
                    userInfo = BaiRongDataProvider.UserDao.GetUserInfoByUserName(cardSnInfo!=null? cardSnInfo.UserName:string.Empty);
                }
                else
                {
                    if (DdlKeyWordType.SelectedValue == "cardSN")
                    {
                        cardSnInfo = DataProviderWx.CardSnDao.GetCardSnInfo(PublishmentSystemId,TranslateUtils.ToInt(DdlCard.SelectedValue), TbKeyWord.Text, string.Empty);
                        userInfo = BaiRongDataProvider.UserDao.GetUserInfoByUserName(cardSnInfo != null ? cardSnInfo.UserName : string.Empty);
                    }
                    else if (DdlKeyWordType.SelectedValue == "mobile")
                    {
                        var userId = BaiRongDataProvider.UserDao.GetUserIdByEmailOrMobile(string.Empty, TbKeyWord.Text);
                        userInfo = BaiRongDataProvider.UserDao.GetUserInfo(userId);
                    }
                }

                if (userInfo == null)
                {
                    FailMessage("会员不存在");
                    return;
                }

                //var userCreditsLogInfo = new UserCreditsLogInfo();
                //userCreditsLogInfo.UserName = userInfo.UserName;
                //userCreditsLogInfo.ProductId = AppManager.WeiXin.AppID;
                //userCreditsLogInfo.Num = TranslateUtils.ToInt(TbCredits.Text);
                //userCreditsLogInfo.AddDate = DateTime.Now;

                //try
                //{
                //    if (DdlOperatType.SelectedValue == "add")
                //    {
                //        userCreditsLogInfo.IsIncreased = true;
                //        userCreditsLogInfo.Action = "手动添加积分";
                //        BaiRongDataProvider.UserCreditsLogDao.Insert(userCreditsLogInfo);
                //        BaiRongDataProvider.UserDao.AddCredits(userInfo.UserName, TranslateUtils.ToInt(TbCredits.Text));
                //    }
                //    else if (DdlOperatType.SelectedValue == "reduce")
                //    {
                //        userCreditsLogInfo.IsIncreased = false;
                //        userCreditsLogInfo.Action = "手动扣除积分";
                //        BaiRongDataProvider.UserCreditsLogDao.Insert(userCreditsLogInfo);
                //        BaiRongDataProvider.UserDao.AddCredits(userInfo.UserName, -TranslateUtils.ToInt(TbCredits.Text));
                //    }
                //    TbCredits.Text = string.Empty;
 
                //    SuccessMessage("操作成功！");

                //    //PageUtils.CloseModalPage(Page);
                    
                //}
                //catch (Exception ex)
                //{
                //    FailMessage(ex, "操作失败！");
                //}
             }
        }
    }
}