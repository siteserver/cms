using System;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundLottery : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button btnAdd;
        public Button btnDelete;

        private ELotteryType lotteryType;

        public static string GetRedirectUrl(int publishmentSystemID, ELotteryType lotteryType)
        {
            return PageUtils.GetWXUrl(
                $"background_lottery.aspx?PublishmentSystemID={publishmentSystemID}&lotteryType={ELotteryTypeUtils.GetValue(lotteryType)}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            lotteryType = ELotteryTypeUtils.GetEnumType(Request.QueryString["lotteryType"]);
            var lotteryName = ELotteryTypeUtils.GetText(lotteryType);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWX.LotteryDAO.Delete(PublishmentSystemID, list);

                        SuccessMessage(lotteryName + "删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, lotteryName + "删除失败！");
                    }
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 30;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            spContents.SelectCommand = DataProviderWX.LotteryDAO.GetSelectString(PublishmentSystemID, lotteryType);
            spContents.SortField = LotteryAttribute.ID;
            spContents.SortMode = SortMode.ASC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            {
                if (lotteryType == ELotteryType.Scratch)
                {
                    BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Scratch, lotteryName, AppManager.WeiXin.Permission.WebSite.Scratch);
                }
                else if (lotteryType == ELotteryType.BigWheel)
                {
                    BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_BigWheel, lotteryName, AppManager.WeiXin.Permission.WebSite.BigWheel);
                }
                else if (lotteryType == ELotteryType.GoldEgg)
                {
                    BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_GoldEgg, lotteryName, AppManager.WeiXin.Permission.WebSite.GoldEgg);
                }
                else if (lotteryType == ELotteryType.Flap)
                {
                    BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Flap, lotteryName, AppManager.WeiXin.Permission.WebSite.Flap);
                }
                else if (lotteryType == ELotteryType.YaoYao)
                {
                    BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_YaoYao, lotteryName, AppManager.WeiXin.Permission.WebSite.YaoYao);
                }               

                spContents.DataBind();

                var urlAdd = string.Empty;
                if (lotteryType == ELotteryType.Scratch)
                {
                    urlAdd = BackgroundScratchAdd.GetRedirectUrl(PublishmentSystemID, 0);
                }
                else if (lotteryType == ELotteryType.BigWheel)
                {
                    urlAdd = BackgroundBigWheelAdd.GetRedirectUrl(PublishmentSystemID, 0);
                }
                else if (lotteryType == ELotteryType.GoldEgg)
                {
                    urlAdd = BackgroundGoldEggAdd.GetRedirectUrl(PublishmentSystemID, 0);
                }
                else if (lotteryType == ELotteryType.Flap)
                {
                    urlAdd = BackgroundFlapAdd.GetRedirectUrl(PublishmentSystemID, 0);
                }
                else if (lotteryType == ELotteryType.YaoYao)
                {
                    urlAdd = BackgroundYaoYaoAdd.GetRedirectUrl(PublishmentSystemID, 0);
                }
                btnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemID, lotteryType), "Delete", "True");
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的" + lotteryName, "此操作将删除所选" + lotteryName + "，确认吗？"));
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lotteryInfo = new LotteryInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlKeywords = e.Item.FindControl("ltlKeywords") as Literal;
                var ltlStartDate = e.Item.FindControl("ltlStartDate") as Literal;
                var ltlEndDate = e.Item.FindControl("ltlEndDate") as Literal;
                var ltlUserCount = e.Item.FindControl("ltlUserCount") as Literal;
                var ltlPVCount = e.Item.FindControl("ltlPVCount") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlWinner = e.Item.FindControl("ltlWinner") as Literal;
                var ltlPreviewUrl = e.Item.FindControl("ltlPreviewUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = lotteryInfo.Title;
                ltlKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(lotteryInfo.KeywordID);
                ltlStartDate.Text = DateUtils.GetDateAndTimeString(lotteryInfo.StartDate);
                ltlEndDate.Text = DateUtils.GetDateAndTimeString(lotteryInfo.EndDate);
                ltlUserCount.Text = lotteryInfo.UserCount.ToString();
                ltlPVCount.Text = lotteryInfo.PVCount.ToString();

                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(!lotteryInfo.IsDisabled);

                ltlWinner.Text =
                    $@"<a href=""{BackgroundLotteryWinner.GetRedirectUrl(PublishmentSystemID, lotteryType,
                        lotteryInfo.ID, 0, GetRedirectUrl(PublishmentSystemID, lotteryType))}"">查看获奖名单</a>";

                var urlPreview = LotteryManager.GetLotteryUrl(lotteryInfo, string.Empty);
                urlPreview = BackgroundPreview.GetRedirectUrlToMobile(urlPreview);
                ltlPreviewUrl.Text = $@"<a href=""{urlPreview}"" target=""_blank"">预览</a>";

                var urlEdit = string.Empty;
                if (lotteryType == ELotteryType.Scratch)
                {
                    urlEdit = BackgroundScratchAdd.GetRedirectUrl(PublishmentSystemID, lotteryInfo.ID);
                }
                else if (lotteryType == ELotteryType.BigWheel)
                {
                    urlEdit = BackgroundBigWheelAdd.GetRedirectUrl(PublishmentSystemID, lotteryInfo.ID);
                }
                else if (lotteryType == ELotteryType.GoldEgg)
                {
                    urlEdit = BackgroundGoldEggAdd.GetRedirectUrl(PublishmentSystemID, lotteryInfo.ID);
                }
                else if (lotteryType == ELotteryType.Flap)
                {
                    urlEdit = BackgroundFlapAdd.GetRedirectUrl(PublishmentSystemID, lotteryInfo.ID);
                }
                else if (lotteryType == ELotteryType.YaoYao)
                {
                    urlEdit = BackgroundYaoYaoAdd.GetRedirectUrl(PublishmentSystemID, lotteryInfo.ID);
                }

                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">编辑</a>";
            }
        }
    }
}
