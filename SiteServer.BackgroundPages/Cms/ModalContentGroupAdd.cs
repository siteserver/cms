using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentGroupAdd : BasePageCms
    {
		protected TextBox TbContentGroupName;
        public Literal LtlContentGroupName;
        protected TextBox TbDescription;

        public static string GetOpenWindowString(int siteId, string groupName)
        {
            return LayerUtils.GetOpenScript("修改内容组", PageUtils.GetCmsUrl(siteId, nameof(ModalContentGroupAdd), new NameValueCollection
            {
                {"GroupName", groupName}
            }), 600, 300);
        }

        public static string GetOpenWindowString(int siteId)
        {
            return LayerUtils.GetOpenScript("添加内容组", PageUtils.GetCmsUrl(siteId, nameof(ModalContentGroupAdd), null), 600, 300);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
				if (AuthRequest.IsQueryExists("GroupName"))
				{
                    var groupName = AuthRequest.GetQueryString("GroupName");
                    var contentGroupInfo = ContentGroupManager.GetContentGroupInfo(SiteId, groupName);
					if (contentGroupInfo != null)
					{
                        TbContentGroupName.Text = contentGroupInfo.GroupName;
                        TbContentGroupName.Visible = false;
                        LtlContentGroupName.Text = $"<strong>{contentGroupInfo.GroupName}</strong>";
                        TbDescription.Text = contentGroupInfo.Description;
					}
				}
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;

            var contentGroupInfo = new ContentGroupInfo
            {
                GroupName = AttackUtils.FilterXss(TbContentGroupName.Text),
                SiteId = SiteId,
                Description = TbDescription.Text
            };

            if (AuthRequest.IsQueryExists("GroupName"))
			{
				try
				{
                    DataProvider.ContentGroupDao.Update(contentGroupInfo);
                    AuthRequest.AddSiteLog(SiteId, "修改内容组", $"内容组:{contentGroupInfo.GroupName}");
					isChanged = true;
				}
                catch (Exception ex)
                {
                    FailMessage(ex, "内容组修改失败！");
				}
			}
			else
			{
				if (ContentGroupManager.IsExists(SiteId, TbContentGroupName.Text))
				{
                    FailMessage("内容组添加失败，内容组名称已存在！");
				}
				else
				{
					try
					{
                        DataProvider.ContentGroupDao.Insert(contentGroupInfo);
                        AuthRequest.AddSiteLog(SiteId, "添加内容组",
                            $"内容组:{contentGroupInfo.GroupName}");
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
                LayerUtils.Close(Page);
			}
		}
	}
}
