using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.WeiXin.Data;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageCardCreditsLog : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public DropDownList DdlCard;
        public TextBox TbCardSn;
        public TextBox TbUserName;
        public TextBox TbMobile;
 
        public Button BtnDelete;
        public Button BtnReturn;

        public int CardId;
        public static string GetRedirectUrl(int publishmentSystemId, int cardId, string cardSn, string userName, string mobile)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageCardCreditsLog), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"cardId", cardId.ToString()},
                {"cardSn", cardSn},
                {"userName", userName},
                {"mobile", mobile}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
             
            if (!string.IsNullOrEmpty(Request.QueryString["Delete"])) 
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
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

            var userNameList =BaiRongDataProvider.UserDao.GetUserNameList(Request.QueryString["mobile"],0,0,true);
            var theUserNameList = DataProviderWx.CardSnDao.GetUserNameList(PublishmentSystemId, TranslateUtils.ToInt(Request.QueryString["cardID"]), Request.QueryString["cardSN"],Request.QueryString["userName"]);
            if (userNameList.Count > 0)
            {
                if (theUserNameList.Count <= 0)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["mobile"]))
                    {
                        theUserNameList = userNameList;
                    }
                }
                else
                {
                    foreach (string userName in userNameList)
                    {
                        if (!string.IsNullOrEmpty(Request.QueryString["mobile"]))
                        {
                            if (string.IsNullOrEmpty(Request.QueryString["cardSN"]) && string.IsNullOrEmpty(Request.QueryString["userName"]))
                            {
                                theUserNameList = userNameList;
                            }
                            else
                            {
                                if (!theUserNameList.Contains(userName))
                                {
                                    theUserNameList.Add(userName);
                                }
                            }
                        }
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 20;
            //spContents.SelectCommand = BaiRongDataProvider.UserCreditsLogDao.GetSqlString(AppManager.WeiXin.AppID, theUserNameArrayList);
            SpContents.SortField = "AddDate";
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            { 
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdCard, "会员积分管理", AppManager.WeiXin.Permission.WebSite.Card);
               
                var cardInfoList = DataProviderWx.CardDao.GetCardInfoList(PublishmentSystemId);
                foreach (var cardInfo in cardInfoList)
                {
                    DdlCard.Items.Add(new ListItem(cardInfo.CardTitle, cardInfo.Id.ToString()));
                }
                  
                SpContents.DataBind();

                DdlCard.SelectedValue = Request.QueryString["cardID"];
                TbCardSn.Text = Request.QueryString["cardSN"];
                TbUserName.Text = Request.QueryString["userName"];
                TbMobile.Text = Request.QueryString["mobile"];

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId, 0, TbCardSn.Text, TbUserName.Text, TbMobile.Text), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的充值记录", "此操作将删除所选充值记录，确认吗？"));

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageNavTransaction.GetRedirectUrl(PublishmentSystemId)}"";return false;");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var id = SqlUtils.EvalInt(e.Item.DataItem, "ID");
                var userName = SqlUtils.EvalString(e.Item.DataItem, "UserName");
                var num = SqlUtils.EvalInt(e.Item.DataItem, "Num");
                var action = SqlUtils.EvalString(e.Item.DataItem, "Action");
                var addDate = SqlUtils.EvalDateTime(e.Item.DataItem, "AddDate");

                var cardSnInfo = DataProviderWx.CardSnDao.GetCardSnInfo(PublishmentSystemId,0, string.Empty, userName);
                var userInfo = BaiRongDataProvider.UserDao.GetUserInfoByUserName(userName);
                  
                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlSn = e.Item.FindControl("ltlSN") as Literal;
                var ltlUserName = e.Item.FindControl("ltlUserName") as Literal;
                var ltlMobile = e.Item.FindControl("ltlMobile") as Literal;
                var ltlNum = e.Item.FindControl("ltlNum") as Literal;
                var ltlAction = e.Item.FindControl("ltlAction") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
              
               
                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlSn.Text = cardSnInfo != null ? cardSnInfo.Sn : string.Empty ;
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
                    _pageUrl = GetRedirectUrl(PublishmentSystemId, TranslateUtils.ToInt(DdlCard.SelectedValue), TbCardSn.Text, TbUserName.Text, TbMobile.Text);
                }
                return _pageUrl;
            }
        }
    }
}
