using System;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.WeiXin.Model;
using SiteServer.WeiXin.Core;
using System.Web.UI.HtmlControls;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundCardSN : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;
         
        public TextBox tbCardSN;
        public TextBox tbUserName;
        public TextBox tbMobile;
        public TextBox tbUserNameList;
         
        public Button btnAdd;
        public Button btnStatus;
        public Button btnExport;
        public Button btnDelete;
        public Button btnReturn;
        public HtmlInputHidden isEntity;

       public int cardID;
      
        public static string GetRedirectUrl(int publishmentSystemID, int cardID, string cardSN, string userName, string mobile, bool isEntity)
        {
            return PageUtils.GetWXUrl("background_cardSN.aspx?PublishmentSystemID=" + publishmentSystemID + "&cardID=" + cardID + "&cardSN=" + cardSN + "&userName=" + userName + "&mobile=" + mobile + "&isEntity=" + isEntity);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            isEntity.Value = Request.QueryString["isEntity"];
            cardID = TranslateUtils.ToInt(Request.QueryString["cardID"]);
           
            if (!string.IsNullOrEmpty(Request.QueryString["Delete"])) 
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWX.CardSNDAO.Delete(PublishmentSystemID, list);

                        SuccessMessage("会员卡删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "会员卡删除失败！");
                    }
                }
            }
            
            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 20;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            spContents.SelectCommand = DataProviderWX.CardSNDAO.GetSelectString(PublishmentSystemID, TranslateUtils.ToInt(Request.QueryString["cardID"]), Request.QueryString["cardSN"], Request.QueryString["userName"], Request.QueryString["mobile"]);
            spContents.SortField = CardSNAttribute.AddDate;
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            {
 
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Card, "会员卡管理", AppManager.WeiXin.Permission.WebSite.Card);
                spContents.DataBind();

                tbCardSN.Text = Request.QueryString["cardSN"];
                tbUserName.Text = Request.QueryString["userName"];
                tbMobile.Text = Request.QueryString["mobile"];

                var urlAdd = BackgroundCardSNAdd.GetRedirectUrl(PublishmentSystemID, cardID);
                btnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");
               
                btnStatus.Attributes.Add("onclick",Modal.CardSNSetting.GetOpenWindowString(PublishmentSystemID,cardID,TranslateUtils.ToBool(isEntity.Value)));
                btnExport.Attributes.Add("onclick",Modal.ExportCardSN.GetOpenWindowString (PublishmentSystemID,cardID));
                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemID,cardID,tbCardSN.Text,tbUserName.Text,tbMobile.Text,TranslateUtils.ToBool(isEntity.Value)), "Delete", "True");
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的会员卡", "此操作将删除所选会员卡，确认吗？"));
 
                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundCard.GetRedirectUrl(PublishmentSystemID)}"";return false;");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var cardSNInfo = new CardSNInfo(e.Item.DataItem);
                var userInfo = BaiRongDataProvider.UserDao.GetUserInfoByUserName(cardSNInfo.UserName);
                var userContactInfo = BaiRongDataProvider.UserContactDao.GetContactInfo(cardSNInfo.UserName);
                
                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlSN = e.Item.FindControl("ltlSN") as Literal;
                var ltlUserName = e.Item.FindControl("ltlUserName") as Literal;
                var ltlMobile = e.Item.FindControl("ltlMobile") as Literal;
                var ltlAmount = e.Item.FindControl("ltlAmount") as Literal;
                var ltlCredits = e.Item.FindControl("ltlCredits") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                var ltlIsDisabled = e.Item.FindControl("ltlIsDisabled") as Literal;
                
                var ltlConsumeUrl = e.Item.FindControl("ltlConsumeUrl") as Literal;
                var ltlRechargeUrl = e.Item.FindControl("ltlRechargeUrl") as Literal;
                var ltlCreditesUrl = e.Item.FindControl("ltlCreditesUrl") as Literal;
               
                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlSN.Text = cardSNInfo.SN;
                if(userInfo!=null)
                {
                    ltlUserName.Text = userInfo.DisplayName;
                    ltlMobile.Text = userInfo.Mobile;
                }
                ltlAmount.Text = cardSNInfo.Amount.ToString();
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(cardSNInfo.AddDate);
                ltlIsDisabled.Text = cardSNInfo.IsDisabled ? "使用" : "冻结";
                ltlConsumeUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{Modal.CardConsume.GetOpenWindowString(PublishmentSystemID,
                        cardSNInfo.CardID, cardSNInfo.ID)}"">消费</a>";
                ltlRechargeUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{Modal.CardRecharge.GetOpenWindowString(PublishmentSystemID,
                        cardSNInfo.CardID, cardSNInfo.ID)}"">充值</a>";
                ltlCreditesUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{Modal.CardCredits.GetOpenWindowString(PublishmentSystemID,
                        cardSNInfo.CardID, cardSNInfo.ID)}"">积分</a>";
                
            } 
        }
         
        public void Search_OnClick(object sender, EventArgs e)
        {
            var btn = sender as Button;

            if (btn.Text == "微信会员")
            {
                isEntity.Value = "false";
             }
            else if (btn.Text == "实体卡会员")
            {
                isEntity.Value = "true";
            }
              
            PageUtils.Redirect(PageUrl);
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    if (TranslateUtils.ToBool(isEntity.Value))
                    {
                        _pageUrl = PageUtils.GetWXUrl(
                            $"background_cardEntitySN.aspx?PublishmentSystemID={PublishmentSystemID}&cardID={cardID}&cardSN={tbCardSN.Text}&userName={tbUserName.Text}&mobile={tbMobile.Text}&isEntity={isEntity.Value}");
                    }
                    else
                    {
                        _pageUrl = PageUtils.GetWXUrl(
                            $"background_cardSN.aspx?PublishmentSystemID={PublishmentSystemID}&cardID={cardID}&cardSN={tbCardSN.Text}&userName={tbUserName.Text}&mobile={tbMobile.Text}&isEntity={isEntity.Value}");
                    }
                   
                }
                return _pageUrl;
            }
        }
    }
}
