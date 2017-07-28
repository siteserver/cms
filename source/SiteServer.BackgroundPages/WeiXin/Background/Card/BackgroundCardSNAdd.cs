using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundCardSNAdd : BackgroundBasePageWX
    {
        public Literal ltlPageTitle;
        public TextBox tbUserNameList;
        public Literal ltlSelectUser;

        public Button btnSubmit;
        public Button btnReturn;

        private int cardID;

        public static string GetRedirectUrl(int publishmentSystemID, int CardID)
        {
            return PageUtils.GetWXUrl(
                $"background_cardSNAdd.aspx?publishmentSystemID={publishmentSystemID}&CardID={CardID}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            cardID = TranslateUtils.ToInt(GetQueryString("CardID"));

            if (!IsPostBack)
            {
                var pageTitle = "领取会员卡"; 
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Card, pageTitle, AppManager.WeiXin.Permission.WebSite.Card);
                ltlPageTitle.Text = pageTitle;

                if (cardID > 0)
                {

                }
                ltlSelectUser.Text =
                    $@"&nbsp;<a href=""javascript:;"" onclick=""{BaiRong.BackgroundPages.Modal.UserSelect
                        .GetOpenWindowString(tbUserNameList.ClientID)}"" style=""vertical-align:bottom"">选择</a>";

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundCardSN.GetRedirectUrl(PublishmentSystemID, cardID,
                        string.Empty, string.Empty, string.Empty, false)}"";return false");
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            { 
                try
                {
                    if (!string.IsNullOrEmpty(tbUserNameList.Text))
                    {
                        var userNameList = TranslateUtils.StringCollectionToStringList(tbUserNameList.Text);
                        foreach (var userName in userNameList)
                        {
                            var cardSNInfo = new CardSNInfo();
                            cardSNInfo.PublishmentSystemID = PublishmentSystemID;
                            cardSNInfo.CardID = cardID;
                            cardSNInfo.SN = DataProviderWX.CardSNDAO.GetNextCardSN(PublishmentSystemID, cardID);
                            cardSNInfo.Amount = 0;
                            cardSNInfo.UserName = userName;
                            cardSNInfo.IsDisabled = true;
                            cardSNInfo.AddDate = DateTime.Now;

                            var isExist = DataProviderWX.CardSNDAO.isExists(PublishmentSystemID, cardID, userName);
                            if(!isExist)
                            {
                                DataProviderWX.CardSNDAO.Insert(cardSNInfo);
                            }
                          
                        }
                   }

                    LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "领取会员卡成功",
                        $"会员卡:{tbUserNameList.Text}");
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