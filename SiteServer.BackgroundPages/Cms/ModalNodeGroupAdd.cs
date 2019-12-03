using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalNodeGroupAdd : BasePageCms
    {
		public TextBox TbNodeGroupName;
        public Literal LtlNodeGroupName;
        public TextBox TbDescription;

        public static string GetOpenWindowString(int siteId, string groupName)
        {
            return LayerUtils.GetOpenScript("修改栏目组", PageUtils.GetCmsUrl(siteId, nameof(ModalNodeGroupAdd), new NameValueCollection
            {
                {"GroupName", groupName}
            }), 600, 300);
        }

        public static string GetOpenWindowString(int siteId)
        {
            return LayerUtils.GetOpenScript("添加栏目组", PageUtils.GetCmsUrl(siteId, nameof(ModalNodeGroupAdd), null), 600, 300);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            if (AuthRequest.IsQueryExists("GroupName"))
            {
                var groupName = AuthRequest.GetQueryString("GroupName");
                var nodeGroupInfo = DataProvider.ChannelGroupRepository.GetAsync(SiteId, groupName).GetAwaiter().GetResult();
                if (nodeGroupInfo != null)
                {
                    TbNodeGroupName.Text = nodeGroupInfo.GroupName;
                    TbNodeGroupName.Visible = false;
                    LtlNodeGroupName.Text = $"<strong>{nodeGroupInfo.GroupName}</strong>";
                    TbDescription.Text = nodeGroupInfo.Description;
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;

            var nodeGroupInfo = new ChannelGroup
            {
                GroupName = TbNodeGroupName.Text,
                SiteId = SiteId,
                Description = TbDescription.Text
            };

            if (AuthRequest.IsQueryExists("GroupName"))
			{
				try
				{
                    DataProvider.ChannelGroupRepository.UpdateAsync(nodeGroupInfo).GetAwaiter().GetResult();
                    AuthRequest.AddSiteLogAsync(SiteId, "修改栏目组", $"栏目组:{nodeGroupInfo.GroupName}").GetAwaiter().GetResult();
					isChanged = true;
                }
				catch(Exception ex)
				{
                    FailMessage(ex, "栏目组修改失败！");
				}
			}
			else
			{
                var exists = DataProvider.ChannelGroupRepository.IsExistsAsync(SiteId, TbNodeGroupName.Text).GetAwaiter().GetResult();
				if (exists)
				{
                    FailMessage("栏目组添加失败，栏目组名称已存在！");
				}
				else
				{
					try
					{
						DataProvider.ChannelGroupRepository.InsertAsync(nodeGroupInfo).GetAwaiter().GetResult();
                        AuthRequest.AddSiteLogAsync(SiteId, "添加栏目组", $"栏目组:{nodeGroupInfo.GroupName}").GetAwaiter().GetResult();
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
                LayerUtils.Close(Page);
			}
		}
	}
}
