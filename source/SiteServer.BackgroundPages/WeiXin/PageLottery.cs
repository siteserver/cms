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
    public class PageLottery : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnAdd;
        public Button BtnDelete;

        private ELotteryType _lotteryType;

        public static string GetRedirectUrl(int publishmentSystemId, ELotteryType lotteryType)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageLottery), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"lotteryType", ELotteryTypeUtils.GetValue(lotteryType)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _lotteryType = ELotteryTypeUtils.GetEnumType(Request.QueryString["lotteryType"]);
            var lotteryName = ELotteryTypeUtils.GetText(_lotteryType);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWx.LotteryDao.Delete(PublishmentSystemId, list);

                        SuccessMessage(lotteryName + "删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, lotteryName + "删除失败！");
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            
            SpContents.SelectCommand = DataProviderWx.LotteryDao.GetSelectString(PublishmentSystemId, _lotteryType);
            SpContents.SortField = LotteryAttribute.Id;
            SpContents.SortMode = SortMode.ASC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                if (_lotteryType == ELotteryType.Scratch)
                {
                    BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdScratch, lotteryName, AppManager.WeiXin.Permission.WebSite.Scratch);
                }
                else if (_lotteryType == ELotteryType.BigWheel)
                {
                    BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdBigWheel, lotteryName, AppManager.WeiXin.Permission.WebSite.BigWheel);
                }
                else if (_lotteryType == ELotteryType.GoldEgg)
                {
                    BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdGoldEgg, lotteryName, AppManager.WeiXin.Permission.WebSite.GoldEgg);
                }
                else if (_lotteryType == ELotteryType.Flap)
                {
                    BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdFlap, lotteryName, AppManager.WeiXin.Permission.WebSite.Flap);
                }
                else if (_lotteryType == ELotteryType.YaoYao)
                {
                    BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdYaoYao, lotteryName, AppManager.WeiXin.Permission.WebSite.YaoYao);
                }               

                SpContents.DataBind();

                var urlAdd = string.Empty;
                if (_lotteryType == ELotteryType.Scratch)
                {
                    urlAdd = PageScratchAdd.GetRedirectUrl(PublishmentSystemId, 0);
                }
                else if (_lotteryType == ELotteryType.BigWheel)
                {
                    urlAdd = PageBigWheelAdd.GetRedirectUrl(PublishmentSystemId, 0);
                }
                else if (_lotteryType == ELotteryType.GoldEgg)
                {
                    urlAdd = PageGoldEggAdd.GetRedirectUrl(PublishmentSystemId, 0);
                }
                else if (_lotteryType == ELotteryType.Flap)
                {
                    urlAdd = PageFlapAdd.GetRedirectUrl(PublishmentSystemId, 0);
                }
                else if (_lotteryType == ELotteryType.YaoYao)
                {
                    urlAdd = PageYaoYaoAdd.GetRedirectUrl(PublishmentSystemId, 0);
                }
                BtnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId, _lotteryType), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的" + lotteryName, "此操作将删除所选" + lotteryName + "，确认吗？"));
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
                var ltlPvCount = e.Item.FindControl("ltlPVCount") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlWinner = e.Item.FindControl("ltlWinner") as Literal;
                var ltlPreviewUrl = e.Item.FindControl("ltlPreviewUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = lotteryInfo.Title;
                ltlKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(lotteryInfo.KeywordId);
                ltlStartDate.Text = DateUtils.GetDateAndTimeString(lotteryInfo.StartDate);
                ltlEndDate.Text = DateUtils.GetDateAndTimeString(lotteryInfo.EndDate);
                ltlUserCount.Text = lotteryInfo.UserCount.ToString();
                ltlPvCount.Text = lotteryInfo.PvCount.ToString();

                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(!lotteryInfo.IsDisabled);

                ltlWinner.Text =
                    $@"<a href=""{PageLotteryWinner.GetRedirectUrl(PublishmentSystemId, _lotteryType,
                        lotteryInfo.Id, 0, GetRedirectUrl(PublishmentSystemId, _lotteryType))}"">查看获奖名单</a>";

                //var urlPreview = LotteryManager.GetLotteryUrl(PublishmentSystemInfo, lotteryInfo, string.Empty);
                //urlPreview = BackgroundPreview.GetRedirectUrlToMobile(urlPreview);
                //ltlPreviewUrl.Text = $@"<a href=""{urlPreview}"" target=""_blank"">预览</a>";

                var urlEdit = string.Empty;
                if (_lotteryType == ELotteryType.Scratch)
                {
                    urlEdit = PageScratchAdd.GetRedirectUrl(PublishmentSystemId, lotteryInfo.Id);
                }
                else if (_lotteryType == ELotteryType.BigWheel)
                {
                    urlEdit = PageBigWheelAdd.GetRedirectUrl(PublishmentSystemId, lotteryInfo.Id);
                }
                else if (_lotteryType == ELotteryType.GoldEgg)
                {
                    urlEdit = PageGoldEggAdd.GetRedirectUrl(PublishmentSystemId, lotteryInfo.Id);
                }
                else if (_lotteryType == ELotteryType.Flap)
                {
                    urlEdit = PageFlapAdd.GetRedirectUrl(PublishmentSystemId, lotteryInfo.Id);
                }
                else if (_lotteryType == ELotteryType.YaoYao)
                {
                    urlEdit = PageYaoYaoAdd.GetRedirectUrl(PublishmentSystemId, lotteryInfo.Id);
                }

                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">编辑</a>";
            }
        }
    }
}
