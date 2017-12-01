using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageCardRechargeLog : BasePageCms
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
            return PageUtils.GetWeiXinUrl(nameof(PageCardRechargeLog), new NameValueCollection
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
                        DataProviderWx.CardCashLogDao.Delete(PublishmentSystemId, list);

                        SuccessMessage("充值记录删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "充值记录删除失败！");
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 20;
            SpContents.SelectCommand = DataProviderWx.CardCashLogDao.GetSelectString(PublishmentSystemId, ECashType.Recharge, TranslateUtils.ToInt(Request.QueryString["cardID"]), Request.QueryString["cardSN"], Request.QueryString["userName"], Request.QueryString["mobile"]);
            SpContents.SortField = CardCashLogAttribute.AddDate;
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdCard, "会员卡充值管理", AppManager.WeiXin.Permission.WebSite.Card);
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
                var cardCashLogInfo = new CardCashLogInfo(e.Item.DataItem);
                var cardSnInfo = DataProviderWx.CardSnDao.GetCardSnInfo(cardCashLogInfo.CardSnId);
                var userInfo = BaiRongDataProvider.UserDao.GetUserInfoByUserName(cardCashLogInfo.UserName);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlSn = e.Item.FindControl("ltlSN") as Literal;
                var ltlUserName = e.Item.FindControl("ltlUserName") as Literal;
                var ltlMobile = e.Item.FindControl("ltlMobile") as Literal;
                var ltlAmount = e.Item.FindControl("ltlAmount") as Literal;
                var ltlBeforeAmount = e.Item.FindControl("ltlBeforeAmount") as Literal;
                var ltlAfterAmount = e.Item.FindControl("ltlAfterAmount") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                var ltlOperator = e.Item.FindControl("ltlOperator") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlAmount.Text = cardCashLogInfo.Amount.ToString();
                ltlBeforeAmount.Text = (cardCashLogInfo.CurAmount - cardCashLogInfo.Amount).ToString();
                ltlAfterAmount.Text = cardCashLogInfo.CurAmount.ToString();
                ltlOperator.Text = cardCashLogInfo.Operator;
                 
                ltlUserName.Text = userInfo != null ? userInfo.UserName : string.Empty;
                ltlMobile.Text = userInfo != null ? userInfo.Mobile : string.Empty;

                ltlSn.Text = cardSnInfo != null ? cardSnInfo.Sn : string.Empty;
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(cardCashLogInfo.AddDate);
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
