using System;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundCollect : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button btnAdd;
        public Button btnDelete;

        public static string GetRedirectUrl(int publishmentSystemID)
        {
            return PageUtils.GetWXUrl($"background_collect.aspx?publishmentSystemID={publishmentSystemID}");
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
                        DataProviderWX.CollectDAO.Delete(PublishmentSystemID, list);
                        SuccessMessage("征集删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "征集删除失败！");
                    }
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 30;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            spContents.SelectCommand = DataProviderWX.CollectDAO.GetSelectString(PublishmentSystemID);
            spContents.SortField = CollectAttribute.ID;
            spContents.SortMode = SortMode.ASC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Collect, "微征集管理", AppManager.WeiXin.Permission.WebSite.Collect);
                spContents.DataBind();

                var urlAdd = BackgroundCollectAdd.GetRedirectUrl(PublishmentSystemID, 0);
                btnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemID), "Delete", "True");
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的征集活动", "此操作将删除所选征集活动，确认吗？"));
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var collectInfo = new CollectInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlKeywords = e.Item.FindControl("ltlKeywords") as Literal;
                var ltlStartDate = e.Item.FindControl("ltlStartDate") as Literal;
                var ltlEndDate = e.Item.FindControl("ltlEndDate") as Literal;
                var ltlUserCount = e.Item.FindControl("ltlUserCount") as Literal;
                var ltlPVCount = e.Item.FindControl("ltlPVCount") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlCollectUrl = e.Item.FindControl("ltlCollectUrl") as Literal;
                var ltlLogUrl = e.Item.FindControl("ltlLogUrl") as Literal;
                var ltlPreviewUrl = e.Item.FindControl("ltlPreviewUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = collectInfo.Title;
                ltlKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(collectInfo.KeywordID);
                ltlStartDate.Text = DateUtils.GetDateAndTimeString(collectInfo.StartDate);
                ltlEndDate.Text = DateUtils.GetDateAndTimeString(collectInfo.EndDate);
                ltlUserCount.Text = collectInfo.UserCount.ToString();
                ltlPVCount.Text = collectInfo.PVCount.ToString();

                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(!collectInfo.IsDisabled);

                var urlCollect = BackgroundCollectItem.GetRedirectUrl(PublishmentSystemID, collectInfo.ID, GetRedirectUrl(PublishmentSystemID));
                ltlCollectUrl.Text = $@"<a href=""{urlCollect}"">参赛记录</a>";

                var urlLog = BackgroundCollectLog.GetRedirectUrl(PublishmentSystemID, collectInfo.ID, GetRedirectUrl(PublishmentSystemID));
                ltlLogUrl.Text = $@"<a href=""{urlLog}"">投票记录</a>";

                var urlPreview = CollectManager.GetCollectUrl(collectInfo, string.Empty);
                urlPreview = BackgroundPreview.GetRedirectUrlToMobile(urlPreview);
                ltlPreviewUrl.Text = $@"<a href=""{urlPreview}"" target=""_blank"">预览</a>";

                var urlEdit = BackgroundCollectAdd.GetRedirectUrl(PublishmentSystemID, collectInfo.ID);
                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">编辑</a>";
            }
        }
    }
}
