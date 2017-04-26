using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Admin
{
	public class ModalDepartmentSelect : BasePage
    {
        public Repeater rptCategory;

        private readonly NameValueCollection _additional = new NameValueCollection();

	    public static string GetShowPopWinString(int projectId)
	    {
	        return PageUtils.GetOpenWindowString("设置分类",
	            PageUtils.GetAdminUrl(nameof(ModalDepartmentSelect), new NameValueCollection
	            {
	                {"ProjectID", projectId.ToString()}
	            }), 460, 260, true);
	    }

	    public void Page_Load(object sender, EventArgs e)
		{
		    _additional.Add("UrlFormatString", PageUtils.GetAdminUrl(nameof(ModalDepartmentSelect), null));

			if (!IsPostBack)
			{
                if (Body.IsQueryExists("DepartmentID"))
                {
                    var departmentId = Body.GetQueryInt("DepartmentID");
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
                rptCategory.DataSource = BaiRongDataProvider.DepartmentDao.GetDepartmentIdListByParentId(0);
                rptCategory.ItemDataBound += rptCategory_ItemDataBound;
                rptCategory.DataBind();
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
