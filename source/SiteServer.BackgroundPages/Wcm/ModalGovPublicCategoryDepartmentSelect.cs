using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Admin;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Wcm.GovPublic;

namespace SiteServer.BackgroundPages.Wcm
{
	public class ModalGovPublicCategoryDepartmentSelect : BasePageCms
    {
        public Repeater RptCategory;

        private readonly NameValueCollection _additional = new NameValueCollection();

        public static string GetOpenWindowString(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("设置分类",
                PageUtils.GetWcmUrl(nameof(ModalGovPublicCategoryDepartmentSelect), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()}
                }), 460, 360, true);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _additional.Add("UrlFormatString",
                PageUtils.GetWcmUrl(nameof(ModalGovPublicCategoryDepartmentSelect), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()}
                }));

			if (!IsPostBack)
			{
                if (Body.IsQueryExists("DepartmentID"))
                {
                    var departmentId = TranslateUtils.ToInt(Request.QueryString["DepartmentID"]);
                    var departmentName = DepartmentManager.GetDepartmentName(departmentId);
                    string scripts = $"window.parent.showCategoryDepartment('{departmentName}', '{departmentId}');";
                    PageUtils.CloseModalPageWithoutRefresh(Page, scripts);
                }
                else
                {
                    ClientScriptRegisterClientScriptBlock("NodeTreeScript", DepartmentTreeItem.GetScript(EDepartmentLoadingType.DepartmentSelect, _additional));
                    BindGrid();
                }
			}
		}

        public void BindGrid()
        {
            try
            {
                RptCategory.DataSource = GovPublicManager.GetFirstDepartmentIdList(PublishmentSystemInfo);
                RptCategory.ItemDataBound += rptCategory_ItemDataBound;
                RptCategory.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        private void rptCategory_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var departmentId = (int)e.Item.DataItem;
            var departmentInfo = DepartmentManager.GetDepartmentInfo(departmentId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = PageDepartment.GetDepartmentRowHtml(departmentInfo, EDepartmentLoadingType.DepartmentSelect, _additional);
        }
	}
}
