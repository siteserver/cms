using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageLogUser : BasePage
    {
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;
        public TextBox TbUserName;
        public TextBox TbKeyword;
        public Repeater RptContents;
        public SqlPager SpContents;
        public Literal LtlState;
        public Button BtnDelete;
        public Button BtnDeleteAll;
        public Button BtnSetting;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = Constants.PageSize;

            SpContents.SelectCommand = !AuthRequest.IsQueryExists("Keyword") ? DataProvider.UserLogDao.GetSelectCommend() : DataProvider.UserLogDao.GetSelectCommend(AuthRequest.GetQueryString("UserName"), AuthRequest.GetQueryString("Keyword"), AuthRequest.GetQueryString("DateFrom"), AuthRequest.GetQueryString("DateTo"));

            SpContents.SortField = nameof(UserLog.Id);
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Log);

            var config = ConfigManager.GetInstanceAsync().GetAwaiter().GetResult();

            if (AuthRequest.IsQueryExists("Keyword"))
            {
                TbUserName.Text = AuthRequest.GetQueryString("UserName");
                TbKeyword.Text = AuthRequest.GetQueryString("Keyword");
                TbDateFrom.Text = AuthRequest.GetQueryString("DateFrom");
                TbDateTo.Text = AuthRequest.GetQueryString("DateTo");
            }

            if (AuthRequest.IsQueryExists("Delete"))
            {
                var list = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("IDCollection"));
                DataProvider.UserLogDao.DeleteAsync(list).GetAwaiter().GetResult();
                SuccessDeleteMessage();
            }
            else if (AuthRequest.IsQueryExists("DeleteAll"))
            {
                DataProvider.UserLogDao.DeleteAllAsync().GetAwaiter().GetResult();
                SuccessDeleteMessage();
            }
            else if (AuthRequest.IsQueryExists("Setting"))
            {
                config.IsLogUser = !config.IsLogUser;
                DataProvider.ConfigDao.UpdateAsync(config).GetAwaiter().GetResult();
                SuccessMessage($"成功{(config.IsLogUser ? "启用" : "禁用")}日志记录");
            }

            BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUtils.GetSettingsUrl(nameof(PageLogUser), new NameValueCollection
            {
                {"Delete", "True" }
            }), "IDCollection", "IDCollection", "请选择需要删除的日志！", "此操作将删除所选日志，确认吗？"));

            BtnDeleteAll.Attributes.Add("onclick",
                AlertUtils.ConfirmRedirect("删除所有日志", "此操作将删除所有日志信息，确定吗？", "删除全部",
                    PageUtils.GetSettingsUrl(nameof(PageLogUser), new NameValueCollection
                    {
                        {"DeleteAll", "True"}
                    })));

            if (config.IsLogUser)
            {
                BtnSetting.Text = "禁用用户日志";
                BtnSetting.Attributes.Add("onclick",
                    AlertUtils.ConfirmRedirect("禁用用户日志", "此操作将禁用用户日志记录功能，确定吗？", "禁 用",
                        PageUtils.GetSettingsUrl(nameof(PageLogUser), new NameValueCollection
                        {
                            {"Setting", "True"}
                        })));
            }
            else
            {
                LtlState.Text = @"<div class=""alert alert-danger m-t-10"">用户日志当前处于禁用状态，系统将不会记录用户操作日志！</div>";
                BtnSetting.Text = "启用用户日志";
                BtnSetting.Attributes.Add("onclick",
                    AlertUtils.ConfirmRedirect("启用用户日志", "此操作将启用用户日志记录功能，确定吗？", "启 用",
                        PageUtils.GetSettingsUrl(nameof(PageLogUser), new NameValueCollection
                        {
                            {"Setting", "True"}
                        })));
            }

            SpContents.DataBind();
        }

        private static void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
            var ltlIpAddress = (Literal)e.Item.FindControl("ltlIpAddress");
            var ltlAction = (Literal)e.Item.FindControl("ltlAction");
            var ltlSummary = (Literal)e.Item.FindControl("ltlSummary");

            ltlUserName.Text = SqlUtils.EvalString(e.Item.DataItem, nameof(UserLog.UserName));
            ltlAddDate.Text = DateUtils.GetDateAndTimeString(SqlUtils.EvalDateTime(e.Item.DataItem, nameof(UserLog.AddDate)));
            ltlIpAddress.Text = SqlUtils.EvalString(e.Item.DataItem, nameof(UserLog.IpAddress));
            ltlAction.Text = SqlUtils.EvalString(e.Item.DataItem, nameof(UserLog.Action));
            ltlSummary.Text = SqlUtils.EvalString(e.Item.DataItem, nameof(UserLog.Summary));
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUtils.GetSettingsUrl(nameof(PageLogUser), new NameValueCollection
            {
                {"UserName", TbUserName.Text},
                {"Keyword", TbKeyword.Text},
                {"DateFrom", TbDateFrom.Text},
                {"DateTo", TbDateTo.Text}
            }));
        }
    }
}
