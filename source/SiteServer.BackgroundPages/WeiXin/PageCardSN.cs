using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageCardSn : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;
         
        public TextBox TbCardSn;
        public TextBox TbUserName;
        public TextBox TbMobile;
        public TextBox TbUserNameList;
         
        public Button BtnAdd;
        public Button BtnStatus;
        public Button BtnExport;
        public Button BtnDelete;
        public Button BtnReturn;
        public HtmlInputHidden IsEntity;

       public int CardId;
      
        public static string GetRedirectUrl(int publishmentSystemId, int cardId, string cardSn, string userName, string mobile, bool isEntity)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageCardSn), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"cardId", cardId.ToString()},
                {"cardSn", cardSn},
                {"userName", userName},
                {"mobile", mobile},
                {"isEntity", isEntity.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            IsEntity.Value = Request.QueryString["isEntity"];
            CardId = TranslateUtils.ToInt(Request.QueryString["cardID"]);
           
            if (!string.IsNullOrEmpty(Request.QueryString["Delete"])) 
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWx.CardSnDao.Delete(PublishmentSystemId, list);

                        SuccessMessage("会员卡删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "会员卡删除失败！");
                    }
                }
            }
            
            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 20;
            SpContents.SelectCommand = DataProviderWx.CardSnDao.GetSelectString(PublishmentSystemId, TranslateUtils.ToInt(Request.QueryString["cardID"]), Request.QueryString["cardSN"], Request.QueryString["userName"], Request.QueryString["mobile"]);
            SpContents.SortField = CardSnAttribute.AddDate;
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
 
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdCard, "会员卡管理", AppManager.WeiXin.Permission.WebSite.Card);
                SpContents.DataBind();

                TbCardSn.Text = Request.QueryString["cardSN"];
                TbUserName.Text = Request.QueryString["userName"];
                TbMobile.Text = Request.QueryString["mobile"];

                var urlAdd = PageCardSnAdd.GetRedirectUrl(PublishmentSystemId, CardId);
                BtnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");
               
                BtnStatus.Attributes.Add("onclick", ModalCardSnSetting.GetOpenWindowString(PublishmentSystemId,CardId,TranslateUtils.ToBool(IsEntity.Value)));
                BtnExport.Attributes.Add("onclick", ModalExportCardSn.GetOpenWindowString (PublishmentSystemId,CardId));
                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId,CardId,TbCardSn.Text,TbUserName.Text,TbMobile.Text,TranslateUtils.ToBool(IsEntity.Value)), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的会员卡", "此操作将删除所选会员卡，确认吗？"));

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageCard.GetRedirectUrl(PublishmentSystemId)}"";return false;");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var cardSnInfo = new CardSnInfo(e.Item.DataItem);
                var userInfo = BaiRongDataProvider.UserDao.GetUserInfoByUserName(cardSnInfo.UserName);
                //var userContactInfo = BaiRongDataProvider.UserContactDao.GetContactInfo(cardSnInfo.UserName);
                
                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlSn = e.Item.FindControl("ltlSN") as Literal;
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
                ltlSn.Text = cardSnInfo.Sn;
                if(userInfo!=null)
                {
                    ltlUserName.Text = userInfo.DisplayName;
                    ltlMobile.Text = userInfo.Mobile;
                }
                ltlAmount.Text = cardSnInfo.Amount.ToString();
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(cardSnInfo.AddDate);
                ltlIsDisabled.Text = cardSnInfo.IsDisabled ? "使用" : "冻结";
                ltlConsumeUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalCardConsume.GetOpenWindowString(PublishmentSystemId,
                        cardSnInfo.CardId, cardSnInfo.Id)}"">消费</a>";
                ltlRechargeUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalCardRecharge.GetOpenWindowString(PublishmentSystemId,
                        cardSnInfo.CardId, cardSnInfo.Id)}"">充值</a>";
                ltlCreditesUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalCardCredits.GetOpenWindowString(PublishmentSystemId,
                        cardSnInfo.CardId, cardSnInfo.Id)}"">积分</a>";

            } 
        }
         
        public void Search_OnClick(object sender, EventArgs e)
        {
            var btn = sender as Button;

            if (btn.Text == "微信会员")
            {
                IsEntity.Value = "false";
            }
            else if (btn.Text == "实体卡会员")
            {
                IsEntity.Value = "true";
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
                    if (TranslateUtils.ToBool(IsEntity.Value))
                    {
                        PageCardEntitySn.GetRedirectUrl(PublishmentSystemId, CardId, TbCardSn.Text, TbUserName.Text, TbMobile.Text, TranslateUtils.ToBool(IsEntity.Value));
                    }
                    else
                    {
                        _pageUrl = GetRedirectUrl(PublishmentSystemId, CardId, TbCardSn.Text, TbUserName.Text, TbMobile.Text, TranslateUtils.ToBool(IsEntity.Value));
                    }
                   
                }
                return _pageUrl;
            }
        }
    }
}
