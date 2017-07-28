using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Permissions;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Admin
{
    public class PageAdministrator : BasePage
    {
        public DropDownList RoleName;
        public DropDownList PageNum;
        public DropDownList Order;
        public TextBox Keyword;
        public DropDownList ddlAreaID;
        public DropDownList LastActivityDate;

        public Repeater rptContents;
        public SqlPager spContents;

        public Button AddButton;
        public Button Lock;
        public Button UnLock;
        public Button Delete;

        private int _departmentId;
        private DepartmentInfo _departmentInfo;
        private bool[] _isLastNodeArrayOfArea;
        private EUserLockType _lockType = EUserLockType.Forever;

        public static string GetRedirectUrl(int departmentId)
        {
            return PageUtils.GetAdminUrl(nameof(PageAdministrator), new NameValueCollection
            {
                {"departmentID", departmentId.ToString()}
            });
        }

        public string GetRolesHtml(string userName)
        {
            return AdminManager.GetRolesHtml(userName);
        }

        public string GetDateTime(DateTime datetime)
        {
            var retval = string.Empty;
            if (datetime > DateUtils.SqlMinValue)
            {
                retval = DateUtils.GetDateString(datetime);
            }
            return retval;
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var permissioins = PermissionsManager.GetPermissions(Body.AdministratorName);

            _departmentId = Body.GetQueryInt("departmentID");
            var areaId = Body.GetQueryInt("areaID");
            if (_departmentId > 0)
            {
                _departmentInfo = DepartmentManager.GetDepartmentInfo(_departmentId);
            }

            if (Body.IsQueryExists("Delete"))
            {
                var userNameCollection = Body.GetQueryString("UserNameCollection");
                try
                {
                    var userNameArrayList = TranslateUtils.StringCollectionToStringList(userNameCollection);
                    foreach (var userName in userNameArrayList)
                    {
                        BaiRongDataProvider.AdministratorDao.Delete(userName);
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
                    BaiRongDataProvider.AdministratorDao.Lock(userNameList);

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
                    BaiRongDataProvider.AdministratorDao.UnLock(userNameList);

                    Body.AddAdminLog("解除锁定管理员", $"管理员:{userNameCollection}");

                    SuccessMessage("成功解除锁定所选管理员！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "解除锁定所选管理员失败！");
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = StringUtils.Constants.PageSize;

            if (string.IsNullOrEmpty(Body.GetQueryString("PageNum")))
            {
                spContents.ItemsPerPage = TranslateUtils.ToInt(PageNum.SelectedValue) == 0 ? StringUtils.Constants.PageSize : TranslateUtils.ToInt(PageNum.SelectedValue);

                spContents.SelectCommand = BaiRongDataProvider.AdministratorDao.GetSelectCommand(permissioins.IsConsoleAdministrator, Body.AdministratorName, _departmentId);
                spContents.SortField = BaiRongDataProvider.AdministratorDao.GetSortFieldName();
                spContents.SortMode = SortMode.ASC;
            }
            else
            {
                spContents.ItemsPerPage = Body.GetQueryInt("PageNum") == 0 ? StringUtils.Constants.PageSize : Body.GetQueryInt("PageNum");
                spContents.SelectCommand = BaiRongDataProvider.AdministratorDao.GetSelectCommand(Body.GetQueryString("Keyword"), Body.GetQueryString("RoleName"), Body.GetQueryInt("LastActivityDate"), permissioins.IsConsoleAdministrator, Body.AdministratorName, _departmentId, Body.GetQueryInt("AreaID"));
                spContents.SortField = Body.GetQueryString("Order");
                spContents.SortMode = StringUtils.EqualsIgnoreCase(spContents.SortField, "UserName") ? SortMode.ASC : SortMode.DESC;
            }

            rptContents.ItemDataBound += rptContents_ItemDataBound;

            _lockType = EUserLockTypeUtils.GetEnumType(ConfigManager.SystemConfigInfo.LoginLockingType);

            if (IsPostBack) return;

            BreadCrumbAdmin(AppManager.Admin.LeftMenu.AdminManagement, "管理员管理", AppManager.Admin.Permission.AdminManagement);

            var theListItem = new ListItem("全部", string.Empty)
            {
                Selected = true
            };
            RoleName.Items.Add(theListItem);

            var allRoles = permissioins.IsConsoleAdministrator ? BaiRongDataProvider.RoleDao.GetAllRoles() : BaiRongDataProvider.RoleDao.GetAllRolesByCreatorUserName(Body.AdministratorName);

            var allPredefinedRoles = EPredefinedRoleUtils.GetAllPredefinedRoleName();
            foreach (var roleName in allRoles)
            {
                if (allPredefinedRoles.Contains(roleName))
                {
                    var listitem = new ListItem(EPredefinedRoleUtils.GetText(EPredefinedRoleUtils.GetEnumType(roleName)), roleName);
                    RoleName.Items.Add(listitem);
                }
            }
            foreach (var roleName in allRoles)
            {
                if (!allPredefinedRoles.Contains(roleName))
                {
                    var listitem = new ListItem(roleName, roleName);
                    RoleName.Items.Add(listitem);
                }
            }

            ddlAreaID.Items.Add(new ListItem("<全部区域>", "0"));
            var areaIdList = AreaManager.GetAreaIdList();
            var count = areaIdList.Count;
            _isLastNodeArrayOfArea = new bool[count];
            foreach (var theAreaId in areaIdList)
            {
                var areaInfo = AreaManager.GetAreaInfo(theAreaId);
                var listitem = new ListItem(GetArea(areaInfo.AreaId, areaInfo.AreaName, areaInfo.ParentsCount, areaInfo.IsLastNode), theAreaId.ToString());
                if (areaId == theAreaId)
                {
                    listitem.Selected = true;
                }
                ddlAreaID.Items.Add(listitem);
            }

            if (Body.IsQueryExists("PageNum"))
            {
                ControlUtils.SelectListItems(RoleName, Body.GetQueryString("RoleName"));
                ControlUtils.SelectListItems(PageNum, Body.GetQueryString("PageNum"));
                Keyword.Text = Body.GetQueryString("Keyword");
                ControlUtils.SelectListItems(ddlAreaID, Body.GetQueryString("AreaID"));
                ControlUtils.SelectListItems(LastActivityDate, Body.GetQueryString("LastActivityDate"));
                ControlUtils.SelectListItems(Order, Body.GetQueryString("Order"));
            }

            var urlAdd = PageAdministratorAdd.GetRedirectUrlToAdd(_departmentId);
            AddButton.Attributes.Add("onclick", $@"location.href='{urlAdd}';return false;");

            var urlAdministrator = GetRedirectUrl(_departmentId);

            Lock.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlAdministrator + "&Lock=True", "UserNameCollection", "UserNameCollection", "请选择需要锁定的管理员！", "此操作将锁定所选管理员，确认吗？"));

            UnLock.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlAdministrator + "&UnLock=True", "UserNameCollection", "UserNameCollection", "请选择需要解除锁定的管理员！", "此操作将解除锁定所选管理员，确认吗？"));

            Delete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlAdministrator + "&Delete=True", "UserNameCollection", "UserNameCollection", "请选择需要删除的管理员！", "此操作将删除所选管理员，确认吗？"));

            spContents.DataBind();
        }

        public string GetArea(int areaId, string areaName, int parentsCount, bool isLastNode)
        {
            var str = "";
            if (isLastNode == false)
            {
                _isLastNodeArrayOfArea[parentsCount] = false;
            }
            else
            {
                _isLastNodeArrayOfArea[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                str = string.Concat(str, _isLastNodeArrayOfArea[i] ? "　" : "│");
            }
            str = string.Concat(str, isLastNode ? "└" : "├");
            str = string.Concat(str, areaName);
            return str;
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var userName = SqlUtils.EvalString(e.Item.DataItem, "UserName");
                var displayName = SqlUtils.EvalString(e.Item.DataItem, "DisplayName");
                var mobile = SqlUtils.EvalString(e.Item.DataItem, "Mobile");
                var departmentId = SqlUtils.EvalInt(e.Item.DataItem, "DepartmentID");
                var areaId = SqlUtils.EvalInt(e.Item.DataItem, "AreaID");
                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = userName;
                }
                var departmentName = string.Empty;
                if (_departmentInfo != null)
                {
                    departmentName = _departmentInfo.DepartmentName;
                }
                else if (departmentId > 0)
                {
                    departmentName = DepartmentManager.GetDepartmentName(departmentId);
                }
                var countOfFailedLogin = SqlUtils.EvalInt(e.Item.DataItem, "CountOfFailedLogin");
                var isLockedOut = SqlUtils.EvalBool(e.Item.DataItem, "IsLockedOut");
                var lastActivityDate = SqlUtils.EvalDateTime(e.Item.DataItem, "LastActivityDate");

                var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
                var ltlDisplayName = (Literal)e.Item.FindControl("ltlDisplayName");
                var ltlMobile = (Literal)e.Item.FindControl("ltlMobile");
                var ltlDepartment = (Literal)e.Item.FindControl("ltlDepartment");
                var ltlArea = (Literal)e.Item.FindControl("ltlArea");
                var ltlEdit = (Literal)e.Item.FindControl("ltlEdit");
                var hlChangePassword = (HyperLink)e.Item.FindControl("hlChangePassword");
                var ltlRole = (Literal)e.Item.FindControl("ltlRole");
                var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");

                ltlUserName.Text = GetUserNameHtml(userName, countOfFailedLogin, isLockedOut, lastActivityDate);
                ltlDisplayName.Text = displayName;
                ltlMobile.Text = mobile;
                ltlDepartment.Text = departmentName;
                ltlArea.Text = AreaManager.GetAreaName(areaId);

                var urlEdit = PageAdministratorAdd.GetRedirectUrlToEdit(departmentId, userName);
                ltlEdit.Text = $@"<a href=""{urlEdit}"">修改属性</a>";
                hlChangePassword.Attributes.Add("onclick", ModalAdminPassword.GetOpenWindowString(userName));

                if (Body.AdministratorName != userName)
                {
                    var openWindowString = ModalPermissionsSet.GetOpenWindowString(userName);
                    ltlRole.Text = $@"<a href=""javascript:;"" onclick=""{openWindowString}"">权限设置</a>";
                    ltlSelect.Text = $@"<input type=""checkbox"" name=""UserNameCollection"" value=""{userName}"" />";
                }
            }
        }

        private string GetUserNameHtml(string userName, int countOfFailedLogin, bool isLockedOut, DateTime lastActivityDate)
        {
            var showPopWinString = ModalAdminView.GetOpenWindowString(userName);
            var state = string.Empty;
            if (isLockedOut)
            {
                state = @"<span style=""color:red;"">[已被锁定]</span>";
            }
            else if (ConfigManager.SystemConfigInfo.IsLoginFailToLock &&
                       ConfigManager.SystemConfigInfo.LoginFailToLockCount <= countOfFailedLogin)
            {
                if (_lockType == EUserLockType.Forever)
                {
                    state = @"<span style=""color:red;"">[已被锁定]</span>";
                }
                else
                {
                    var ts = new TimeSpan(DateTime.Now.Ticks - lastActivityDate.Ticks);
                    var hours = Convert.ToInt32(ConfigManager.SystemConfigInfo.LoginLockingHours - ts.TotalHours);
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
                    var url = GetRedirectUrl(_departmentId);
                    _pageUrl = url +
                                    $"&RoleName={RoleName.SelectedValue}&PageNum={PageNum.SelectedValue}&Keyword={Keyword.Text}&AreaID={ddlAreaID.SelectedValue}&LastActivityDate={LastActivityDate.SelectedValue}&Order={Order.SelectedValue}";
                }
                return _pageUrl;
            }
        }
    }
}
