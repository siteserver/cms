using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalCardSnSetting : BasePageCms
    { 
        public DropDownList DdlIsDisabled;
        public DropDownList DdlIsBinding;
        public Control IsDisabledRow;
        public Control IsBindingRow;

        private int _cardId;
        private bool _isEntity;
        public static string GetOpenWindowString(int publishmentSystemId,int cardId, bool isEntity)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("设置会员卡状态",
                PageUtils.GetWeiXinUrl(nameof(ModalCardSnSetting), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"cardId", cardId.ToString()},
                    {"isEntity", isEntity.ToString()}
                }), "IDCollection", "请选择需要设置的会员卡", 400, 300);
        }

        public static string GetOpenWindowString(int publishmentSystemId, bool isEntity)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("设置会员卡状态",
                PageUtils.GetWeiXinUrl(nameof(ModalCardSnSetting), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"isEntity", isEntity.ToString()}
                }), "IDCollection", "请选择需要设置的会员卡", 400, 300);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            _cardId = TranslateUtils.ToInt(Request.QueryString["cardID"]);
            _isEntity = TranslateUtils.ToBool(Request.QueryString["isEntity"]);

			if (!IsPostBack)
			{
                DdlIsDisabled.Items.Add(new ListItem("冻结", "false"));
                DdlIsDisabled.Items.Add(new ListItem("解除冻结", "true"));

                DdlIsBinding.Items.Add(new ListItem("绑定", "true"));
                DdlIsBinding.Items.Add(new ListItem("解除绑定", "false"));

                if (!_isEntity)
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
                if (!_isEntity)
                {
                    DataProviderWx.CardSnDao.UpdateStatus(_cardId, TranslateUtils.ToBool(DdlIsDisabled.SelectedValue), TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]));
                }
                else
                {
                    var cardEntitySnidList = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                    if (cardEntitySnidList.Count > 0)
                    {
                        for (var i = 0; i < cardEntitySnidList.Count; i++)
                        {
                            var cardEntitySnInfo = DataProviderWx.CardEntitySnDao.GetCardEntitySnInfo(cardEntitySnidList[i]);

                            var userId = BaiRongDataProvider.UserDao.GetUserIdByEmailOrMobile(string.Empty, cardEntitySnInfo.Mobile);
                            var userInfo = BaiRongDataProvider.UserDao.GetUserInfo(userId);
                            if (userInfo != null)
                            {
                                var cardSnInfo = DataProviderWx.CardSnDao.GetCardSnInfo(PublishmentSystemId,_cardId, string.Empty, userInfo.UserName);
                                 
                                var cardCashLogInfo = new CardCashLogInfo();
                                cardCashLogInfo.PublishmentSystemId = PublishmentSystemId;
                                cardCashLogInfo.UserName = userInfo.UserName;
                                cardCashLogInfo.CardId = cardSnInfo.CardId;
                                cardCashLogInfo.CardSnId = cardSnInfo.Id;
                                cardCashLogInfo.Amount = cardEntitySnInfo.Amount;
                                cardCashLogInfo.CurAmount += cardEntitySnInfo.Amount; ;
                                cardCashLogInfo.CashType = ECashTypeUtils.GetValue(ECashType.Recharge);
                                cardCashLogInfo.Operator = Body.AdministratorName;
                                cardCashLogInfo.Description = "绑定实体卡充值";
                                cardCashLogInfo.AddDate = DateTime.Now;

                                //var userCreditsLogInfo = new UserCreditsLogInfo();
                                //userCreditsLogInfo.UserName = userInfo.UserName;
                                //userCreditsLogInfo.ProductId = AppManager.WeiXin.AppID;
                                //userCreditsLogInfo.Num = cardEntitySnInfo.Credits;
                                //userCreditsLogInfo.IsIncreased = true;
                                //userCreditsLogInfo.Action = "绑定实体卡添加积分";
                                //userCreditsLogInfo.AddDate = DateTime.Now;
                                 

                                //if (!cardEntitySnInfo.IsBinding)
                                //{ 
                                //    cardEntitySnInfo.IsBinding = true;
                                //    DataProviderWx.CardEntitySndao.Update(cardEntitySnInfo);

                                //    DataProviderWx.CardCashLogDao.Insert(cardCashLogInfo);
                                //    DataProviderWx.CardSnDao.Recharge(cardSnInfo.Id, userInfo.UserName, cardEntitySnInfo.Amount);
                                     
                                //    BaiRongDataProvider.UserCreditsLogDao.Insert(userCreditsLogInfo);
                                //    BaiRongDataProvider.UserDao.AddCredits(userInfo.UserName, cardEntitySnInfo.Credits);
                                //}
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
                PageUtils.CloseModalPage(Page);
            }
		}
	}
}
