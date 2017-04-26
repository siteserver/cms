using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalInnerLinkAdd : BasePageCms
    {
		protected TextBox InnerLinkName;
		protected TextBox LinkUrl;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加站内链接", PageUtils.GetCmsUrl(nameof(ModalInnerLinkAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            }), 500, 280);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, string innerLinkName)
        {
            return PageUtils.GetOpenWindowString("修改站内链接", PageUtils.GetCmsUrl(nameof(ModalInnerLinkAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"InnerLinkName", innerLinkName}
            }), 500, 280);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{  
                if (Body.IsQueryExists("InnerLinkName"))
				{
                    var innerLinkName = Body.GetQueryString("InnerLinkName");
                    var innerLinkInfo = DataProvider.InnerLinkDao.GetInnerLinkInfo(innerLinkName, PublishmentSystemId);
                    if (innerLinkInfo != null)
					{
                        InnerLinkName.Text = innerLinkInfo.InnerLinkName;
                        LinkUrl.Text = innerLinkInfo.LinkUrl;
					}
				}
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;

            var innerLinkInfo = new InnerLinkInfo();
            innerLinkInfo.InnerLinkName = InnerLinkName.Text;
            innerLinkInfo.PublishmentSystemID = PublishmentSystemId;
            innerLinkInfo.LinkUrl = LinkUrl.Text;

            if (Body.IsQueryExists("InnerLinkName"))
			{
				try
				{
                    if (Body.GetQueryString("InnerLinkName") == innerLinkInfo.InnerLinkName)
                    {
                        DataProvider.InnerLinkDao.Update(innerLinkInfo);
                        Body.AddSiteLog(PublishmentSystemId, "修改站内链接", $"站内链接:{innerLinkInfo.InnerLinkName}");
                        isChanged = true;
                    }
                    else
                    {
                        var innerLinkNameList = DataProvider.InnerLinkDao.GetInnerLinkNameList(PublishmentSystemId);
                        if (innerLinkNameList.IndexOf(InnerLinkName.Text) != -1)
                        {
                            FailMessage("站内链接修改失败，站内链接名称已存在！");
                        }
                        else
                        {
                            DataProvider.InnerLinkDao.Delete(Body.GetQueryString("InnerLinkName"), PublishmentSystemId);
                            DataProvider.InnerLinkDao.Insert(innerLinkInfo);
                            Body.AddSiteLog(PublishmentSystemId, "修改站内链接", $"站内链接:{innerLinkInfo.InnerLinkName}");
                            isChanged = true;
                        }
                    }
					
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "站内链接修改失败！");
				}
			}
			else
			{
                var innerLinkNameList = DataProvider.InnerLinkDao.GetInnerLinkNameList(PublishmentSystemId);
                if (innerLinkNameList.IndexOf(InnerLinkName.Text) != -1)
				{
                    FailMessage("站内链接添加失败，站内链接名称已存在！");
				}
				else
				{
					try
					{
                        DataProvider.InnerLinkDao.Insert(innerLinkInfo);
                        Body.AddSiteLog(PublishmentSystemId, "添加站内链接", $"站内链接:{innerLinkInfo.InnerLinkName}");
						isChanged = true;
					}
					catch(Exception ex)
					{
                        FailMessage(ex, "站内链接添加失败！");
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
