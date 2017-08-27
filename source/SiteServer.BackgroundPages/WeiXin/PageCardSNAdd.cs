using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageCardSnAdd : BasePageCms
    {
        public Literal LtlPageTitle;
        public TextBox TbUserNameList;
        public Literal LtlSelectUser;

        public Button BtnSubmit;
        public Button BtnReturn;

        private int _cardId;

        public static string GetRedirectUrl(int publishmentSystemId, int cardId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageCardSnAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"cardId", cardId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemId");
            _cardId = Body.GetQueryInt("CardID");

            if (!IsPostBack)
            {
                var pageTitle = "领取会员卡";
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdCard, pageTitle, AppManager.WeiXin.Permission.WebSite.Card);
                LtlPageTitle.Text = pageTitle;

                if (_cardId > 0)
                {

                }
                LtlSelectUser.Text =
                    $@"&nbsp;<a href=""javascript:;"" onclick=""{ModalUserSelect.GetOpenWindowString(TbUserNameList.ClientID)}"" style=""vertical-align:bottom"">选择</a>";

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageCardSn.GetRedirectUrl(PublishmentSystemId, _cardId,
                        string.Empty, string.Empty, string.Empty, false)}"";return false");
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            { 
                try
                {
                    if (!string.IsNullOrEmpty(TbUserNameList.Text))
                    {
                        var userNameList = TranslateUtils.StringCollectionToStringList(TbUserNameList.Text);
                        foreach (var userName in userNameList)
                        {
                            var cardSnInfo = new CardSnInfo();
                            cardSnInfo.PublishmentSystemId = PublishmentSystemId;
                            cardSnInfo.CardId = _cardId;
                            cardSnInfo.Sn = DataProviderWx.CardSnDao.GetNextCardSn(PublishmentSystemId, _cardId);
                            cardSnInfo.Amount = 0;
                            cardSnInfo.UserName = userName;
                            cardSnInfo.IsDisabled = true;
                            cardSnInfo.AddDate = DateTime.Now;

                            var isExist = DataProviderWx.CardSnDao.IsExists(PublishmentSystemId, _cardId, userName);
                            if(!isExist)
                            {
                                DataProviderWx.CardSnDao.Insert(cardSnInfo);
                            }
                          
                        }
                   }

                    LogUtils.AddAdminLog(Body.AdministratorName, "领取会员卡成功", $"会员卡:{TbUserNameList.Text}");
                    SuccessMessage("领取会员卡成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "领取会员卡失败！");
                }
            }

        }
    }
}