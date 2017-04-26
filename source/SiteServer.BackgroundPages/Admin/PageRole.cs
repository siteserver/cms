using System;
using System.Data;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Permissions;
using BaiRong.Core.Text;

namespace SiteServer.BackgroundPages.Admin
{
	public class PageRole : BasePage
    {
		public DataGrid dgContents;
		public LinkButton pageFirst;
		public LinkButton pageLast;
		public LinkButton pageNext;
		public LinkButton pagePrevious;
		public Label currentPage;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetAdminUrl(nameof(PageRole), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (Body.IsQueryExists("Delete"))
			{
				var roleName = Body.GetQueryString("RoleName");
				try
				{
                    BaiRongDataProvider.PermissionsInRolesDao.Delete(roleName);
                    BaiRongDataProvider.RoleDao.DeleteRole(roleName);

                    Body.AddAdminLog("删除管理员角色", $"角色名称:{roleName}");

					SuccessDeleteMessage();
				}
				catch(Exception ex)
				{
					FailDeleteMessage(ex);
				}
			}

			if(!IsPostBack)
            {
                BreadCrumbAdmin(AppManager.Admin.LeftMenu.AdminManagement, "角色管理", AppManager.Admin.Permission.AdminManagement);

                BindGrid();
			}
		}

        public void dgContents_Page(object sender, DataGridPageChangedEventArgs e)
		{
            dgContents.CurrentPageIndex = e.NewPageIndex;
			BindGrid();
		}

		public DataSet GetDataSetByRoles(string[] roles)
		{
			var dataset = new DataSet();

			var dataTable = new DataTable("AllRoles");

			var column = new  DataColumn();
			column.DataType = Type.GetType("System.String");
			column.ColumnName = "RoleName";
			dataTable.Columns.Add(column);

			column = new  DataColumn();
			column.DataType = Type.GetType("System.String");
			column.ColumnName = "Description";
			dataTable.Columns.Add(column);

			foreach (var roleName in roles)
			{
                var dataRow = dataTable.NewRow();

                dataRow["RoleName"] = roleName;
                dataRow["Description"] = BaiRongDataProvider.RoleDao.GetRoleDescription(roleName);

                dataTable.Rows.Add(dataRow);
			}

			dataset.Tables.Add(dataTable);
			return dataset;
		}

		public void BindGrid()
		{
			try
			{
                dgContents.PageSize = StringUtils.Constants.PageSize;
                var permissioins = PermissionsManager.GetPermissions(Body.AdministratorName);
                if (permissioins.IsConsoleAdministrator)
				{
                    dgContents.DataSource = GetDataSetByRoles(BaiRongDataProvider.RoleDao.GetAllRoles());
				}
				else
				{
                    dgContents.DataSource = GetDataSetByRoles(BaiRongDataProvider.RoleDao.GetAllRolesByCreatorUserName(Body.AdministratorName));
				}
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();

                if (dgContents.CurrentPageIndex > 0)
				{
					pageFirst.Enabled = true;
					pagePrevious.Enabled = true;
				}
				else
				{
					pageFirst.Enabled = false;
					pagePrevious.Enabled = false;
				}

                if (dgContents.CurrentPageIndex + 1 == dgContents.PageCount)
				{
					pageLast.Enabled = false;
					pageNext.Enabled = false;
				}
				else
				{
					pageLast.Enabled = true;
					pageNext.Enabled = true;
				}

                currentPage.Text = $"{dgContents.CurrentPageIndex + 1}/{dgContents.PageCount}";
			}
			catch(Exception ex)
			{
                PageUtils.RedirectToErrorPage(ex.Message);
			}
				
		}

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var roleName = (string)dgContents.DataKeys[e.Item.ItemIndex];
                e.Item.Visible = !EPredefinedRoleUtils.IsPredefinedRole(roleName);
            }
        }

		protected void NavigationButtonClick(object sender, EventArgs e)
		{
			var button = (LinkButton)sender;
			var direction = button.CommandName;

			switch (direction.ToUpper())
			{
				case "FIRST" :
                    dgContents.CurrentPageIndex = 0;
					break;
				case "PREVIOUS" :
                    dgContents.CurrentPageIndex =
                        Math.Max(dgContents.CurrentPageIndex - 1, 0);
					break;
				case "NEXT" :
                    dgContents.CurrentPageIndex =
                        Math.Min(dgContents.CurrentPageIndex + 1,
                        dgContents.PageCount - 1);
					break;
				case "LAST" :
                    dgContents.CurrentPageIndex = dgContents.PageCount - 1;
					break;
				default :
					break;
			}
			BindGrid();
		}


	}
}
