using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageLogServiceCreateTask : BasePage
    {
        public DropDownList DdlIsSuccess;
        public TextBox TbKeyword;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;
        public Repeater RptContents;
        public SqlPager SpContents;
		public Button BtnDelete;
		public Button BtnDeleteAll;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = StringUtils.Constants.PageSize;

            SpContents.SelectCommand = Body.GetQueryString("Keyword") == null ? DataProvider.CreateTaskLogDao.GetSelectCommend() : DataProvider.CreateTaskLogDao.GetSelectCommend(ETriStateUtils.GetEnumType(Body.GetQueryString("IsSuccess")), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"));

            SpContents.SortField = "ID";
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Log);

            ETriStateUtils.AddListItems(DdlIsSuccess, "全部", "成功", "失败");

            if (Body.IsQueryExists("Keyword"))
            {
                ControlUtils.SelectSingleItem(DdlIsSuccess, Body.GetQueryString("IsSuccess"));
                TbKeyword.Text = Body.GetQueryString("Keyword");
                TbDateFrom.Text = Body.GetQueryString("DateFrom");
                TbDateTo.Text = Body.GetQueryString("DateTo");
            }

            if (Body.IsQueryExists("Delete"))
            {
                var list = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("IDCollection"));
                DataProvider.CreateTaskLogDao.Delete(list);
                SuccessDeleteMessage();
            }
            else if (Body.IsQueryExists("DeleteAll"))
            {
                DataProvider.CreateTaskLogDao.DeleteAll();
                SuccessDeleteMessage();
            }

            BtnDelete.Attributes.Add("onclick",
                PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                    PageUtils.GetSettingsUrl(nameof(PageLogServiceCreateTask), new NameValueCollection
                    {
                        {"Delete", "True"}
                    }), "IDCollection", "IDCollection", "请选择需要删除的日志！", "此操作将删除所选日志，确认吗？"));

            BtnDeleteAll.Attributes.Add("onclick",
                AlertUtils.ConfirmRedirect("删除所有日志", "此操作将删除所有日志信息，确定吗？", "删除全部",
                    PageUtils.GetSettingsUrl(nameof(PageLogServiceCreateTask), new NameValueCollection
                    {
                        {"DeleteAll", "True"}
                    })));

            SpContents.DataBind();
        }

        static void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var publishmentSystemId = SqlUtils.EvalInt(e.Item.DataItem, "PublishmentSystemID");
            var taskName = SqlUtils.EvalString(e.Item.DataItem, "TaskName");
            var createType = ECreateTypeUtils.GetEnumType(SqlUtils.EvalString(e.Item.DataItem, "CreateType"));
            var timeSpan = SqlUtils.EvalString(e.Item.DataItem, "TimeSpan");

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            if (publishmentSystemInfo == null)
            {
                e.Item.Visible = false;
                return;
            }

            var addDate = SqlUtils.EvalDateTime(e.Item.DataItem, "AddDate");
            var isSuccess = SqlUtils.EvalString(e.Item.DataItem, "IsSuccess");
            var errorMessage = SqlUtils.EvalString(e.Item.DataItem, "ErrorMessage");

            var ltlPublishmentSystem = (Literal)e.Item.FindControl("ltlPublishmentSystem");
            var ltlTaskName = (Literal)e.Item.FindControl("ltlTaskName");
            var ltlCreateType = (Literal)e.Item.FindControl("ltlCreateType");
            var ltlTimeSpan = (Literal)e.Item.FindControl("ltlTimeSpan");
            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
            var ltlIsSuccess = (Literal)e.Item.FindControl("ltlIsSuccess");
            var ltlErrorMessage = (Literal)e.Item.FindControl("ltlErrorMessage");

            ltlPublishmentSystem.Text = publishmentSystemInfo.PublishmentSystemName;
            ltlTaskName.Text = taskName;
            ltlTimeSpan.Text = timeSpan;
            ltlCreateType.Text = ECreateTypeUtils.GetText(createType);
                
            ltlAddDate.Text = DateUtils.GetDateAndTimeString(addDate);
            ltlIsSuccess.Text = StringUtils.GetTrueOrFalseImageHtml(isSuccess);
            ltlErrorMessage.Text = errorMessage;
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUtils.GetSettingsUrl(nameof(PageLogServiceCreateTask), new NameValueCollection
            {
                {"Keyword", TbKeyword.Text},
                {"DateFrom", TbDateFrom.Text},
                {"DateTo", TbDateTo.Text},
                {"IsSuccess", DdlIsSuccess.SelectedValue}
            }));
        }
    }
}
