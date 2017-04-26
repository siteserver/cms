using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using System.Collections.Specialized;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTagStyleGovInteractApplyAdd : BasePageCms
    {
        protected RadioButtonList IsAnomynous;

        private int styleID;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加互动交流提交样式", PageUtils.GetCmsUrl(nameof(ModalTagStyleGovInteractApplyAdd), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()}
                }), 500, 220);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int styleId)
        {
            return PageUtils.GetOpenWindowString("添加互动交流提交样式", PageUtils.GetCmsUrl(nameof(ModalTagStyleGovInteractApplyAdd), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"StyleID", styleId.ToString()}
                }), 500, 220);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            styleID = Body.GetQueryInt("StyleID");

			if (!IsPostBack)
			{
                var styleInfo = DataProvider.TagStyleDao.GetTagStyleInfo(styleID);
                var applyInfo = new TagStyleGovInteractApplyInfo(styleInfo.SettingsXML);

                ControlUtils.SelectListItems(IsAnomynous, applyInfo.IsAnomynous.ToString());
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;

            try
            {
                var styleInfo = DataProvider.TagStyleDao.GetTagStyleInfo(styleID);
                var applyInfo = new TagStyleGovInteractApplyInfo(styleInfo.SettingsXML);

                applyInfo.IsAnomynous = TranslateUtils.ToBool(IsAnomynous.SelectedValue);

                styleInfo.SettingsXML = applyInfo.ToString();

                DataProvider.TagStyleDao.Update(styleInfo);

                Body.AddSiteLog(PublishmentSystemId, "修改互动交流提交样式", $"样式名称:{styleInfo.StyleName}");

                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "互动交流提交样式修改失败！");
            }

			if (isChanged)
			{
                PageUtils.CloseModalPage(Page);
			}
		}
	}
}
