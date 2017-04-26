using System;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.WeiXin.Model;
using SiteServer.WeiXin.Core;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundCardConsumeLog : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public DropDownList ddlCard;
        public TextBox tbCardSN;
        public TextBox tbUserName;
        public TextBox tbMobile;
         
        public Button btnDelete;
        public Button btnReturn;
        public static string GetRedirectUrl(int publishmentSystemID,string cardSN,string userName,string mobile)
        {
            return PageUtils.GetWXUrl($"background_cardConsumeLog.aspx?PublishmentSystemID={publishmentSystemID}&cardSN={cardSN}&userName={userName}&mobile={mobile}");
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
                        DataProviderWX.CardCashLogDAO.Delete(PublishmentSystemID, list);

                        SuccessMessage("消费记录删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "消费记录删除失败！");
                    }
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 20;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            spContents.SelectCommand = DataProviderWX.CardCashLogDAO.GetSelectString(PublishmentSystemID, ECashType.Consume, TranslateUtils.ToInt(Request.QueryString["cardID"]), Request.QueryString["cardSN"], Request.QueryString["userName"], Request.QueryString["mobile"]);
            spContents.SortField = CardCashLogAttribute.AddDate;
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            {
               
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Card, "会员消费管理", AppManager.WeiXin.Permission.WebSite.Card);
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
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的消费记录", "此操作将删除所选消费记录，确认吗？"));

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundNavTransaction.GetRedirectUrl(PublishmentSystemID)}"";return false;");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var cardCashLogInfo = new CardCashLogInfo(e.Item.DataItem);
                var cardSNInfo = DataProviderWX.CardSNDAO.GetCardSNInfo(cardCashLogInfo.CardSNID);
                var userInfo = BaiRongDataProvider.UserDao.GetUserInfoByUserName(cardCashLogInfo.UserName);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlSN = e.Item.FindControl("ltlSN") as Literal;
                var ltlUserName = e.Item.FindControl("ltlUserName") as Literal;
                var ltlMobile = e.Item.FindControl("ltlMobile") as Literal;
                var ltlType = e.Item.FindControl("ltlType") as Literal;
                var ltlAmount = e.Item.FindControl("ltlAmount") as Literal;
                var ltlBeforeAmount = e.Item.FindControl("ltlBeforeAmount") as Literal;
                var ltlAfterAmount = e.Item.FindControl("ltlAfterAmount") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                var ltlOperator = e.Item.FindControl("ltlOperator") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlType.Text = EConsumeTypeUtils.GetText(EConsumeTypeUtils.GetEnumType(cardCashLogInfo.ConsumeType));
                ltlAmount.Text = cardCashLogInfo.Amount.ToString();
                if (EConsumeTypeUtils.GetEnumType(cardCashLogInfo.ConsumeType) == EConsumeType.Cash)
                {
                    ltlBeforeAmount.Text = cardCashLogInfo.CurAmount.ToString();
                }
                else
                {
                    ltlBeforeAmount.Text = (cardCashLogInfo.CurAmount + cardCashLogInfo.Amount).ToString();
                }
                ltlAfterAmount.Text = cardCashLogInfo.CurAmount.ToString();
                ltlUserName.Text = userInfo != null ? userInfo.UserName : string.Empty;
                ltlMobile.Text = userInfo != null ? userInfo.Mobile : string.Empty;
                ltlSN.Text = cardSNInfo != null ? cardSNInfo.SN : string.Empty;
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(cardCashLogInfo.AddDate);
                ltlOperator.Text = cardCashLogInfo.Operator;
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
                        $"background_cardConsumeLog.aspx?PublishmentSystemID={PublishmentSystemID}&cardID={ddlCard.SelectedValue}&cardSN={tbCardSN.Text}&userName={tbUserName.Text}&mobile={tbMobile.Text}");
                }
                return _pageUrl;
            }
        }
    }
}
