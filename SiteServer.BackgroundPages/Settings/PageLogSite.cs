using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageLogSite : BasePageCms
    {
        public DropDownList DdlSiteId;
        public DropDownList DdlLogType;
        public TextBox TbUserName;
        public TextBox TbKeyword;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;

        public Repeater RptContents;
        public SqlPager SpContents;
        public Literal LtlSite;

		public Button BtnDelete;
		public Button BtnDeleteAll;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = StringUtils.Constants.PageSize;

            SpContents.SelectCommand = !Body.IsQueryExists("LogType")
                ? DataProvider.SiteLogDao.GetSelectCommend()
                : DataProvider.SiteLogDao.GetSelectCommend(SiteId, Body.GetQueryString("LogType"),
                    Body.GetQueryString("UserName"), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"),
                    Body.GetQueryString("DateTo"));

            SpContents.SortField = nameof(SiteLogInfo.Id);
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (Body.IsQueryExists("Delete"))
            {
                var arraylist = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("IDCollection"));
                try
                {
                    DataProvider.SiteLogDao.Delete(arraylist);
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
            else if (Body.IsQueryExists("DeleteAll"))
            {
                try
                {
                    DataProvider.SiteLogDao.DeleteAll();
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            if (IsPostBack) return;

            VerifyAdministratorPermissions(ConfigManager.Permissions.Settings.Log);

            if (SiteId == 0)
            {
                LtlSite.Text = @"<th align=""text-center text-nowrap"">站点名称</th>";
            }

            DdlSiteId.Items.Add(new ListItem("<<全部站点>>", "0"));

            var siteIdList = SiteManager.GetSiteIdListOrderByLevel();
            foreach (var psId in siteIdList)
            {
                DdlSiteId.Items.Add(new ListItem(SiteManager.GetSiteInfo(psId).SiteName, psId.ToString())); 
            }

            DdlLogType.Items.Add(new ListItem("全部记录", "All"));
            DdlLogType.Items.Add(new ListItem("栏目相关记录", "Channel"));
            DdlLogType.Items.Add(new ListItem("内容相关记录", "Content"));

            if (Body.IsQueryExists("LogType"))
            {
                ControlUtils.SelectSingleItem(DdlSiteId, SiteId.ToString());
                ControlUtils.SelectSingleItem(DdlLogType, Body.GetQueryString("LogType"));
                TbUserName.Text = Body.GetQueryString("UserName");
                TbKeyword.Text = Body.GetQueryString("Keyword");
                TbDateFrom.Text = Body.GetQueryString("DateFrom");
                TbDateTo.Text = Body.GetQueryString("DateTo");
            }

            BtnDelete.Attributes.Add("onclick",
                PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                    PageUtils.GetSettingsUrl(nameof(PageLogSite), new NameValueCollection
                    {
                        {"Delete", "True"}
                    }), "IDCollection", "IDCollection", "请选择需要删除的日志！", "此操作将删除所选日志，确认吗？"));

            BtnDeleteAll.Attributes.Add("onclick",
                AlertUtils.ConfirmRedirect("删除所有日志", "此操作将删除所有日志信息，确定吗？", "删除全部",
                    PageUtils.GetSettingsUrl(nameof(PageLogSite), new NameValueCollection
                    {
                        {"DeleteAll", "True"}
                    })));

            SpContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var ltlSite = (Literal)e.Item.FindControl("ltlSite");
            var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
            var ltlIpAddress = (Literal)e.Item.FindControl("ltlIpAddress");
            var ltlAction = (Literal)e.Item.FindControl("ltlAction");
            var ltlSummary = (Literal)e.Item.FindControl("ltlSummary");

            if (SiteId == 0)
            {
                var siteInfo = SiteManager.GetSiteInfo(SqlUtils.EvalInt(e.Item.DataItem, nameof(SiteLogInfo.SiteId)));
                var siteName = string.Empty;
                if (siteInfo != null)
                {
                    siteName =
                        $"<a href='{siteInfo.Additional.WebUrl}' target='_blank'>{siteInfo.SiteName}</a>";
                }
                ltlSite.Text = $@"<td align=""text-center text-nowrap"">{siteName}</td>";
            }
            ltlUserName.Text = SqlUtils.EvalString(e.Item.DataItem, nameof(SiteLogInfo.UserName));
            ltlAddDate.Text = DateUtils.GetDateAndTimeString(SqlUtils.EvalDateTime(e.Item.DataItem, nameof(SiteLogInfo.AddDate)));
            ltlIpAddress.Text = SqlUtils.EvalString(e.Item.DataItem, nameof(SiteLogInfo.IpAddress));
            ltlAction.Text = SqlUtils.EvalString(e.Item.DataItem, nameof(SiteLogInfo.Action));
            ltlSummary.Text = SqlUtils.EvalString(e.Item.DataItem, nameof(SiteLogInfo.Summary));
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUtils.GetSettingsUrl(nameof(PageLogSite), new NameValueCollection
            {
                {"UserName", TbUserName.Text},
                {"Keyword", TbKeyword.Text},
                {"DateFrom", TbDateFrom.Text},
                {"DateTo", TbDateTo.Text},
                {"SiteId", DdlSiteId.SelectedValue},
                {"LogType", DdlLogType.SelectedValue}
            }));
        }
    }
}
