using System;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.BackgroundPages;

using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using System.Web.UI;

namespace SiteServer.WeiXin.BackgroundPages.Modal
{
    public class CardSNSetting : BackgroundBasePage
	{ 
        public DropDownList ddlIsDisabled;
        public DropDownList ddlIsBinding;
        public Control IsDisabledRow;
        public Control IsBindingRow;

        private int cardID;
        private bool isEntity;
        public static string GetOpenWindowString(int publishmentSystemID,int cardID,bool isEntity)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("cardID",cardID.ToString());
            arguments.Add("isEntity",isEntity.ToString());
            return PageUtilityWX.GetOpenWindowStringWithCheckBoxValue("设置会员卡状态", "modal_cardSNSetting.aspx", arguments, "IDCollection", "请选择需要设置的会员卡", 400, 300);
        }

        public static string GetOpenWindowString(int publishmentSystemID, bool isEntity)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("isEntity", isEntity.ToString());
            return PageUtilityWX.GetOpenWindowStringWithCheckBoxValue("设置实体卡状态", "modal_cardSNSetting.aspx", arguments, "IDCollection", "请选择需要设置的会员卡", 400, 300);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            cardID = TranslateUtils.ToInt(Request.QueryString["cardID"]);
            isEntity = TranslateUtils.ToBool(Request.QueryString["isEntity"]);

			if (!IsPostBack)
			{
                ddlIsDisabled.Items.Add(new ListItem("冻结", "false"));
                ddlIsDisabled.Items.Add(new ListItem("解除冻结", "true"));

                ddlIsBinding.Items.Add(new ListItem("绑定", "true"));
                ddlIsBinding.Items.Add(new ListItem("解除绑定", "false"));

                if (!isEntity)
                { 
                    IsBindingRow.Visible = false;
                }
                else
                {
                    IsDisabledRow.Visible = false;
                }
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                if (!isEntity)
                {
                    DataProviderWX.CardSNDAO.UpdateStatus(cardID, TranslateUtils.ToBool(ddlIsDisabled.SelectedValue), TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]));
                }
                else
                {
                    var cardEntitySNIDList = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                    if (cardEntitySNIDList.Count > 0)
                    {
                        for (var i = 0; i < cardEntitySNIDList.Count; i++)
                        {
                            var cardEntitySNInfo = DataProviderWX.CardEntitySNDAO.GetCardEntitySNInfo(cardEntitySNIDList[i]);

                            var userID = BaiRongDataProvider.UserDao.GetUserIdByEmailOrMobile(string.Empty, cardEntitySNInfo.Mobile);
                            var userInfo = BaiRongDataProvider.UserDao.GetUserInfo(userID);
                            if (userInfo != null)
                            {
                                var cardSNInfo = DataProviderWX.CardSNDAO.GetCardSNInfo(PublishmentSystemID,cardID, string.Empty, userInfo.UserName);
                                 
                                var cardCashLogInfo = new CardCashLogInfo();
                                cardCashLogInfo.PublishmentSystemID = PublishmentSystemID;
                                cardCashLogInfo.UserName = userInfo.UserName;
                                cardCashLogInfo.CardID = cardSNInfo.CardID;
                                cardCashLogInfo.CardSNID = cardSNInfo.ID;
                                cardCashLogInfo.Amount = cardEntitySNInfo.Amount;
                                cardCashLogInfo.CurAmount += cardEntitySNInfo.Amount; ;
                                cardCashLogInfo.CashType = ECashTypeUtils.GetValue(ECashType.Recharge);
                                cardCashLogInfo.Operator = AdminManager.Current.UserName;
                                cardCashLogInfo.Description = "绑定实体卡充值";
                                cardCashLogInfo.AddDate = DateTime.Now;

                                var userCreditsLogInfo = new UserCreditsLogInfo();
                                userCreditsLogInfo.UserName = userInfo.UserName;
                                userCreditsLogInfo.ProductId = AppManager.WeiXin.AppID;
                                userCreditsLogInfo.Num = cardEntitySNInfo.Credits;
                                userCreditsLogInfo.IsIncreased = true;
                                userCreditsLogInfo.Action = "绑定实体卡添加积分";
                                userCreditsLogInfo.AddDate = DateTime.Now;
                                 

                                if (!cardEntitySNInfo.IsBinding)
                                { 
                                    cardEntitySNInfo.IsBinding = true;
                                    DataProviderWX.CardEntitySNDAO.Update(cardEntitySNInfo);

                                    DataProviderWX.CardCashLogDAO.Insert(cardCashLogInfo);
                                    DataProviderWX.CardSNDAO.Recharge(cardSNInfo.ID, userInfo.UserName, cardEntitySNInfo.Amount);
                                     
                                    BaiRongDataProvider.UserCreditsLogDao.Insert(userCreditsLogInfo);
                                    BaiRongDataProvider.UserDao.AddCredits(userInfo.UserName, cardEntitySNInfo.Credits);
                                }
                            }
                        }
                     }
                 }

                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "失败：" + ex.Message);
            }

            if (isChanged)
            {
                JsUtils.OpenWindow.CloseModalPage(Page);
            }
		}
	}
}
