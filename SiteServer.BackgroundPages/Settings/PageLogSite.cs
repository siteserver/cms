using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageLogSite : BasePageCms
    {
        public DropDownList DdlPublishmentSystemId;
        public DropDownList DdlLogType;
        public TextBox TbUserName;
        public TextBox TbKeyword;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;

        public Repeater RptContents;
        public SqlPager SpContents;
        public Literal LtlPublishmentSystem;

		public Button BtnDelete;
		public Button BtnDeleteAll;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = StringUtils.Constants.PageSize;

            SpContents.SelectCommand = !Body.IsQueryExists("LogType")
                ? DataProvider.LogDao.GetSelectCommend()
                : DataProvider.LogDao.GetSelectCommend(PublishmentSystemId, Body.GetQueryString("LogType"),
                    Body.GetQueryString("UserName"), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"),
                    Body.GetQueryString("DateTo"));

            SpContents.SortField = "ID";
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (Body.IsQueryExists("Delete"))
            {
                var arraylist = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("IDCollection"));
                try
                {
                    DataProvider.LogDao.Delete(arraylist);
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
                    DataProvider.LogDao.DeleteAll();
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Log);

            if (PublishmentSystemId == 0)
            {
                LtlPublishmentSystem.Text = @"<th align=""text-center text-nowrap"">站点名称</th>";
            }

            DdlPublishmentSystemId.Items.Add(new ListItem("<<全部站点>>", "0"));

            var publishmentSystemIdList = PublishmentSystemManager.GetPublishmentSystemIdListOrderByLevel();
            foreach (var psId in publishmentSystemIdList)
            {
                DdlPublishmentSystemId.Items.Add(new ListItem(PublishmentSystemManager.GetPublishmentSystemInfo(psId).PublishmentSystemName, psId.ToString())); 
            }

            DdlLogType.Items.Add(new ListItem("全部记录", "All"));
            DdlLogType.Items.Add(new ListItem("栏目相关记录", "Channel"));
            DdlLogType.Items.Add(new ListItem("内容相关记录", "Content"));

            if (Body.IsQueryExists("LogType"))
            {
                ControlUtils.SelectSingleItem(DdlPublishmentSystemId, PublishmentSystemId.ToString());
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

            var ltlPublishmentSystem = (Literal)e.Item.FindControl("ltlPublishmentSystem");
            var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
            var ltlIpAddress = (Literal)e.Item.FindControl("ltlIpAddress");
            var ltlAction = (Literal)e.Item.FindControl("ltlAction");
            var ltlSummary = (Literal)e.Item.FindControl("ltlSummary");

            if (PublishmentSystemId == 0)
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(SqlUtils.EvalInt(e.Item.DataItem, "PublishmentSystemID"));
                var publishmentSystemName = string.Empty;
                if (publishmentSystemInfo != null)
                {
                    publishmentSystemName =
                        $"<a href='{publishmentSystemInfo.Additional.WebUrl}' target='_blank'>{publishmentSystemInfo.PublishmentSystemName}</a>";
                }
                ltlPublishmentSystem.Text = $@"<td align=""text-center text-nowrap"">{publishmentSystemName}</td>";
            }
            ltlUserName.Text = SqlUtils.EvalString(e.Item.DataItem, "UserName");
            ltlAddDate.Text = DateUtils.GetDateAndTimeString(SqlUtils.EvalDateTime(e.Item.DataItem, "AddDate"));
            ltlIpAddress.Text = SqlUtils.EvalString(e.Item.DataItem, "IPAddress");
            ltlAction.Text = SqlUtils.EvalString(e.Item.DataItem, "Action");
            ltlSummary.Text = SqlUtils.EvalString(e.Item.DataItem, "Summary");
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUtils.GetSettingsUrl(nameof(PageLogSite), new NameValueCollection
            {
                {"UserName", TbUserName.Text},
                {"Keyword", TbKeyword.Text},
                {"DateFrom", TbDateFrom.Text},
                {"DateTo", TbDateTo.Text},
                {"PublishmentSystemID", DdlPublishmentSystemId.SelectedValue},
                {"LogType", DdlLogType.SelectedValue}
            }));
        }
    }
}
