using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
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
        public Pager PgContents;
        
        public Button BtnLock;
        public Button BtnUnLock;
        public Button BtnDelete;

        private readonly Dictionary<int, bool> _parentsCountDictOfDepartment = new Dictionary<int, bool>();
        private readonly Dictionary<int, bool> _parentsCountDictOfArea = new Dictionary<int, bool>();
        private EUserLockType _lockType = EUserLockType.Forever;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageAdministrator), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var pageNum = AuthRequest.GetQueryInt("pageNum") == 0 ? 30 : AuthRequest.GetQueryInt("pageNum");
            var keyword = AuthRequest.GetQueryString("keyword");
            var roleName = AuthRequest.GetQueryString("roleName");
            var lastActivityDate = AuthRequest.GetQueryInt("lastActivityDate");
            var isConsoleAdministrator = AuthRequest.AdminPermissionsImpl.IsConsoleAdministrator;
            var adminName = AuthRequest.AdminName;
            var order = AuthRequest.IsQueryExists("order") ? AuthRequest.GetQueryString("order") : nameof(AdministratorInfo.UserName);
            var departmentId = AuthRequest.GetQueryInt("departmentId");
            var areaId = AuthRequest.GetQueryInt("areaId");

            if (AuthRequest.IsQueryExists("Delete"))
            {
                var userNameCollection = AuthRequest.GetQueryString("UserNameCollection");
                try
                {
                    var userNameArrayList = TranslateUtils.StringCollectionToStringList(userNameCollection);
                    foreach (var userName in userNameArrayList)
                    {
                        var adminInfo = AdminManager.GetAdminInfoByUserName(userName);
                        DataProvider.AdministratorDao.Delete(adminInfo);
                    }

                    AuthRequest.AddAdminLog("删除管理员", $"管理员:{userNameCollection}");

                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
            else if (AuthRequest.IsQueryExists("Lock"))
            {
                var userNameCollection = AuthRequest.GetQueryString("UserNameCollection");
                try
                {
                    var userNameList = TranslateUtils.StringCollectionToStringList(userNameCollection);
                    DataProvider.AdministratorDao.Lock(userNameList);

                    AuthRequest.AddAdminLog("锁定管理员", $"管理员:{userNameCollection}");

                    SuccessMessage("成功锁定所选管理员！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "锁定所选管理员失败！");
                }
            }
            else if (AuthRequest.IsQueryExists("UnLock"))
            {
                var userNameCollection = AuthRequest.GetQueryString("UserNameCollection");
                try
                {
                    var userNameList = TranslateUtils.StringCollectionToStringList(userNameCollection);
                    DataProvider.AdministratorDao.UnLock(userNameList);

                    AuthRequest.AddAdminLog("解除锁定管理员", $"管理员:{userNameCollection}");

                    SuccessMessage("成功解除锁定所选管理员！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "解除锁定所选管理员失败！");
                }
            }

            PgContents.Param = new PagerParam
            {
                ControlToPaginate = RptContents,
                TableName = DataProvider.AdministratorDao.TableName,
                PageSize = pageNum,
                Page = AuthRequest.GetQueryInt(Pager.QueryNamePage, 1),
                OrderSqlString = DataProvider.AdministratorDao.GetOrderSqlString(order),
                ReturnColumnNames = SqlUtils.Asterisk,
                WhereSqlString = DataProvider.AdministratorDao.GetWhereSqlString(isConsoleAdministrator, adminName, keyword, roleName, lastActivityDate, departmentId, areaId)
            };

            PgContents.Param.TotalCount =
                DataProvider.DatabaseDao.GetPageTotalCount(DataProvider.AdministratorDao.TableName, PgContents.Param.WhereSqlString);

            RptContents.ItemDataBound += RptContents_ItemDataBound;

            _lockType = EUserLockTypeUtils.GetEnumType(ConfigManager.SystemConfigInfo.AdminLockLoginType);

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Admin);

            var theListItem = new ListItem("全部", string.Empty)
            {
                Selected = true
            };
            DdlRoleName.Items.Add(theListItem);

            var allRoles = AuthRequest.AdminPermissionsImpl.IsConsoleAdministrator ? DataProvider.RoleDao.GetRoleNameList() : DataProvider.RoleDao.GetRoleNameListByCreatorUserName(AuthRequest.AdminName);

            var allPredefinedRoles = EPredefinedRoleUtils.GetAllPredefinedRoleName();
            foreach (var theRoleName in allRoles)
            {
                if (allPredefinedRoles.Contains(theRoleName))
                {
                    var listitem = new ListItem(EPredefinedRoleUtils.GetText(EPredefinedRoleUtils.GetEnumType(theRoleName)), theRoleName);
                    DdlRoleName.Items.Add(listitem);
                }
                else
                {
                    var listitem = new ListItem(theRoleName, theRoleName);
                    DdlRoleName.Items.Add(listitem);
                }
            }

            DdlDepartmentId.Items.Add(new ListItem("<所有部门>", "0"));
            var departmentIdList = DepartmentManager.GetDepartmentIdList();
            foreach (var theDepartmentId in departmentIdList)
            {
                var departmentInfo = DepartmentManager.GetDepartmentInfo(theDepartmentId);
                DdlDepartmentId.Items.Add(new ListItem(GetTreeItem(departmentInfo.DepartmentName, departmentInfo.ParentsCount, departmentInfo.IsLastNode, _parentsCountDictOfDepartment), theDepartmentId.ToString()));
            }
            ControlUtils.SelectSingleItem(DdlDepartmentId, departmentId.ToString());

            DdlAreaId.Items.Add(new ListItem("<全部区域>", "0"));
            var areaIdList = AreaManager.GetAreaIdList();
            foreach (var theAreaId in areaIdList)
            {
                var areaInfo = AreaManager.GetAreaInfo(theAreaId);
                DdlAreaId.Items.Add(new ListItem(GetTreeItem(areaInfo.AreaName, areaInfo.ParentsCount, areaInfo.IsLastNode, _parentsCountDictOfArea), theAreaId.ToString()));
            }
            ControlUtils.SelectSingleItem(DdlAreaId, areaId.ToString());

            ControlUtils.SelectSingleItem(DdlRoleName, roleName);
            ControlUtils.SelectSingleItem(DdlPageNum, pageNum.ToString());
            TbKeyword.Text = keyword;
            ControlUtils.SelectSingleItem(DdlDepartmentId, departmentId.ToString());
            ControlUtils.SelectSingleItem(DdlAreaId, areaId.ToString());
            ControlUtils.SelectSingleItem(DdlLastActivityDate, lastActivityDate.ToString());
            ControlUtils.SelectSingleItem(DdlOrder, order);

            PgContents.DataBind();

            var urlAdministrator = GetRedirectUrl();

            BtnLock.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlAdministrator + "?Lock=True", "UserNameCollection", "UserNameCollection", "请选择需要锁定的管理员！", "此操作将锁定所选管理员，确认吗？"));

            BtnUnLock.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlAdministrator + "?UnLock=True", "UserNameCollection", "UserNameCollection", "请选择需要解除锁定的管理员！", "此操作将解除锁定所选管理员，确认吗？"));

            BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlAdministrator + "?Delete=True", "UserNameCollection", "UserNameCollection", "请选择需要删除的管理员！", "此操作将删除所选管理员，确认吗？"));
        }

        public string GetTreeItem(string areaName, int parentsCount, bool isLastNode, Dictionary<int, bool> parentsCountDict)
        {
            var str = "";
            if (isLastNode == false)
            {
                parentsCountDict[parentsCount] = false;
            }
            else
            {
                parentsCountDict[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                str = string.Concat(str, TranslateUtils.DictGetValue(parentsCountDict, i) ? "　" : "│");
            }
            str = string.Concat(str, isLastNode ? "└" : "├");
            str = string.Concat(str, areaName);
            return str;
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var userId = SqlUtils.EvalInt(e.Item.DataItem, nameof(AdministratorInfo.Id));
            var userName = SqlUtils.EvalString(e.Item.DataItem, nameof(AdministratorInfo.UserName));
            var displayName = SqlUtils.EvalString(e.Item.DataItem, nameof(AdministratorInfo.DisplayName));
            var mobile = SqlUtils.EvalString(e.Item.DataItem, nameof(AdministratorInfo.Mobile));
            var avatarUrl = SqlUtils.EvalString(e.Item.DataItem, nameof(AdministratorInfo.AvatarUrl));
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

            var ltlAvatar = (Literal)e.Item.FindControl("ltlAvatar");
            var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
            var ltlDisplayName = (Literal)e.Item.FindControl("ltlDisplayName");
            var ltlMobile = (Literal)e.Item.FindControl("ltlMobile");
            var ltlDepartment = (Literal)e.Item.FindControl("ltlDepartment");
            var ltlArea = (Literal)e.Item.FindControl("ltlArea");
            var ltlLastActivityDate = (Literal)e.Item.FindControl("ltlLastActivityDate");
            var ltlCountOfLogin = (Literal)e.Item.FindControl("ltlCountOfLogin");
            var ltlRoles = (Literal)e.Item.FindControl("ltlRoles");
            var ltlActions = (Literal)e.Item.FindControl("ltlActions");
            var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");

            ltlAvatar.Text = $@"<img src=""{(!string.IsNullOrEmpty(avatarUrl) ? avatarUrl : "../assets/images/default_avatar.png")}"" class=""rounded-circle"" style=""height: 36px; width: 36px;""/>";
            ltlUserName.Text = GetUserNameHtml(userId, userName, countOfFailedLogin, isLockedOut, lastActivityDate);
            ltlDisplayName.Text = displayName;
            ltlMobile.Text = mobile;
            ltlDepartment.Text = DepartmentManager.GetDepartmentName(departmentId);
            ltlArea.Text = AreaManager.GetAreaName(areaId);

            ltlLastActivityDate.Text = GetDateTime(lastActivityDate);
            ltlCountOfLogin.Text = countOfLogin.ToString();
            ltlRoles.Text = AdminManager.GetRolesHtml(userName);

            if (AuthRequest.AdminName != userName)
            {
                ltlActions.Text = $@"
<a class=""m-r-5"" href=""adminProfile.cshtml?pageType=admin&userId={userId}"">修改资料</a>
<a class=""m-r-5"" href=""adminPassword.cshtml?pageType=admin&userId={userId}"">更改密码</a>
<a class=""m-r-5"" href=""javascript:;"" onclick=""{ModalPermissionsSet.GetOpenWindowString(userName)}"">权限设置</a>
";

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

        private string GetUserNameHtml(int userId, string userName, int countOfFailedLogin, bool isLockedOut, DateTime lastActivityDate)
        {
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
            return $@"<a href=""adminView.cshtml?pageType=admin&userId={userId}"">{userName}</a> {state}";
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
