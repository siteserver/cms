using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.BackgroundPages.Settings
{
	public class ModalAdminSelect : BasePage
    {
        public Repeater RptDepartment;
        public Literal LtlDepartment;
        public Repeater RptUser;

        private readonly NameValueCollection _additional = new NameValueCollection();
        private int _departmentId;
        private string _scriptName;

        public static string GetShowPopWinString(int departmentId, string scriptName)
        {
            return LayerUtils.GetOpenScript("管理员选择", PageUtils.GetSettingsUrl(nameof(ModalAdminSelect), new NameValueCollection
            {
                {"departmentID", departmentId.ToString()},
                {"scriptName", scriptName}
            }), 460, 400);
        }

		public void Page_Load(object sender, EventArgs e)
		{
            _departmentId = AuthRequest.GetQueryInt("departmentID");
            _scriptName = AuthRequest.GetQueryString("ScriptName");
		    var url = PageUtils.GetSettingsUrl(nameof(ModalAdminSelect), new NameValueCollection
            {
                {"scriptName", _scriptName}
            });
            _additional.Add("UrlFormatString", url);

			if (!IsPostBack)
			{
                LtlDepartment.Text = "管理员列表";
                if (AuthRequest.IsQueryExists("UserName"))
                {
                    var userName = AuthRequest.GetQueryString("UserName");
                    var displayName = AdminManager.GetDisplayName(userName, true);
                    string scripts = $"window.parent.{_scriptName}('{displayName}', '{userName}');";
                    LayerUtils.CloseWithoutRefresh(Page, scripts);
                }
                else if (AuthRequest.IsQueryExists("departmentID"))
                {
                    if (_departmentId > 0)
                    {
                        LtlDepartment.Text = DepartmentManager.GetDepartmentName(_departmentId);
                        RptUser.DataSource = DataProvider.AdministratorDao.GetUserNameList(_departmentId, false);
                        RptUser.ItemDataBound += RptUser_ItemDataBound;
                        RptUser.DataBind();
                    }
                }
                else
                {
                    ClientScriptRegisterClientScriptBlock("NodeTreeScript", DepartmentTreeItem.GetScript(EDepartmentLoadingType.DepartmentSelect, _additional));
                }
			}

            BindGrid();
		}

        public void BindGrid()
        {
            RptDepartment.DataSource = DataProvider.DepartmentDao.GetIdListByParentId(0);
            RptDepartment.ItemDataBound += rptDepartment_ItemDataBound;
            RptDepartment.DataBind();
        }

        private void rptDepartment_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var departmentId = (int)e.Item.DataItem;
            var departmentInfo = DepartmentManager.GetDepartmentInfo(departmentId);

            var ltlHtml = e.Item.FindControl("ltlHtml") as Literal;

            if (ltlHtml != null)
            {
                ltlHtml.Text = PageAdminDepartment.GetDepartmentRowHtml(departmentInfo, EDepartmentLoadingType.DepartmentSelect, _additional);
            }
        }

        private void RptUser_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var userName = (string)e.Item.DataItem;

            var ltlUrl = e.Item.FindControl("ltlUrl") as Literal;

            var url = PageUtils.GetSettingsUrl(nameof(ModalAdminSelect), new NameValueCollection
            {
                {"scriptName", _scriptName},
                {"UserName", userName}
            });

            if (ltlUrl != null) ltlUrl.Text = $"<a href='{url}'>{AdminManager.GetDisplayName(userName, false)}</a>";
        }
	}
}
