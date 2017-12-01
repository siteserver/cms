using System;
using System.Collections.Generic;
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
    public class PageLotteryWinner : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnDelete;
        public Button BtnSetting;
        public Button BtnExport;
        public Button BtnReturn;

        private ELotteryType _lotteryType;
        private int _lotteryId;
        private int _awardId;
        private string _returnUrl;
        private Dictionary<int, LotteryAwardInfo> _awardInfoMap = new Dictionary<int, LotteryAwardInfo>();

        public static string GetRedirectUrl(int publishmentSystemId, ELotteryType lotteryType, int lotteryId, int awardId, string returnUrl)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageLotteryWinner), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"lotteryType", ELotteryTypeUtils.GetValue(lotteryType)},
                {"lotteryId", lotteryId.ToString()},
                {"awardId", awardId.ToString()},
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            _lotteryType = ELotteryTypeUtils.GetEnumType(Request.QueryString["lotteryType"]);
            _lotteryId = TranslateUtils.ToInt(Request.QueryString["lotteryID"]);
            _awardId = TranslateUtils.ToInt(Request.QueryString["awardID"]);
            _returnUrl = StringUtils.ValueFromUrl(Request.QueryString["returnUrl"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWx.LotteryWinnerDao.Delete(PublishmentSystemId, list);
                        SuccessMessage("删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "删除失败！");
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            
            SpContents.SelectCommand = DataProviderWx.LotteryWinnerDao.GetSelectString(PublishmentSystemId, _lotteryType, _lotteryId, _awardId);
            SpContents.SortField = LotteryWinnerAttribute.Id;
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, "获奖名单查看", string.Empty);

                SpContents.DataBind();

                var totalNum = 0;
                var wonNum = 0;
                DataProviderWx.LotteryAwardDao.GetCount(PublishmentSystemId, _lotteryType, _lotteryId, out totalNum, out wonNum);
                InfoMessage($"总奖品数：{totalNum}，已中奖人数：{wonNum}，剩余奖品数：{totalNum - wonNum}");

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId, _lotteryType, _lotteryId, _awardId, _returnUrl), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的获奖项", "此操作将删除所选获奖项，确认吗？"));

                BtnSetting.Attributes.Add("onclick", ModalWinnerSetting.GetOpenWindowString(PublishmentSystemId));

                BtnExport.Attributes.Add("onclick", ModalExport.GetOpenWindowStringByLottery(PublishmentSystemId, _lotteryType, _lotteryId));

                BtnReturn.Attributes.Add("onclick", $"location.href='{_returnUrl}';return false");
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
                var ltlCashSn = e.Item.FindControl("ltlCashSN") as Literal;
                var ltlCashDate = e.Item.FindControl("ltlCashDate") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();

                LotteryAwardInfo awardInfo = null;
                if (_awardInfoMap.ContainsKey(winnerInfo.AwardId))
                {
                    awardInfo = _awardInfoMap[winnerInfo.AwardId];
                }
                else
                {
                    awardInfo = DataProviderWx.LotteryAwardDao.GetAwardInfo(winnerInfo.AwardId);
                    _awardInfoMap.Add(winnerInfo.AwardId, awardInfo);
                }
                if (awardInfo != null)
                {
                    ltlAward.Text =
                        $@"<a href=""{GetRedirectUrl(PublishmentSystemId,
                            ELotteryTypeUtils.GetEnumType(winnerInfo.LotteryType), winnerInfo.LotteryId, winnerInfo.AwardId,
                            _returnUrl)}"">{awardInfo.AwardName + "：" + awardInfo.Title}</a>";
                }

                ltlRealName.Text = winnerInfo.RealName;
                ltlMobile.Text = winnerInfo.Mobile;
                ltlEmail.Text = winnerInfo.Email;
                ltlAddress.Text = winnerInfo.Address;
                ltlStatus.Text = EWinStatusUtils.GetText(EWinStatusUtils.GetEnumType(winnerInfo.Status));
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(winnerInfo.AddDate);
                ltlCashSn.Text = winnerInfo.CashSn;
                ltlCashDate.Text = DateUtils.GetDateAndTimeString(winnerInfo.CashDate);
            }
        }
    }
}
