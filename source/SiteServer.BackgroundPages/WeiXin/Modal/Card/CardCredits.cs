using System;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;


namespace SiteServer.WeiXin.BackgroundPages.Modal
{
    public class CardCredits : BackgroundBasePage
    {
        public DropDownList ddlCard;
        public PlaceHolder phKeyWord;
        public DropDownList ddlKeyWordType;
        public TextBox tbKeyWord;
        public DropDownList ddlOperatType;
        public TextBox tbCredits;

        private int cardID;
        private int cardSNID;
        public static string GetOpenWindowString(int publishmentSystemID, int cardID, int cardSNID)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("cardID", cardID.ToString());
            arguments.Add("cardSNID", cardSNID.ToString());
            return PageUtilityWX.GetOpenWindowString("会员卡充值", "modal_cardCredits.aspx", arguments, 400, 380);
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
                    ddlCard.Items.Add(new ListItem(cardInfo.CardTitle, cardInfo.ID.ToString()));
                }

                ddlKeyWordType.Items.Add(new ListItem("卡号", "cardSN"));
                ddlKeyWordType.Items.Add(new ListItem("手机", "mobile"));

                ddlOperatType.Items.Add(new ListItem("增加","add"));
                ddlOperatType.Items.Add(new ListItem("减少", "reduce"));

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
                    userInfo = BaiRongDataProvider.UserDao.GetUserInfoByUserName(cardSNInfo!=null? cardSNInfo.UserName:string.Empty);
                }
                else
                {
                    if (ddlKeyWordType.SelectedValue == "cardSN")
                    {
                        cardSNInfo = DataProviderWX.CardSNDAO.GetCardSNInfo(PublishmentSystemID,TranslateUtils.ToInt(ddlCard.SelectedValue), tbKeyWord.Text, string.Empty);
                        userInfo = BaiRongDataProvider.UserDao.GetUserInfoByUserName(cardSNInfo != null ? cardSNInfo.UserName : string.Empty);
                    }
                    else if (ddlKeyWordType.SelectedValue == "mobile")
                    {
                        var userID = BaiRongDataProvider.UserDao.GetUserIdByEmailOrMobile(string.Empty, tbKeyWord.Text);
                        userInfo = BaiRongDataProvider.UserDao.GetUserInfo(userID);
                    }
                }

                if (userInfo == null)
                {
                    FailMessage("会员不存在");
                    return;
                }

                var userCreditsLogInfo = new UserCreditsLogInfo();
                userCreditsLogInfo.UserName = userInfo.UserName;
                userCreditsLogInfo.ProductId = AppManager.WeiXin.AppID;
                userCreditsLogInfo.Num = TranslateUtils.ToInt(tbCredits.Text);
                userCreditsLogInfo.AddDate = DateTime.Now;

                try
                {
                    if (ddlOperatType.SelectedValue == "add")
                    {
                        userCreditsLogInfo.IsIncreased = true;
                        userCreditsLogInfo.Action = "手动添加积分";
                        BaiRongDataProvider.UserCreditsLogDao.Insert(userCreditsLogInfo);
                        BaiRongDataProvider.UserDao.AddCredits(userInfo.UserName, TranslateUtils.ToInt(tbCredits.Text));
                    }
                    else if (ddlOperatType.SelectedValue == "reduce")
                    {
                        userCreditsLogInfo.IsIncreased = false;
                        userCreditsLogInfo.Action = "手动扣除积分";
                        BaiRongDataProvider.UserCreditsLogDao.Insert(userCreditsLogInfo);
                        BaiRongDataProvider.UserDao.AddCredits(userInfo.UserName, -TranslateUtils.ToInt(tbCredits.Text));
                    }
                    tbCredits.Text = string.Empty;
 
                    SuccessMessage("操作成功！");

                    //JsUtils.OpenWindow.CloseModalPage(Page);
                    
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "操作失败！");
                }
             }
        }
    }
}