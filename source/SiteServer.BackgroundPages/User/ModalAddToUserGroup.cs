using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Text;

namespace SiteServer.BackgroundPages.User
{
    public class ModalAddToUserGroup : BasePage
    {
        protected DropDownList UserGroupIDDropDownList;

        private List<int> _userIdList;

        public static string GetOpenWindowString()
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("设置用户类别", PageUtils.GetUserUrl(nameof(ModalAddToUserGroup), null), "UserIDCollection", "请选择需要设置类别的用户！", 400, 220);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _userIdList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("UserIDCollection"));

            if (!IsPostBack)
            {
                var userGroupInfoList = UserGroupManager.GetGroupInfoList();
                foreach (var theUserGroupInfo in userGroupInfoList)
                {
                    var listItem = new ListItem(theUserGroupInfo.GroupName, theUserGroupInfo.GroupId.ToString());
                    UserGroupIDDropDownList.Items.Add(listItem);
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var groupID = TranslateUtils.ToInt(UserGroupIDDropDownList.SelectedValue);

            var groupInfo = UserGroupManager.GetGroupInfo(groupID);

            foreach (var userID in _userIdList)
            {
                var userInfo = BaiRongDataProvider.UserDao.GetUserInfo(userID);

                userInfo.GroupId = groupID;

                BaiRongDataProvider.UserDao.Update(userInfo);
            }

            Body.AddAdminLog("设置用户类别", string.Empty);

            PageUtils.CloseModalPage(Page);
        }

    }
}
