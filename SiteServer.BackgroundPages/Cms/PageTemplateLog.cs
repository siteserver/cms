using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Context;
using SiteServer.CMS.Repositories;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageTemplateLog : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;
        public Button BtnDelete;

        private int _templateId;

        public static string GetRedirectUrl(int siteId, int templateId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageTemplateLog), new NameValueCollection
            {
                {"TemplateID", templateId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _templateId = AuthRequest.GetQueryInt("templateID");

            if (AuthRequest.IsQueryExists("Delete"))
            {
                var arraylist = StringUtils.GetIntList(Request.QueryString["IDCollection"]);
                try
                {
                    DataProvider.TemplateLogRepository.DeleteAsync(arraylist).GetAwaiter().GetResult();
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = Constants.PageSize;

            SpContents.SelectCommand = DataProvider.TemplateLogRepository.GetSelectCommend(SiteId, _templateId);

            SpContents.SortField = nameof(TemplateLog.Id);
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (IsPostBack) return;

            VerifySitePermissions(Constants.WebSitePermissions.Template);

            BtnDelete.Attributes.Add("onClick",
                PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                    PageUtils.GetCmsUrl(SiteId, nameof(PageTemplateLog), new NameValueCollection
                    {
                        {"TemplateID", _templateId.ToString()},
                        {"Delete", true.ToString()}
                    }), "IDCollection", "IDCollection", "请选择需要删除的修订历史！", "此操作将删除所选修订历史，确认吗？"));

            SpContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var ltlIndex = (Literal)e.Item.FindControl("ltlIndex");
            var ltlAddUserName = (Literal)e.Item.FindControl("ltlAddUserName");
            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
            var ltlContentLength = (Literal)e.Item.FindControl("ltlContentLength");
            var ltlView = (Literal)e.Item.FindControl("ltlView");

            var logId = SqlUtils.EvalInt(e.Item.DataItem, nameof(TemplateLog.Id));

            ltlIndex.Text = Convert.ToString(e.Item.ItemIndex + 1);
            ltlAddUserName.Text = SqlUtils.EvalString(e.Item.DataItem, nameof(TemplateLog.AddUserName));
            ltlAddDate.Text = DateUtils.GetDateAndTimeString(SqlUtils.EvalDateTime(e.Item.DataItem, nameof(TemplateLog.AddDate)));
            ltlContentLength.Text = SqlUtils.EvalInt(e.Item.DataItem, nameof(TemplateLog.ContentLength)).ToString();
            ltlView.Text =
                $@"<a href=""javascript:;"" onClick=""{ModalTemplateView.GetOpenWindowString(SiteId,
                    logId)}"">查看</a>";
        }
    }
}
