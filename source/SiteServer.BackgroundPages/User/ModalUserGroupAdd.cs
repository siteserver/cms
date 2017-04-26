using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Text;

namespace SiteServer.BackgroundPages.User
{
    public class ModalUserGroupAdd : BasePage
    {
        protected TextBox GroupName;
        protected TextBox Description;

        private int _groupId;
        private bool _isAdd;

        public static string GetOpenWindowStringToEdit(int groupId)
        {
            return PageUtils.GetOpenWindowString("修改用户组", PageUtils.GetUserUrl(nameof(ModalUserGroupAdd), new NameValueCollection
            {
                {"GroupID", groupId.ToString()},
                {"IsAdd", false.ToString()}
            }), 620, 550);
        }

        public static string GetOpenWindowStringToAdd(int groupId)
        {
            return PageUtils.GetOpenWindowString("添加用户组", PageUtils.GetUserUrl(nameof(ModalUserGroupAdd), new NameValueCollection
            {
                {"GroupID", groupId.ToString()},
                {"IsAdd", true.ToString()}
            }), 620, 550);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _groupId = Body.GetQueryInt("GroupID");
            _isAdd = Body.GetQueryBool("IsAdd");

            if (!IsPostBack)
            {
                if (!_isAdd)
                {
                    var groupInfo = UserGroupManager.GetGroupInfo(_groupId);
                    if (groupInfo != null)
                    {
                        GroupName.Text = groupInfo.GroupName;
                        Description.Text = groupInfo.Description;
                    }
                }
                else
                {
                    if (_groupId > 0)
                    {
                        var groupInfo = UserGroupManager.GetGroupInfo(_groupId);
                        if (groupInfo != null)
                        {
                            GroupName.Text = groupInfo.GroupName;
                            Description.Text = groupInfo.Description;
                        }
                    }
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (!_isAdd)
                {
                    try
                    {
                        var groupInfo = UserGroupManager.GetGroupInfo(_groupId);
                        if (groupInfo.GroupName != GroupName.Text && BaiRongDataProvider.UserGroupDao.IsExists(GroupName.Text))
                        {
                            FailMessage("用户组修改失败，用户组名称已存在！");
                            return;
                        }

                        groupInfo.GroupName = GroupName.Text;
                        groupInfo.Description = Description.Text;

                        Body.AddAdminLog("修改用户组",
                            $"用户组:{GroupName.Text}");

                        PageUtils.CloseModalPage(Page);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "用户组修改失败！");
                    }
                }
                else
                {
                    if (BaiRongDataProvider.UserGroupDao.IsExists(GroupName.Text))
                    {
                        FailMessage("用户组添加失败，用户组名称已存在！");
                        return;
                    }
                    else
                    {
                        var groupInfo = new UserGroupInfo(0, GroupName.Text, false, Description.Text, string.Empty);

                        try
                        {
                            BaiRongDataProvider.UserGroupDao.Insert(groupInfo);

                            Body.AddAdminLog("添加用户组",
                                $"用户组:{GroupName.Text}");

                            PageUtils.CloseModalPage(Page);
                        }
                        catch (Exception ex)
                        {
                            FailMessage(ex, "用户组添加失败！" + ex.ToString());
                        }
                    }
                }

            }
        }

	}
}
