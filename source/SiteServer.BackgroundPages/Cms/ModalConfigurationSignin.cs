using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    //设置签收项
    public class ModalConfigurationSignin : BasePageCms
    {
        public RadioButtonList TypeList;

        public PlaceHolder phGroup;
        public CheckBoxList GroupIDList;

        public PlaceHolder phUser;
        public TextBox UserNameList;

        public DropDownList Priority;
        public DateTimeTextBox EndDate;       

        private int _nodeId;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetOpenWindowString("签收设置", PageUtils.GetCmsUrl(nameof(ModalConfigurationSignin), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()}
            }), 480, 400);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _nodeId = Body.GetQueryInt("NodeID");

			if (!IsPostBack)
			{
                ESigninPriorityUtils.AddListItems(Priority);
                ControlUtils.SelectListItemsIgnoreCase(Priority, ESigninPriorityUtils.GetValue(ESigninPriority.Normal));
                var listItem = new ListItem("用户组", "Group");
                listItem.Selected = true;
                TypeList.Items.Add(listItem);
                listItem = new ListItem("用户", "User");
                TypeList.Items.Add(listItem);

                //foreach (UserGroupInfo groupInfo in UserGroupManager.GetGroupInfoArrayList())
                //{
                //    this.GroupIDList.Items.Add(new ListItem(groupInfo.GroupName, groupInfo.GroupID.ToString()));
                //}
                #region 编辑初始化

                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);

                if (nodeInfo.Additional.IsSignin)
                {
                    if (nodeInfo.Additional.IsSigninGroup)
                    {
                        phGroup.Visible = true;
                        phUser.Visible = false;
                        TypeList.SelectedValue = "Group";
                        var groupIDArrayList = TranslateUtils.StringCollectionToIntList(nodeInfo.Additional.SigninUserGroupCollection);
                        foreach (int groupID in groupIDArrayList)
                        {
                            GroupIDList.Items[groupID-1].Selected = true;
                        }
                    }
                    else
                    {
                        phGroup.Visible = false;
                        phUser.Visible = true;
                        TypeList.SelectedValue = "User";
                        UserNameList.Text = nodeInfo.Additional.SigninUserNameCollection;
                    }
                    Priority.SelectedValue = nodeInfo.Additional.SigninPriority.ToString();
                    EndDate.Text = nodeInfo.Additional.SigninEndDate.ToString();

                }
                #endregion
				
			}
		}

        public void TypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TypeList.SelectedValue == "Group")
            {
                phGroup.Visible = true;
                phUser.Visible = false;
            }
            else
            {
                phGroup.Visible = false;
                phUser.Visible = true;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);

                nodeInfo.Additional.SigninPriority = TranslateUtils.ToInt(Priority.SelectedValue);
                nodeInfo.Additional.SigninEndDate = EndDate.Text;
                if (TypeList.SelectedValue == "Group")
                {
                    var GroupID = "";
                    for (var i = 0; i < GroupIDList.Items.Count; i++)
                    {
                        if (GroupIDList.Items[i].Selected)
                        {
                            GroupID += GroupIDList.Items[i].Value + ","; //用户组列表
                        }
                    }
                    nodeInfo.Additional.SigninUserGroupCollection = GroupID.TrimEnd(',');
                    nodeInfo.Additional.SigninUserNameCollection = "";
                    nodeInfo.Additional.IsSigninGroup = true;
                    
                }
                else if (TypeList.SelectedValue == "User")
                {
                    nodeInfo.Additional.SigninUserGroupCollection = "";
                    nodeInfo.Additional.SigninUserNameCollection = UserNameList.Text.Trim();
                    nodeInfo.Additional.IsSigninGroup = false;
                }
                
                DataProvider.NodeDao.UpdateNodeInfo(nodeInfo);
                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
                isChanged = false;
            }

            if (isChanged)
            {
                PageUtils.CloseModalPageWithoutRefresh(Page, "alert('内容签收设置成功！');");
            }
        }

	}
}
