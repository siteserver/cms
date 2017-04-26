using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalNodeGroupAdd : BasePageCms
    {
		protected TextBox tbNodeGroupName;
		protected TextBox tbDescription;

        public static string GetOpenWindowString(int publishmentSystemId, string groupName)
        {
            return PageUtils.GetOpenWindowString("修改栏目组", PageUtils.GetCmsUrl(nameof(ModalNodeGroupAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"GroupName", groupName}
            }), 400, 280);
        }

        public static string GetOpenWindowString(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加栏目组", PageUtils.GetCmsUrl(nameof(ModalNodeGroupAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            }), 400, 280);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
				if (Body.IsQueryExists("GroupName"))
				{
                    var groupName = Body.GetQueryString("GroupName");
                    var nodeGroupInfo = DataProvider.NodeGroupDao.GetNodeGroupInfo(PublishmentSystemId, groupName);
					if (nodeGroupInfo != null)
					{
                        tbNodeGroupName.Text = nodeGroupInfo.NodeGroupName;
                        tbNodeGroupName.Enabled = false;
						tbDescription.Text = nodeGroupInfo.Description;
					}
				}
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;

            var nodeGroupInfo = new NodeGroupInfo
            {
                NodeGroupName = tbNodeGroupName.Text,
                PublishmentSystemID = PublishmentSystemId,
                Description = tbDescription.Text
            };

            if (Body.IsQueryExists("GroupName"))
			{
				try
				{
                    DataProvider.NodeGroupDao.Update(nodeGroupInfo);
                    Body.AddSiteLog(PublishmentSystemId, "修改栏目组", $"栏目组:{nodeGroupInfo.NodeGroupName}");
					isChanged = true;
                }
				catch(Exception ex)
				{
                    FailMessage(ex, "栏目组修改失败！");
				}
			}
			else
			{
                var nodeGroupNameList = DataProvider.NodeGroupDao.GetNodeGroupNameList(PublishmentSystemId);
				if (nodeGroupNameList.IndexOf(tbNodeGroupName.Text) != -1)
				{
                    FailMessage("栏目组添加失败，栏目组名称已存在！");
				}
				else
				{
					try
					{
						DataProvider.NodeGroupDao.Insert(nodeGroupInfo);
                        Body.AddSiteLog(PublishmentSystemId, "添加栏目组", $"栏目组:{nodeGroupInfo.NodeGroupName}");
						isChanged = true;
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "栏目组添加失败！");
					}
				}
			}

			if (isChanged)
			{
				PageUtils.CloseModalPage(Page);
			}
		}
	}
}
