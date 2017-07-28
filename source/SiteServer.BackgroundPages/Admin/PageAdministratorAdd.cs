using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Permissions;
using BaiRong.Core.Text;

namespace SiteServer.BackgroundPages.Admin
{
	public class PageAdministratorAdd : BasePage
    {
        public Literal ltlPageTitle;
        public DropDownList ddlDepartmentID;
        public TextBox tbUserName;
        public TextBox tbDisplayName;
        public PlaceHolder phPassword;
        public TextBox tbPassword;

        public DropDownList ddlAreaID;
        public TextBox tbEmail;
        public TextBox tbMobile;
        
        public Button btnReturn;

        private int departmentID;
        private int areaID;
        private string userName;
        private bool[] isLastNodeArrayOfDepartment;
        private bool[] isLastNodeArrayOfArea;

        public static string GetRedirectUrlToAdd(int departmentId)
        {
            return PageUtils.GetAdminUrl(nameof(PageAdministratorAdd), new NameValueCollection
            {
                {"departmentID", departmentId.ToString()}
            });
        }

        public static string GetRedirectUrlToEdit(int departmentId, string userName)
        {
            return PageUtils.GetAdminUrl(nameof(PageAdministratorAdd), new NameValueCollection
            {
                {"departmentID", departmentId.ToString()},
                {"userName", userName}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            departmentID = Body.GetQueryInt("departmentID");
            areaID = Body.GetQueryInt("areaID");
            userName = Body.GetQueryString("userName");

            if (!Page.IsPostBack)
            {
                var pageTitle = string.IsNullOrEmpty(userName) ? "添加管理员" : "编辑管理员";
                BreadCrumbAdmin(AppManager.Admin.LeftMenu.AdminManagement, pageTitle, AppManager.Admin.Permission.AdminManagement);

                ltlPageTitle.Text = pageTitle;

                ddlDepartmentID.Items.Add(new ListItem("<无所属部门>", "0"));
                var departmentIdList = DepartmentManager.GetDepartmentIdList();
                var count = departmentIdList.Count;
                isLastNodeArrayOfDepartment = new bool[count];
                foreach (var theDepartmentId in departmentIdList)
                {
                    var departmentInfo = DepartmentManager.GetDepartmentInfo(theDepartmentId);
                    var listitem = new ListItem(GetDepartment(departmentInfo.DepartmentId, departmentInfo.DepartmentName, departmentInfo.ParentsCount, departmentInfo.IsLastNode), theDepartmentId.ToString());
                    if (departmentID == theDepartmentId)
                    {
                        listitem.Selected = true;
                    }
                    ddlDepartmentID.Items.Add(listitem);
                }

                ddlAreaID.Items.Add(new ListItem("<无所在区域>", "0"));
                var areaIdList = AreaManager.GetAreaIdList();
                count = areaIdList.Count;
                isLastNodeArrayOfArea = new bool[count];
                foreach (var theAreaId in areaIdList)
                {
                    var areaInfo = AreaManager.GetAreaInfo(theAreaId);
                    var listitem = new ListItem(GetArea(areaInfo.AreaId, areaInfo.AreaName, areaInfo.ParentsCount, areaInfo.IsLastNode), theAreaId.ToString());
                    if (areaID == theAreaId)
                    {
                        listitem.Selected = true;
                    }
                    ddlAreaID.Items.Add(listitem);
                }

                if (!string.IsNullOrEmpty(userName))
                {
                    var adminInfo = BaiRongDataProvider.AdministratorDao.GetByUserName(userName);
                    if (adminInfo != null)
                    {
                        ControlUtils.SelectListItems(ddlDepartmentID, adminInfo.DepartmentId.ToString());
                        tbUserName.Text = adminInfo.UserName;
                        tbUserName.Enabled = false;
                        tbDisplayName.Text = adminInfo.DisplayName;
                        phPassword.Visible = false;
                        ControlUtils.SelectListItems(ddlAreaID, adminInfo.AreaId.ToString());
                        tbEmail.Text = adminInfo.Email;
                        tbMobile.Text = adminInfo.Mobile;
                    }
                }

                var urlReturn = PageAdministrator.GetRedirectUrl(departmentID);
                btnReturn.Attributes.Add("onclick", $"location.href='{urlReturn}';return false;");
            }
		}

        public string GetDepartment(int departmentID, string departmentName, int parentsCount, bool isLastNode)
        {
            var str = "";
            if (isLastNode == false)
            {
                isLastNodeArrayOfDepartment[parentsCount] = false;
            }
            else
            {
                isLastNodeArrayOfDepartment[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                if (isLastNodeArrayOfDepartment[i])
                {
                    str = string.Concat(str, "　");
                }
                else
                {
                    str = string.Concat(str, "│");
                }
            }
            if (isLastNode)
            {
                str = string.Concat(str, "└");
            }
            else
            {
                str = string.Concat(str, "├");
            }
            str = string.Concat(str, departmentName);
            return str;
        }

        public string GetArea(int areaID, string areaName, int parentsCount, bool isLastNode)
        {
            var str = "";
            if (isLastNode == false)
            {
                isLastNodeArrayOfArea[parentsCount] = false;
            }
            else
            {
                isLastNodeArrayOfArea[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                if (isLastNodeArrayOfArea[i])
                {
                    str = string.Concat(str, "　");
                }
                else
                {
                    str = string.Concat(str, "│");
                }
            }
            if (isLastNode)
            {
                str = string.Concat(str, "└");
            }
            else
            {
                str = string.Concat(str, "├");
            }
            str = string.Concat(str, areaName);
            return str;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (string.IsNullOrEmpty(userName))
                {
                    var adminInfo = new AdministratorInfo
                    {
                        UserName = tbUserName.Text.Trim(),
                        Password = tbPassword.Text,
                        CreatorUserName = Body.AdministratorName,
                        DisplayName = tbDisplayName.Text,
                        Email = tbEmail.Text,
                        Mobile = tbMobile.Text,
                        DepartmentId = TranslateUtils.ToInt(ddlDepartmentID.SelectedValue),
                        AreaId = TranslateUtils.ToInt(ddlAreaID.SelectedValue)
                    };

                    if (!string.IsNullOrEmpty(BaiRongDataProvider.AdministratorDao.GetUserNameByEmail(tbEmail.Text)))
                    {
                        FailMessage("管理员添加失败，邮箱地址已存在");
                        return;
                    }

                    if (!string.IsNullOrEmpty(BaiRongDataProvider.AdministratorDao.GetUserNameByMobile(tbMobile.Text)))
                    {
                        FailMessage("管理员添加失败，手机号码已存在");
                        return;
                    }

                    string errorMessage;
                    if (!AdminManager.CreateAdministrator(adminInfo, out errorMessage))
                    {
                        FailMessage($"管理员添加失败：{errorMessage}");
                        return;   
                    }

                    Body.AddAdminLog("添加管理员", $"管理员:{tbUserName.Text.Trim()}");
                    SuccessMessage("管理员添加成功！");
                    AddWaitAndRedirectScript(PageAdministrator.GetRedirectUrl(TranslateUtils.ToInt(ddlDepartmentID.SelectedValue)));
                }
                else
                {
                    var adminInfo = BaiRongDataProvider.AdministratorDao.GetByUserName(userName);

                    if (adminInfo.Email != tbEmail.Text && !string.IsNullOrEmpty(BaiRongDataProvider.AdministratorDao.GetUserNameByEmail(tbEmail.Text)))
                    {
                        FailMessage("管理员设置失败，邮箱地址已存在");
                        return;
                    }

                    if (adminInfo.Mobile != tbMobile.Text && !string.IsNullOrEmpty(BaiRongDataProvider.AdministratorDao.GetUserNameByMobile(adminInfo.Mobile)))
                    {
                        FailMessage("管理员设置失败，手机号码已存在");
                        return;
                    }

                    adminInfo.DisplayName = tbDisplayName.Text;
                    adminInfo.Email = tbEmail.Text;
                    adminInfo.Mobile = tbMobile.Text;
                    adminInfo.DepartmentId = TranslateUtils.ToInt(ddlDepartmentID.SelectedValue);
                    adminInfo.AreaId = TranslateUtils.ToInt(ddlAreaID.SelectedValue);

                    BaiRongDataProvider.AdministratorDao.Update(adminInfo);

                    Body.AddAdminLog("修改管理员属性", $"管理员:{tbUserName.Text.Trim()}");
                    SuccessMessage("管理员设置成功！");
                    AddWaitAndRedirectScript(PageAdministrator.GetRedirectUrl(TranslateUtils.ToInt(ddlDepartmentID.SelectedValue)));
                }
            }
        }
	}
}
