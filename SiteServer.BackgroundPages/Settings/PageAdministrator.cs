using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAdministrator : BasePage
    {
        public DropDownList DdlRoleName;
        public DropDownList DdlPageNum;
        public DropDownList DdlOrder;
        public DropDownList DdlLastActivityDate;
        public DropDownList DdlDepartmentId;
        public DropDownList DdlAreaId;
        public TextBox TbKeyword;

        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnAdd;
        public Button BtnLock;
        public Button BtnUnLock;
        public Button BtnDelete;

        private bool[] _isLastNodeArrayOfDepartment;
        private bool[] _isLastNodeArrayOfArea;
        private EUserLockType _lockType = EUserLockType.Forever;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageAdministrator), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var permissioins = PermissionsManager.GetPermissions(Body.AdminName);

            var departmentId = Body.GetQueryInt("departmentId");
            var areaId = Body.GetQueryInt("areaId");

            if (Body.IsQueryExists("Delete"))
            {
                var userNameCollection = Body.GetQueryString("UserNameCollection");
                try
                {
                    var userNameArrayList = TranslateUtils.StringCollectionToStringList(userNameCollection);
                    foreach (var userName in userNameArrayList)
                    {
                        DataProvider.AdministratorDao.Delete(userName);
                    }

                    Body.AddAdminLog("删除管理员", $"管理员:{userNameCollection}");

                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
            else if (Body.IsQueryExists("Lock"))
            {
                var userNameCollection = Body.GetQueryString("UserNameCollection");
                try
                {
                    var userNameList = TranslateUtils.StringCollectionToStringList(userNameCollection);
                    DataProvider.AdministratorDao.Lock(userNameList);

                    Body.AddAdminLog("锁定管理员", $"管理员:{userNameCollection}");

                    SuccessMessage("成功锁定所选管理员！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "锁定所选管理员失败！");
                }
            }
            else if (Body.IsQueryExists("UnLock"))
            {
                var userNameCollection = Body.GetQueryString("UserNameCollection");
                try
                {
                    var userNameList = TranslateUtils.StringCollectionToStringList(userNameCollection);
                    DataProvider.AdministratorDao.UnLock(userNameList);

                    Body.AddAdminLog("解除锁定管理员", $"管理员:{userNameCollection}");

                    SuccessMessage("成功解除锁定所选管理员！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "解除锁定所选管理员失败！");
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = StringUtils.Constants.PageSize;

            if (string.IsNullOrEmpty(Body.GetQueryString("pageNum")))
            {
                SpContents.ItemsPerPage = TranslateUtils.ToInt(DdlPageNum.SelectedValue) == 0 ? StringUtils.Constants.PageSize : TranslateUtils.ToInt(DdlPageNum.SelectedValue);

                SpContents.SelectCommand = DataProvider.AdministratorDao.GetSelectCommand(permissioins.IsConsoleAdministrator, Body.AdminName);
                SpContents.SortField = DataProvider.AdministratorDao.GetSortFieldName();
                SpContents.SortMode = SortMode.ASC;
            }
            else
            {
                SpContents.ItemsPerPage = Body.GetQueryInt("pageNum") == 0 ? StringUtils.Constants.PageSize : Body.GetQueryInt("pageNum");
                SpContents.SelectCommand = DataProvider.AdministratorDao.GetSelectCommand(Body.GetQueryString("keyword"), Body.GetQueryString("roleName"), Body.GetQueryInt("lastActivityDate"), permissioins.IsConsoleAdministrator, Body.AdminName, Body.GetQueryInt("departmentId"), Body.GetQueryInt("areaId"));
                SpContents.SortField = Body.GetQueryString("order");
                SpContents.SortMode = StringUtils.EqualsIgnoreCase(SpContents.SortField, nameof(AdministratorInfo.UserName)) ? SortMode.ASC : SortMode.DESC;
            }

            RptContents.ItemDataBound += RptContents_ItemDataBound;

            _lockType = EUserLockTypeUtils.GetEnumType(ConfigManager.SystemConfigInfo.AdminLockLoginType);

            if (IsPostBack) return;

            VerifyAdministratorPermissions(ConfigManager.Permissions.Settings.Admin);

            var theListItem = new ListItem("全部", string.Empty)
            {
                Selected = true
            };
            DdlRoleName.Items.Add(theListItem);

            var allRoles = permissioins.IsConsoleAdministrator ? DataProvider.RoleDao.GetAllRoles() : DataProvider.RoleDao.GetAllRolesByCreatorUserName(Body.AdminName);

            var allPredefinedRoles = EPredefinedRoleUtils.GetAllPredefinedRoleName();
            foreach (var roleName in allRoles)
            {
                if (allPredefinedRoles.Contains(roleName))
                {
                    var listitem = new ListItem(EPredefinedRoleUtils.GetText(EPredefinedRoleUtils.GetEnumType(roleName)), roleName);
                    DdlRoleName.Items.Add(listitem);
                }
            }
            foreach (var roleName in allRoles)
            {
                if (!allPredefinedRoles.Contains(roleName))
                {
                    var listitem = new ListItem(roleName, roleName);
                    DdlRoleName.Items.Add(listitem);
                }
            }

            DdlDepartmentId.Items.Add(new ListItem("<所有部门>", "0"));
            var departmentIdList = DepartmentManager.GetDepartmentIdList();
            var count = departmentIdList.Count;
            _isLastNodeArrayOfDepartment = new bool[count];
            foreach (var theDepartmentId in departmentIdList)
            {
                var departmentInfo = DepartmentManager.GetDepartmentInfo(theDepartmentId);
                DdlDepartmentId.Items.Add(new ListItem(GetTreeItem(departmentInfo.Id, departmentInfo.DepartmentName, departmentInfo.ParentsCount, departmentInfo.IsLastNode, _isLastNodeArrayOfDepartment), theDepartmentId.ToString()));
            }
            ControlUtils.SelectSingleItem(DdlDepartmentId, departmentId.ToString());

            DdlAreaId.Items.Add(new ListItem("<全部区域>", "0"));
            var areaIdList = AreaManager.GetAreaIdList();
            count = areaIdList.Count;
            _isLastNodeArrayOfArea = new bool[count];
            foreach (var theAreaId in areaIdList)
            {
                var areaInfo = AreaManager.GetAreaInfo(theAreaId);
                DdlAreaId.Items.Add(new ListItem(GetTreeItem(areaInfo.Id, areaInfo.AreaName, areaInfo.ParentsCount, areaInfo.IsLastNode, _isLastNodeArrayOfArea), theAreaId.ToString()));
            }
            ControlUtils.SelectSingleItem(DdlAreaId, areaId.ToString());

            if (Body.IsQueryExists("pageNum"))
            {
                ControlUtils.SelectSingleItem(DdlRoleName, Body.GetQueryString("roleName"));
                ControlUtils.SelectSingleItem(DdlPageNum, Body.GetQueryString("pageNum"));
                TbKeyword.Text = Body.GetQueryString("keyword");
                ControlUtils.SelectSingleItem(DdlDepartmentId, Body.GetQueryString("departmentId"));
                ControlUtils.SelectSingleItem(DdlAreaId, Body.GetQueryString("areaId"));
                ControlUtils.SelectSingleItem(DdlLastActivityDate, Body.GetQueryString("lastActivityDate"));
                ControlUtils.SelectSingleItem(DdlOrder, Body.GetQueryString("order"));
            }

            BtnAdd.Attributes.Add("onclick", $@"location.href='{PageAdministratorAdd.GetRedirectUrlToAdd(departmentId)}';return false;");

            var urlAdministrator = GetRedirectUrl();

            BtnLock.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlAdministrator + "&Lock=True", "UserNameCollection", "UserNameCollection", "请选择需要锁定的管理员！", "此操作将锁定所选管理员，确认吗？"));

            BtnUnLock.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlAdministrator + "&UnLock=True", "UserNameCollection", "UserNameCollection", "请选择需要解除锁定的管理员！", "此操作将解除锁定所选管理员，确认吗？"));

            BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlAdministrator + "&Delete=True", "UserNameCollection", "UserNameCollection", "请选择需要删除的管理员！", "此操作将删除所选管理员，确认吗？"));

            SpContents.DataBind();
        }

        public string GetTreeItem(int areaId, string areaName, int parentsCount, bool isLastNode, bool[] array)
        {
            var str = "";
            if (isLastNode == false)
            {
                array[parentsCount] = false;
            }
            else
            {
                array[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                str = string.Concat(str, array[i] ? "　" : "│");
            }
            str = string.Concat(str, isLastNode ? "└" : "├");
            str = string.Concat(str, areaName);
            return str;
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var userName = SqlUtils.EvalString(e.Item.DataItem, nameof(AdministratorInfo.UserName));
            var displayName = SqlUtils.EvalString(e.Item.DataItem, nameof(AdministratorInfo.DisplayName));
            var mobile = SqlUtils.EvalString(e.Item.DataItem, nameof(AdministratorInfo.Mobile));
            var departmentId = SqlUtils.EvalInt(e.Item.DataItem, nameof(AdministratorInfo.DepartmentId));
            var areaId = SqlUtils.EvalInt(e.Item.DataItem, nameof(AdministratorInfo.AreaId));
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = userName;
            }
            var countOfFailedLogin = SqlUtils.EvalInt(e.Item.DataItem, nameof(AdministratorInfo.CountOfFailedLogin));
            var countOfLogin = SqlUtils.EvalInt(e.Item.DataItem, nameof(AdministratorInfo.CountOfLogin));
            var isLockedOut = SqlUtils.EvalBool(e.Item.DataItem, nameof(AdministratorInfo.IsLockedOut));
            var lastActivityDate = SqlUtils.EvalDateTime(e.Item.DataItem, nameof(AdministratorInfo.LastActivityDate));

            var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
            var ltlDisplayName = (Literal)e.Item.FindControl("ltlDisplayName");
            var ltlMobile = (Literal)e.Item.FindControl("ltlMobile");
            var ltlDepartment = (Literal)e.Item.FindControl("ltlDepartment");
            var ltlArea = (Literal)e.Item.FindControl("ltlArea");
            var ltlLastActivityDate = (Literal)e.Item.FindControl("ltlLastActivityDate");
            var ltlCountOfLogin = (Literal)e.Item.FindControl("ltlCountOfLogin");
            var ltlRoles = (Literal)e.Item.FindControl("ltlRoles");
            var ltlEdit = (Literal)e.Item.FindControl("ltlEdit");
            var hlChangePassword = (HyperLink)e.Item.FindControl("hlChangePassword");
            var ltlRole = (Literal)e.Item.FindControl("ltlRole");
            var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");

            ltlUserName.Text = GetUserNameHtml(userName, countOfFailedLogin, isLockedOut, lastActivityDate);
            ltlDisplayName.Text = displayName;
            ltlMobile.Text = mobile;
            ltlDepartment.Text = DepartmentManager.GetDepartmentName(departmentId);
            ltlArea.Text = AreaManager.GetAreaName(areaId);

            ltlLastActivityDate.Text = GetDateTime(lastActivityDate);
            ltlCountOfLogin.Text = countOfLogin.ToString();
            ltlRoles.Text = AdminManager.GetRolesHtml(userName);

            var urlEdit = PageAdministratorAdd.GetRedirectUrlToEdit(departmentId, userName);
            ltlEdit.Text = $@"<a href=""{urlEdit}"">修改属性</a>";
            hlChangePassword.Attributes.Add("onclick", ModalAdminPassword.GetOpenWindowString(userName));

            if (Body.AdminName != userName)
            {
                var openWindowString = ModalPermissionsSet.GetOpenWindowString(userName);
                ltlRole.Text = $@"<a href=""javascript:;"" onclick=""{openWindowString}"">权限设置</a>";
                ltlSelect.Text = $@"<input type=""checkbox"" name=""UserNameCollection"" value=""{userName}"" />";
            }
        }

        private static string GetDateTime(DateTime datetime)
        {
            var retval = string.Empty;
            if (datetime > DateUtils.SqlMinValue)
            {
                retval = DateUtils.GetDateString(datetime);
            }
            return retval;
        }

        private string GetUserNameHtml(string userName, int countOfFailedLogin, bool isLockedOut, DateTime lastActivityDate)
        {
            var showPopWinString = ModalAdminView.GetOpenWindowString(userName);
            var state = string.Empty;
            if (isLockedOut)
            {
                state = @"<span style=""color:red;"">[已被锁定]</span>";
            }
            else if (ConfigManager.SystemConfigInfo.IsAdminLockLogin &&
                       ConfigManager.SystemConfigInfo.AdminLockLoginCount <= countOfFailedLogin)
            {
                if (_lockType == EUserLockType.Forever)
                {
                    state = @"<span style=""color:red;"">[已被锁定]</span>";
                }
                else
                {
                    var ts = new TimeSpan(DateTime.Now.Ticks - lastActivityDate.Ticks);
                    var hours = Convert.ToInt32(ConfigManager.SystemConfigInfo.AdminLockLoginHours - ts.TotalHours);
                    if (hours > 0)
                    {
                        state = $@"<span style=""color:red;"">[错误登录次数过多，已被锁定{hours}小时]</span>";
                    }
                }
            }
            return $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">{userName}</a> {state}";
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUrl);
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = $"{GetRedirectUrl()}?roleName={DdlRoleName.SelectedValue}&pageNum={DdlPageNum.SelectedValue}&keyword={TbKeyword.Text}&departmentId={DdlDepartmentId.SelectedValue}&areaId={DdlAreaId.SelectedValue}&lastActivityDate={DdlLastActivityDate.SelectedValue}&order={DdlOrder.SelectedValue}";
                }
                return _pageUrl;
            }
        }
    }
}
