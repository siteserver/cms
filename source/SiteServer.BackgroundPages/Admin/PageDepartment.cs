using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.BackgroundPages.Core;
using SiteServer.BackgroundPages.Wcm;

namespace SiteServer.BackgroundPages.Admin
{
    public class PageDepartment : BasePage
    {
        public Repeater rptContents;

        public Button AddChannel;
        public Button Translate;
        public Button Import;
        public Button Export;
        public Button Delete;

        private int _currentDepartmentId;

        public static string GetRedirectUrl(int currentDepartmentId)
        {
            if (currentDepartmentId != 0)
            {
                return PageUtils.GetAdminUrl(nameof(PageDepartment), new NameValueCollection
                {
                    {"CurrentDepartmentID", currentDepartmentId.ToString() }
                });
            }
            return PageUtils.GetAdminUrl(nameof(PageDepartment), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("Delete") && Body.IsQueryExists("DepartmentIDCollection"))
            {
                var departmentIdArrayList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("DepartmentIDCollection"));
                foreach (var departmentId in departmentIdArrayList)
                {
                    BaiRongDataProvider.DepartmentDao.Delete(departmentId);
                }
                SuccessMessage("成功删除所选部门");
            }
            else if (Body.IsQueryExists("DepartmentID") && (Body.IsQueryExists("Subtract") || Body.IsQueryExists("Add")))
            {
                var departmentId = Body.GetQueryInt("DepartmentID");
                var isSubtract = Body.IsQueryExists("Subtract");
                BaiRongDataProvider.DepartmentDao.UpdateTaxis(departmentId, isSubtract);

                PageUtils.Redirect(GetRedirectUrl(departmentId));
                return;
            }

            if (!IsPostBack)
            {
                BreadCrumbAdmin(AppManager.Admin.LeftMenu.AdminConfiguration, "所属部门管理", AppManager.Admin.Permission.AdminConfiguration);

                ClientScriptRegisterClientScriptBlock("NodeTreeScript", DepartmentTreeItem.GetScript(EDepartmentLoadingType.ContentList, null));

                if (Body.IsQueryExists("CurrentDepartmentID"))
                {
                    _currentDepartmentId = Body.GetQueryInt("CurrentDepartmentID");
                    var onLoadScript = GetScriptOnLoad(_currentDepartmentId);
                    if (!string.IsNullOrEmpty(onLoadScript))
                    {
                        ClientScriptRegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                    }
                }

                AddChannel.Attributes.Add("onclick", ModalDepartmentAdd.GetOpenWindowStringToAdd(GetRedirectUrl(0)));

                Delete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUtils.GetAdminUrl(nameof(PageDepartment), new NameValueCollection
                {
                    {"Delete", "True" }
                }), "DepartmentIDCollection", "DepartmentIDCollection", "请选择需要删除的部门！", "此操作将删除对应部门以及所有下级部门，确认删除吗？"));

                BindGrid();
            }
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

        public void BindGrid()
        {
            try
            {
                rptContents.DataSource = BaiRongDataProvider.DepartmentDao.GetDepartmentIdListByParentId(0);
                rptContents.ItemDataBound += rptContents_ItemDataBound;
                rptContents.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
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
                    departmentInfo.DepartmentId, GetRedirectUrl(departmentInfo.DepartmentId))}"">编辑</a>";

                var urlUp = PageUtils.GetAdminUrl(nameof(PageDepartment), new NameValueCollection
                {
                    {"Subtract", "True"},
                    {"DepartmentID", departmentInfo.DepartmentId.ToString()}
                });
                string upLink = $@"<a href=""{urlUp}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";

                var urlDown = PageUtils.GetAdminUrl(nameof(PageDepartment), new NameValueCollection
                {
                    {"Add", "True"},
                    {"DepartmentID", departmentInfo.DepartmentId.ToString()}
                });
                string downLink = $@"<a href=""{urlDown}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";

                string checkBoxHtml = $"<input type='checkbox' name='DepartmentIDCollection' value='{departmentInfo.DepartmentId}' />";

                rowHtml = $@"
<tr treeItemLevel=""{departmentInfo.ParentsCount + 1}"">
    <td>{title}</td>
    <td>&nbsp;{departmentInfo.Code}</td>
    <td class=""center"">{departmentInfo.CountOfAdmin}</td>
    <td class=""center"">{upLink}</td>
    <td class=""center"">{downLink}</td>
    <td class=""center"">{editUrl}</td>
    <td class=""center"">{checkBoxHtml}</td>
</tr>
";
            }
            else if (loadingType == EDepartmentLoadingType.GovPublicDepartment)
            {
                var publishmentSystemId = TranslateUtils.ToInt(additional["PublishmentSystemID"]);

                var returnUrl = PageGovPublicDepartment.GetRedirectUrl(publishmentSystemId, departmentInfo.DepartmentId);

                string editUrl = $@"<a href=""javascript:;"" onclick=""{ModalDepartmentAdd.GetOpenWindowStringToEdit(
                    departmentInfo.DepartmentId, returnUrl)}"">编辑</a>";

                var urlUp = PageUtils.GetWcmUrl(nameof(PageGovPublicDepartment), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"DepartmentID", departmentInfo.DepartmentId.ToString()},
                    {"Subtract", true.ToString()}
                });
                string upLink = $@"<a href=""{urlUp}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";

                var urlDown = PageUtils.GetWcmUrl(nameof(PageGovPublicDepartment), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"DepartmentID", departmentInfo.DepartmentId.ToString()},
                    {"Add", true.ToString()}
                });
                string downLink = $@"<a href=""{urlDown}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";

                string checkBoxHtml = $"<input type='checkbox' name='DepartmentIDCollection' value='{departmentInfo.DepartmentId}' />";

                rowHtml = $@"
<tr treeItemLevel=""{departmentInfo.ParentsCount + 1}"">
    <td>{title}</td>
    <td>&nbsp;{departmentInfo.Code}</td>
    <td class=""center"">{upLink}</td>
    <td class=""center"">{downLink}</td>
    <td class=""center"">{editUrl}</td>
    <td class=""center"">{checkBoxHtml}</td>
</tr>
";
            }
            return rowHtml;
        }
    }
}
