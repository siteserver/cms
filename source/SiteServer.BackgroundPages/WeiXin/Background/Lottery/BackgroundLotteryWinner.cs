using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;


namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundLotteryWinner : BackgroundBasePage
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button btnDelete;
        public Button btnSetting;
        public Button btnExport;
        public Button btnReturn;

        private ELotteryType lotteryType;
        private int lotteryID;
        private int awardID;
        private string returnUrl;
        private Dictionary<int, LotteryAwardInfo> awardInfoMap = new Dictionary<int, LotteryAwardInfo>();

        public static string GetRedirectUrl(int publishmentSystemID, ELotteryType lotteryType, int lotteryID, int awardID, string returnUrl)
        {
            return PageUtils.GetWXUrl(
                $"background_lotteryWinner.aspx?publishmentSystemID={publishmentSystemID}&lotteryType={ELotteryTypeUtils.GetValue(lotteryType)}&lotteryID={lotteryID}&awardID={awardID}&returnUrl={StringUtils.ValueToUrl(returnUrl)}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            lotteryType = ELotteryTypeUtils.GetEnumType(Request.QueryString["lotteryType"]);
            lotteryID = TranslateUtils.ToInt(Request.QueryString["lotteryID"]);
            awardID = TranslateUtils.ToInt(Request.QueryString["awardID"]);
            returnUrl = StringUtils.ValueFromUrl(Request.QueryString["returnUrl"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWX.LotteryWinnerDAO.Delete(PublishmentSystemID, list);             
                        SuccessMessage("删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "删除失败！");
                    }
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 30;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            spContents.SelectCommand = DataProviderWX.LotteryWinnerDAO.GetSelectString(PublishmentSystemID, lotteryType, lotteryID, awardID);
            spContents.SortField = LotteryWinnerAttribute.ID;
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.CMS.LeftMenu.ID_Configration, "获奖名单查看", string.Empty);

                spContents.DataBind();

                var totalNum = 0;
                var wonNum = 0;
                DataProviderWX.LotteryAwardDAO.GetCount(PublishmentSystemID, lotteryType, lotteryID, out totalNum, out wonNum);
                InfoMessage($"总奖品数：{totalNum}，已中奖人数：{wonNum}，剩余奖品数：{totalNum - wonNum}");

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemID, lotteryType, lotteryID, awardID, returnUrl), "Delete", "True");
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的获奖项", "此操作将删除所选获奖项，确认吗？"));

                btnSetting.Attributes.Add("onclick", Modal.WinnerSetting.GetOpenWindowString(PublishmentSystemID));

                btnExport.Attributes.Add("onclick", Modal.Export.GetOpenWindowStringByLottery(PublishmentSystemID, lotteryType, lotteryID));

                btnReturn.Attributes.Add("onclick", $"location.href='{returnUrl}';return false");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var winnerInfo = new LotteryWinnerInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlAward = e.Item.FindControl("ltlAward") as Literal;
                var ltlRealName = e.Item.FindControl("ltlRealName") as Literal;
                var ltlMobile = e.Item.FindControl("ltlMobile") as Literal;
                var ltlEmail = e.Item.FindControl("ltlEmail") as Literal;
                var ltlAddress = e.Item.FindControl("ltlAddress") as Literal;
                var ltlStatus = e.Item.FindControl("ltlStatus") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                var ltlCashSN = e.Item.FindControl("ltlCashSN") as Literal;
                var ltlCashDate = e.Item.FindControl("ltlCashDate") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();

                LotteryAwardInfo awardInfo = null;
                if (awardInfoMap.ContainsKey(winnerInfo.AwardID))
                {
                    awardInfo = awardInfoMap[winnerInfo.AwardID];
                }
                else
                {
                    awardInfo = DataProviderWX.LotteryAwardDAO.GetAwardInfo(winnerInfo.AwardID);
                    awardInfoMap.Add(winnerInfo.AwardID, awardInfo);
                }
                if (awardInfo != null)
                {
                    ltlAward.Text =
                        $@"<a href=""{GetRedirectUrl(PublishmentSystemID,
                            ELotteryTypeUtils.GetEnumType(winnerInfo.LotteryType), winnerInfo.LotteryID, winnerInfo.AwardID,
                            returnUrl)}"">{awardInfo.AwardName + "：" + awardInfo.Title}</a>";
                }

                ltlRealName.Text = winnerInfo.RealName;
                ltlMobile.Text = winnerInfo.Mobile;
                ltlEmail.Text = winnerInfo.Email;
                ltlAddress.Text = winnerInfo.Address;
                ltlStatus.Text = EWinStatusUtils.GetText(EWinStatusUtils.GetEnumType(winnerInfo.Status));
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(winnerInfo.AddDate);
                ltlCashSN.Text = winnerInfo.CashSN;
                ltlCashDate.Text = DateUtils.GetDateAndTimeString(winnerInfo.CashDate);
            }
        }
    }
}
