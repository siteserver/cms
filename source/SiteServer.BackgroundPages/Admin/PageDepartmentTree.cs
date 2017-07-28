using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Admin
{
    public class PageDepartmentTree : BasePage
    {
        public Repeater rptDepartment;

        private readonly NameValueCollection _additional = new NameValueCollection();

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _additional.Add("module", Body.GetQueryString("module"));

            if (!IsPostBack)
            {
                ClientScriptRegisterClientScriptBlock("NodeTreeScript", DepartmentTreeItem.GetScript(EDepartmentLoadingType.AdministratorTree, null));

                BindGrid();
            }
        }

        public void BindGrid()
        {
            try
            {
                rptDepartment.DataSource = BaiRongDataProvider.DepartmentDao.GetDepartmentIdListByParentId(0);
                rptDepartment.ItemDataBound += rptDepartment_ItemDataBound;
                rptDepartment.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        private void rptDepartment_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var departmentId = (int)e.Item.DataItem;
            var departmentInfo = DepartmentManager.GetDepartmentInfo(departmentId);

            var ltlHtml = e.Item.FindControl("ltlHtml") as Literal;

            if (ltlHtml != null)
            {
                ltlHtml.Text = PageDepartment.GetDepartmentRowHtml(departmentInfo, EDepartmentLoadingType.AdministratorTree, _additional);
            }
        }
    }
}
