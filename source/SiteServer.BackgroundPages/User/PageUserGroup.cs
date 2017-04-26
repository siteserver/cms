using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;

namespace SiteServer.BackgroundPages.User
{
    public class PageUserGroup : BasePage
    {
        public DataGrid MyDataGrid;
        public Button AddButton;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetUserUrl(nameof(PageUserGroup), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (Body.IsQueryExists("Delete"))
			{
                var groupId = Body.GetQueryInt("GroupID");
			
				try
				{
                    BaiRongDataProvider.UserGroupDao.Delete(groupId);

					SuccessDeleteMessage();
				}
				catch(Exception ex)
				{
					FailDeleteMessage(ex);
				}
            }

			if (!IsPostBack)
            {
                BreadCrumbUser(AppManager.User.LeftMenu.UserManagement, "用户组管理", AppManager.User.Permission.UserManagement);

                UserGroupManager.GetDefaultGroupInfo();

                BindGrid();
                AddButton.Attributes.Add("onclick", ModalUserGroupAdd.GetOpenWindowStringToAdd(0));
			}
		}

        public void BindGrid()
		{
            var userGroupInfoList = UserGroupManager.GetGroupInfoList();
  
            MyDataGrid.DataSource = userGroupInfoList;
            MyDataGrid.ItemDataBound += MyDataGrid_ItemDataBound;
            MyDataGrid.DataBind();
		}

        public void MyDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var groupInfo = e.Item.DataItem as UserGroupInfo;
                if (groupInfo == null)
                {
                    e.Item.Visible = false;
                    return;
                }

                var ltlGroupName = (Literal)e.Item.FindControl("ltlGroupName");
                var ltlDescription = (Literal)e.Item.FindControl("ltlDescription");
                var ltlIsDefault = (Literal)e.Item.FindControl("ltlIsDefault");
                var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");
                var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

                ltlGroupName.Text = groupInfo.GroupName;
                ltlDescription.Text = groupInfo.Description;
                ltlIsDefault.Text = StringUtils.GetTrueImageHtml(groupInfo.IsDefault);

                ltlEditUrl.Text =
                    $@"<a href='javascript:;' onclick=""{ModalUserGroupAdd.GetOpenWindowStringToEdit(groupInfo.GroupId)}"">编辑</a>";

                if (!groupInfo.IsDefault)
                {
                    var deleteUrl = PageUtils.GetUserUrl(nameof(PageUserGroup), new NameValueCollection
                    {
                        {"Delete", "True"},
                        {"GroupID", groupInfo.GroupId.ToString()}
                    });
                    ltlDeleteUrl.Text =
                        $"<a href=\"{deleteUrl}\" onClick=\"javascript:return confirm('此操作将删除用户组“{groupInfo.GroupName}”，确认吗？');\">删除</a>";
                }
            }
        }
	}
}
