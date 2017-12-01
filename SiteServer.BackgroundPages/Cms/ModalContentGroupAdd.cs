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
		protected TextBox TbContentGroupName;
        public Literal LtlContentGroupName;
        protected TextBox TbDescription;

        public static string GetOpenWindowString(int publishmentSystemId, string groupName)
        {
            return PageUtils.GetOpenLayerString("修改内容组", PageUtils.GetCmsUrl(nameof(ModalContentGroupAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"GroupName", groupName}
            }), 600, 300);
        }

        public static string GetOpenWindowString(int publishmentSystemId)
        {
            return PageUtils.GetOpenLayerString("添加内容组", PageUtils.GetCmsUrl(nameof(ModalContentGroupAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            }), 600, 300);
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
                        TbContentGroupName.Text = contentGroupInfo.ContentGroupName;
                        TbContentGroupName.Visible = false;
                        LtlContentGroupName.Text = $"<strong>{contentGroupInfo.ContentGroupName}</strong>";
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
                ContentGroupName = PageUtils.FilterXss(TbContentGroupName.Text),
                PublishmentSystemId = PublishmentSystemId,
                Description = TbDescription.Text
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
				if (contentGroupNameList.IndexOf(TbContentGroupName.Text) != -1)
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
