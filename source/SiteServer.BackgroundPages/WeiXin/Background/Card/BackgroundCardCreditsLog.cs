using System;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.WeiXin.Core;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundCardCreditsLog : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public DropDownList ddlCard;
        public TextBox tbCardSN;
        public TextBox tbUserName;
        public TextBox tbMobile;
 
        public Button btnDelete;
        public Button btnReturn;

        public int cardID;
        public static string GetRedirectUrl(int publishmentSystemID, string cardSN, string userName, string mobile)
        {
            return PageUtils.GetWXUrl("background_cardCreditsLog.aspx?PublishmentSystemID=" + publishmentSystemID + "&cardSN=" + cardSN + "&userName=" + userName + "&mobile=" + mobile);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
             
            if (!string.IsNullOrEmpty(Request.QueryString["Delete"])) 
            {
                var list = TranslateUtils.StringCollectionToIntArrayList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        //BaiRongDataProvider.UserCreditsLogDao.Delete(list);

                        SuccessMessage("积分记录删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "积分记录删除失败！");
                    }
                }
            }

            var userNameArrayList =BaiRongDataProvider.UserDao.GetUserNameArrayList(Request.QueryString["mobile"],0,0,true);
            var theUserNameArrayList = DataProviderWX.CardSNDAO.GetUserNameArrayList(PublishmentSystemID, TranslateUtils.ToInt(Request.QueryString["cardID"]), Request.QueryString["cardSN"],Request.QueryString["userName"]);
            if (userNameArrayList.Count > 0)
            {
                if (theUserNameArrayList.Count <= 0)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["mobile"]))
                    {
                        theUserNameArrayList = userNameArrayList;
                    }
                }
                else
                {
                    foreach (string userName in userNameArrayList)
                    {
                        if (!string.IsNullOrEmpty(Request.QueryString["mobile"]))
                        {
                            if (string.IsNullOrEmpty(Request.QueryString["cardSN"]) && string.IsNullOrEmpty(Request.QueryString["userName"]))
                            {
                                theUserNameArrayList = userNameArrayList;
                            }
                            else
                            {
                                if (!theUserNameArrayList.Contains(userName))
                                {
                                    theUserNameArrayList.Add(userName);
                                }
                            }
                        }
                    }
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 20;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            //spContents.SelectCommand = BaiRongDataProvider.UserCreditsLogDao.GetSqlString(AppManager.WeiXin.AppID, theUserNameArrayList);
            spContents.SortField = "AddDate";
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            { 
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Card, "会员积分管理", AppManager.WeiXin.Permission.WebSite.Card);
               
                var cardInfoList = DataProviderWX.CardDAO.GetCardInfoList(PublishmentSystemID);
                foreach (var cardInfo in cardInfoList)
                {
                    ddlCard.Items.Add(new ListItem(cardInfo.CardTitle, cardInfo.ID.ToString()));
                }
                  
                spContents.DataBind();

                ddlCard.SelectedValue = Request.QueryString["cardID"];
                tbCardSN.Text = Request.QueryString["cardSN"];
                tbUserName.Text = Request.QueryString["userName"];
                tbMobile.Text = Request.QueryString["mobile"];

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemID, tbCardSN.Text, tbUserName.Text, tbMobile.Text), "Delete", "True");
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的充值记录", "此操作将删除所选充值记录，确认吗？"));

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundNavTransaction.GetRedirectUrl(PublishmentSystemID)}"";return false;");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var id = TranslateUtils.EvalInt(e.Item.DataItem, "ID");
                var userName = TranslateUtils.EvalString(e.Item.DataItem, "UserName");
                var num = TranslateUtils.EvalInt(e.Item.DataItem, "Num");
                var action = TranslateUtils.EvalString(e.Item.DataItem, "Action");
                var addDate = TranslateUtils.EvalDateTime(e.Item.DataItem, "AddDate");

                var cardSNInfo = DataProviderWX.CardSNDAO.GetCardSNInfo(PublishmentSystemID,0, string.Empty, userName);
                var userInfo = BaiRongDataProvider.UserDao.GetUserInfoByUserName(userName);
                  
                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlSN = e.Item.FindControl("ltlSN") as Literal;
                var ltlUserName = e.Item.FindControl("ltlUserName") as Literal;
                var ltlMobile = e.Item.FindControl("ltlMobile") as Literal;
                var ltlNum = e.Item.FindControl("ltlNum") as Literal;
                var ltlAction = e.Item.FindControl("ltlAction") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
              
               
                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlSN.Text = cardSNInfo != null ? cardSNInfo.SN : string.Empty ;
                ltlUserName.Text = userInfo!=null? userInfo.UserName:string.Empty;
                ltlMobile.Text =userInfo!=null? userInfo.Mobile:string.Empty;
                ltlNum.Text =num.ToString();
                ltlAction.Text =action;
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(addDate);
                
            } 
        }
         
        public void Search_OnClick(object sender, EventArgs e)
        {
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
                        $"background_cardCreditsLog.aspx?PublishmentSystemID={PublishmentSystemID}&cardID={ddlCard.SelectedValue}&cardSN={tbCardSN.Text}&userName={tbUserName.Text}&mobile={tbMobile.Text}");
                }
                return _pageUrl;
            }
        }
    }
}
