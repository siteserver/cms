using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentGroupAdd : BasePageCms
    {
		protected TextBox ContentGroupName;
		protected TextBox Description;

        public static string GetOpenWindowString(int publishmentSystemId, string groupName)
        {
            return PageUtils.GetOpenWindowString("修改内容组", PageUtils.GetCmsUrl(nameof(ModalContentGroupAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"GroupName", groupName}
            }), 400, 280);
        }

        public static string GetOpenWindowString(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加内容组", PageUtils.GetCmsUrl(nameof(ModalContentGroupAdd), new NameValueCollection
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
                    var contentGroupInfo = DataProvider.ContentGroupDao.GetContentGroupInfo(groupName, PublishmentSystemId);
					if (contentGroupInfo != null)
					{
						ContentGroupName.Text = contentGroupInfo.ContentGroupName;
						ContentGroupName.Enabled = false;
						Description.Text = contentGroupInfo.Description;
					}
				}
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;

            var contentGroupInfo = new ContentGroupInfo
            {
                ContentGroupName = PageUtils.FilterXss(ContentGroupName.Text),
                PublishmentSystemId = PublishmentSystemId,
                Description = Description.Text
            };

            if (Body.IsQueryExists("GroupName"))
			{
				try
				{
                    DataProvider.ContentGroupDao.Update(contentGroupInfo);
                    Body.AddSiteLog(PublishmentSystemId, "修改内容组", $"内容组:{contentGroupInfo.ContentGroupName}");
					isChanged = true;
				}
                catch (Exception ex)
                {
                    FailMessage(ex, "内容组修改失败！");
				}
			}
			else
			{
                var contentGroupNameList = DataProvider.ContentGroupDao.GetContentGroupNameList(PublishmentSystemId);
				if (contentGroupNameList.IndexOf(ContentGroupName.Text) != -1)
				{
                    FailMessage("内容组添加失败，内容组名称已存在！");
				}
				else
				{
					try
					{
                        DataProvider.ContentGroupDao.Insert(contentGroupInfo);
                        Body.AddSiteLog(PublishmentSystemId, "添加内容组",
                            $"内容组:{contentGroupInfo.ContentGroupName}");
						isChanged = true;
					}
					catch(Exception ex)
					{
                        FailMessage(ex, "内容组添加失败！");
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
