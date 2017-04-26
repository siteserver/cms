using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.User
{
    public class PageUserConfigWriting : BasePage
    {
        public DropDownList DdlIsWritingEnabled;
        public PlaceHolder PhWriting;
        public Repeater RptGroup;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            BreadCrumbUser(AppManager.User.LeftMenu.UserConfiguration, "投稿基本设置", AppManager.User.Permission.UserConfiguration);

            EBooleanUtils.AddListItems(DdlIsWritingEnabled, "开启", "关闭");
            ControlUtils.SelectListItemsIgnoreCase(DdlIsWritingEnabled, ConfigManager.UserConfigInfo.IsWritingEnabled.ToString());
            PhWriting.Visible = ConfigManager.UserConfigInfo.IsWritingEnabled;

            var pairList = new List<KeyValuePair<int, string>>();
            var defaultGroupInfo = UserGroupManager.GetDefaultGroupInfo();
            pairList.Add(new KeyValuePair<int, string>(defaultGroupInfo.GroupId, defaultGroupInfo.GroupName));
            var groupInfoList = UserGroupManager.GetGroupInfoList();
            foreach (var groupInfo in groupInfoList)
            {
                if (groupInfo.GroupId != defaultGroupInfo.GroupId)
                {
                    pairList.Add(new KeyValuePair<int, string>(groupInfo.GroupId, groupInfo.GroupName));
                }
            }

            RptGroup.DataSource = pairList;
            RptGroup.ItemDataBound += RptGroup_ItemDataBound;
            RptGroup.DataBind();
        }

        private void RptGroup_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var pair = (KeyValuePair<int, string>)e.Item.DataItem;

                var hfGroupId = (HiddenField)e.Item.FindControl("hfGroupID");
                var ltlGroupName = (Literal)e.Item.FindControl("ltlGroupName");
                var ddlIsGroupWritingEnabled = (DropDownList)e.Item.FindControl("ddlIsGroupWritingEnabled");
                var phIsGroupWritingEnabled = (PlaceHolder)e.Item.FindControl("phIsGroupWritingEnabled");
                var tbGroupWritingAdminUserName = (TextBox)e.Item.FindControl("tbGroupWritingAdminUserName");

                hfGroupId.Value = pair.Key.ToString();
                ltlGroupName.Text = pair.Value;

                var groupInfo = UserGroupManager.GetGroupInfo(pair.Key);
                var isWritingEnabled = groupInfo.Additional.IsWritingEnabled;
                var writingAdminUserName = groupInfo.Additional.WritingAdminUserName;

                EBooleanUtils.AddListItems(ddlIsGroupWritingEnabled, "开启", "关闭");
                ControlUtils.SelectListItemsIgnoreCase(ddlIsGroupWritingEnabled, isWritingEnabled.ToString());
                phIsGroupWritingEnabled.Visible = isWritingEnabled;
                tbGroupWritingAdminUserName.Text = writingAdminUserName;
            }
        }

        public void DdlIsGroupWritingEnabled_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ddlIsGroupWritingEnabled = (DropDownList)sender;
            var phIsGroupWritingEnabled = (PlaceHolder)ddlIsGroupWritingEnabled.Parent.FindControl("phIsGroupWritingEnabled");
            phIsGroupWritingEnabled.Visible = TranslateUtils.ToBool(ddlIsGroupWritingEnabled.SelectedValue);
        }

        public void DdlIsWritingEnabled_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhWriting.Visible = TranslateUtils.ToBool(DdlIsWritingEnabled.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                var groupInfoList = new List<UserGroupInfo>();
                foreach (RepeaterItem item in RptGroup.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        var hfGroupId = (HiddenField)item.FindControl("hfGroupID");
                        var ddlIsGroupWritingEnabled = (DropDownList)item.FindControl("ddlIsGroupWritingEnabled");
                        var tbGroupWritingAdminUserName = (TextBox)item.FindControl("tbGroupWritingAdminUserName");

                        var isWritingEnabled = TranslateUtils.ToBool(ddlIsGroupWritingEnabled.SelectedValue);
                        var writingAdminUserName = tbGroupWritingAdminUserName.Text;

                        if (!string.IsNullOrEmpty(writingAdminUserName))
                        {
                            if (!BaiRongDataProvider.AdministratorDao.IsUserNameExists(writingAdminUserName))
                            {
                                FailMessage($"保存失败，管理员不存在：{writingAdminUserName}");
                                return;
                            }
                        }

                        var groupId = TranslateUtils.ToInt(hfGroupId.Value);

                        var groupInfo = UserGroupManager.GetGroupInfo(groupId);
                        groupInfo.Additional.IsWritingEnabled = isWritingEnabled;
                        groupInfo.Additional.WritingAdminUserName = writingAdminUserName;

                        groupInfoList.Add(groupInfo);
                    }
                }

                foreach (var groupInfo in groupInfoList)
                {
                    BaiRongDataProvider.UserGroupDao.Update(groupInfo);
                }

                ConfigManager.UserConfigInfo.IsWritingEnabled = TranslateUtils.ToBool(DdlIsWritingEnabled.SelectedValue);
                BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

                Body.AddAdminLog("投稿基本设置");
                SuccessMessage("投稿基本设置成功");
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }
    }
}
