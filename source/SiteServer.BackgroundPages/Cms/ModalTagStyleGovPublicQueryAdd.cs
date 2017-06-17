using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalTagStyleGovPublicQueryAdd : BasePageCms
    {
        protected TextBox StyleName;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加依申请公开查询样式",
                PageUtils.GetCmsUrl(nameof(ModalTagStyleGovPublicQueryAdd), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()}
                }), 500, 220);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int styleId)
        {
            return PageUtils.GetOpenWindowString("修改依申请公开查询样式",
                PageUtils.GetCmsUrl(nameof(ModalTagStyleGovPublicQueryAdd), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"StyleID", styleId.ToString()}
                }), 500, 220);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
                if (Body.IsQueryExists("StyleID"))
                {
                    var styleId = Body.GetQueryInt("StyleID");
                    var styleInfo = DataProvider.TagStyleDao.GetTagStyleInfo(styleId);
                    if (styleInfo != null)
                    {
                        StyleName.Text = styleInfo.StyleName;
                    }
                }
				
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;
            TagStyleInfo styleInfo;
				
			if (Body.IsQueryExists("StyleID"))
			{
				try
				{
                    var styleId = Body.GetQueryInt("StyleID");
                    styleInfo = DataProvider.TagStyleDao.GetTagStyleInfo(styleId);
                    if (styleInfo != null)
                    {
                        styleInfo.StyleName = StyleName.Text;
                    }
                    DataProvider.TagStyleDao.Update(styleInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改依申请公开查询样式", $"样式名称:{styleInfo.StyleName}");

					isChanged = true;
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "依申请公开查询样式修改失败！");
				}
			}
			else
			{
                var styleNameArrayList = DataProvider.TagStyleDao.GetStyleNameArrayList(PublishmentSystemId, StlGovPublicQuery.ElementName);
                if (styleNameArrayList.IndexOf(StyleName.Text) != -1)
				{
                    FailMessage("依申请公开查询样式添加失败，依申请公开查询样式名称已存在！");
				}
				else
				{
					try
					{
                        styleInfo = new TagStyleInfo();

                        styleInfo.StyleName = StyleName.Text;
                        styleInfo.ElementName = StlGovPublicQuery.ElementName;
                        styleInfo.PublishmentSystemID = PublishmentSystemId;

                        DataProvider.TagStyleDao.Insert(styleInfo);

                        Body.AddSiteLog(PublishmentSystemId, "添加依申请公开查询样式", $"样式名称:{styleInfo.StyleName}");

						isChanged = true;
					}
					catch(Exception ex)
					{
                        FailMessage(ex, "依申请公开查询样式添加失败！");
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
