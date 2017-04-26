using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageTemplateLog : BasePageCms
    {
        public Repeater rptContents;
        public SqlPager spContents;
        public Button btnCompare;
        public Button btnDelete;

        private int _templateId;

        public static string GetRedirectUrl(int publishmentSystemId, int templateId)
        {
            return PageUtils.GetCmsUrl(nameof(PageTemplateLog), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"TemplateID", templateId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _templateId = Body.GetQueryInt("templateID");

            if (Body.IsQueryExists("Compare"))
            {
                var idList = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (idList.Count != 2)
                {
                    FailMessage("请选择2条记录以便进行对比");
                }
            }
            if (Body.IsQueryExists("Delete"))
            {
                var arraylist = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                try
                {
                    DataProvider.TemplateLogDao.Delete(arraylist);
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = StringUtils.Constants.PageSize;

            spContents.SelectCommand = DataProvider.TemplateLogDao.GetSelectCommend(PublishmentSystemId, _templateId);

            spContents.SortField = "ID";
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdTemplate, "修订历史", AppManager.Cms.Permission.WebSite.Template);

                btnCompare.Attributes.Add("onclick",
                    PageUtils.GetRedirectStringWithCheckBoxValue(
                        PageUtils.GetCmsUrl(nameof(PageTemplateLog), new NameValueCollection
                        {
                            {"PublishmentSystemID", PublishmentSystemId.ToString()},
                            {"TemplateID", _templateId.ToString()},
                            {"Compare", true.ToString()}
                        }), "IDCollection", "IDCollection", "请选择需要对比的记录！"));

                btnDelete.Attributes.Add("onclick",
                    PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                        PageUtils.GetCmsUrl(nameof(PageTemplateLog), new NameValueCollection
                        {
                            {"PublishmentSystemID", PublishmentSystemId.ToString()},
                            {"TemplateID", _templateId.ToString()},
                            {"Delete", true.ToString()}
                        }), "IDCollection", "IDCollection", "请选择需要删除的修订历史！", "此操作将删除所选修订历史，确认吗？"));

                spContents.DataBind();
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlIndex = (Literal)e.Item.FindControl("ltlIndex");
                var ltlAddUserName = (Literal)e.Item.FindControl("ltlAddUserName");
                var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
                var ltlContentLength = (Literal)e.Item.FindControl("ltlContentLength");
                var ltlView = (Literal)e.Item.FindControl("ltlView");

                var logID = SqlUtils.EvalInt(e.Item.DataItem, "ID");

                ltlIndex.Text = Convert.ToString(e.Item.ItemIndex + 1);
                ltlAddUserName.Text = SqlUtils.EvalString(e.Item.DataItem, "AddUserName");
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(SqlUtils.EvalDateTime(e.Item.DataItem, "AddDate"));
                ltlContentLength.Text = SqlUtils.EvalInt(e.Item.DataItem, "ContentLength").ToString();
                ltlView.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalTemplateView.GetOpenLayerString(PublishmentSystemId,
                        logID)}"">查看</a>";
            }
        }
    }
}
