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

namespace SiteServer.BackgroundPages.Service
{
    public class PageCreateTaskLog : BasePage
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
            RptContents.ItemDataBound += rptContents_ItemDataBound;

			if(!IsPostBack)
            {
                BreadCrumbService(AppManager.Service.LeftMenu.ServiceCreate, "任务运行日志", AppManager.Service.Permission.ServiceCreate);

                ETriStateUtils.AddListItems(DdlIsSuccess, "全部", "成功", "失败");

                if (Body.IsQueryExists("Keyword"))
                {
                    ControlUtils.SelectListItems(DdlIsSuccess, Body.GetQueryString("IsSuccess"));
                    TbKeyword.Text = Body.GetQueryString("Keyword");
                    TbDateFrom.Text = Body.GetQueryString("DateFrom");
                    TbDateTo.Text = Body.GetQueryString("DateTo");
                }

                if (Body.IsQueryExists("Delete"))
                {
                    var list = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("IDCollection"));
                    try
                    {
                        DataProvider.CreateTaskLogDao.Delete(list);
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
                        DataProvider.CreateTaskLogDao.DeleteAll();
                        SuccessDeleteMessage();
                    }
                    catch (Exception ex)
                    {
                        FailDeleteMessage(ex);
                    }
                }

                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUtils.GetServiceUrl(nameof(PageCreateTaskLog), new NameValueCollection
                {
                    {"Delete", "True"}
                }), "IDCollection", "IDCollection", "请选择需要删除的日志！", "此操作将删除所选日志，确认吗？"));

                BtnDeleteAll.Attributes.Add("onclick", PageUtils.GetRedirectStringWithConfirm(PageUtils.GetServiceUrl(nameof(PageCreateTaskLog), new NameValueCollection
                {
                    {"DeleteAll", "True"}
                }), "此操作将删除所有日志信息，确定吗？"));

                SpContents.DataBind();
			}
		}

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
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
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(PageUrl, true);
        }

        private string PageUrl => PageUtils.GetServiceUrl(nameof(PageCreateTaskLog), new NameValueCollection
        {
            {"Keyword", TbKeyword.Text},
            {"DateFrom", TbDateFrom.Text},
            {"DateTo", TbDateTo.Text},
            {"IsSuccess", DdlIsSuccess.SelectedValue}
        });
	}
}
