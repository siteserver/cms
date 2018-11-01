using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAdminDepartment : BasePage
    {
        public Repeater RptContents;

        public Button BtnAdd;
        public Button BtnDelete;

        private int _currentDepartmentId;

        public static string GetRedirectUrl(int currentDepartmentId)
        {
            if (currentDepartmentId != 0)
            {
                return PageUtils.GetSettingsUrl(nameof(PageAdminDepartment), new NameValueCollection
                {
                    {"CurrentDepartmentID", currentDepartmentId.ToString() }
                });
            }
            return PageUtils.GetSettingsUrl(nameof(PageAdminDepartment), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (AuthRequest.IsQueryExists("Delete") && AuthRequest.IsQueryExists("DepartmentIDCollection"))
            {
                var departmentIdArrayList = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("DepartmentIDCollection"));
                foreach (var departmentId in departmentIdArrayList)
                {
                    DataProvider.DepartmentDao.Delete(departmentId);
                }
                SuccessMessage("成功删除所选部门");
            }
            else if (AuthRequest.IsQueryExists("DepartmentID") && (AuthRequest.IsQueryExists("Subtract") || AuthRequest.IsQueryExists("Add")))
            {
                var departmentId = AuthRequest.GetQueryInt("DepartmentID");
                var isSubtract = AuthRequest.IsQueryExists("Subtract");
                DataProvider.DepartmentDao.UpdateTaxis(departmentId, isSubtract);

                PageUtils.Redirect(GetRedirectUrl(departmentId));
                return;
            }

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Admin);

            ClientScriptRegisterClientScriptBlock("NodeTreeScript", DepartmentTreeItem.GetScript(EDepartmentLoadingType.ContentList, null));

            if (AuthRequest.IsQueryExists("CurrentDepartmentID"))
            {
                _currentDepartmentId = AuthRequest.GetQueryInt("CurrentDepartmentID");
                var onLoadScript = GetScriptOnLoad(_currentDepartmentId);
                if (!string.IsNullOrEmpty(onLoadScript))
                {
                    ClientScriptRegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                }
            }

            BtnAdd.Attributes.Add("onclick", ModalDepartmentAdd.GetOpenWindowStringToAdd(GetRedirectUrl(0)));

            BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUtils.GetSettingsUrl(nameof(PageAdminDepartment), new NameValueCollection
            {
                {"Delete", "True" }
            }), "DepartmentIDCollection", "DepartmentIDCollection", "请选择需要删除的部门！", "此操作将删除对应部门以及所有下级部门，确认删除吗？"));

            RptContents.DataSource = DataProvider.DepartmentDao.GetIdListByParentId(0);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        public string GetScriptOnLoad(int currentDepartmentId)
        {
            if (currentDepartmentId != 0)
            {
                var departmentInfo = DepartmentManager.GetDepartmentInfo(currentDepartmentId);
                if (departmentInfo != null)
                {
                    string path;
                    if (departmentInfo.ParentsCount <= 1)
                    {
                        path = currentDepartmentId.ToString();
                    }
                    else
                    {
                        path = departmentInfo.ParentsPath.Substring(departmentInfo.ParentsPath.IndexOf(",", StringComparison.Ordinal) + 1) + "," + currentDepartmentId;
                    }
                    return DepartmentTreeItem.GetScriptOnLoad(path);
                }
            }
            return string.Empty;
        }

        private static void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var departmentId = (int)e.Item.DataItem;

            var departmentInfo = DepartmentManager.GetDepartmentInfo(departmentId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = GetDepartmentRowHtml(departmentInfo, EDepartmentLoadingType.ContentList, null);
        }

        public static string GetDepartmentRowHtml(DepartmentInfo departmentInfo, EDepartmentLoadingType loadingType, NameValueCollection additional)
        {
            var treeItem = DepartmentTreeItem.CreateInstance(departmentInfo);
            var title = treeItem.GetItemHtml(loadingType, additional, false);

            var rowHtml = string.Empty;

            if (loadingType == EDepartmentLoadingType.AdministratorTree || loadingType == EDepartmentLoadingType.DepartmentSelect || loadingType == EDepartmentLoadingType.ContentTree)
            {
                rowHtml = $@"
<tr treeItemLevel=""{departmentInfo.ParentsCount + 1}"">
	<td nowrap>{title}</td>
</tr>
";
            }
            else if (loadingType == EDepartmentLoadingType.ContentList)
            {
                string editUrl = $@"<a href=""javascript:;"" onclick=""{ModalDepartmentAdd.GetOpenWindowStringToEdit(
                    departmentInfo.Id, GetRedirectUrl(departmentInfo.Id))}"">编辑</a>";

                var urlUp = PageUtils.GetSettingsUrl(nameof(PageAdminDepartment), new NameValueCollection
                {
                    {"Subtract", "True"},
                    {"DepartmentID", departmentInfo.Id.ToString()}
                });
                string upLink = $@"<a href=""{urlUp}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";

                var urlDown = PageUtils.GetSettingsUrl(nameof(PageAdminDepartment), new NameValueCollection
                {
                    {"Add", "True"},
                    {"DepartmentID", departmentInfo.Id.ToString()}
                });
                string downLink = $@"<a href=""{urlDown}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";

                string checkBoxHtml = $"<input type='checkbox' name='DepartmentIDCollection' value='{departmentInfo.Id}' />";

                rowHtml = $@"
<tr treeItemLevel=""{departmentInfo.ParentsCount + 1}"">
    <td>{title}</td>
    <td>&nbsp;{departmentInfo.Code}</td>
    <td class=""text-center"">{departmentInfo.CountOfAdmin}</td>
    <td class=""text-center"">{upLink}</td>
    <td class=""text-center"">{downLink}</td>
    <td class=""text-center"">{editUrl}</td>
    <td class=""text-center"">{checkBoxHtml}</td>
</tr>
";
            }
            return rowHtml;
        }
    }
}
