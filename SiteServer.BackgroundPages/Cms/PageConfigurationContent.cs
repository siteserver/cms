using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationContent : BasePageCms
    {
        public DropDownList DdlIsSaveImageInTextEditor;
        public DropDownList DdlIsAutoPageInTextEditor;
        public PlaceHolder PhAutoPage;
        public TextBox TbAutoPageWordNum;
        public DropDownList DdlIsContentTitleBreakLine;
        public DropDownList DdlIsCheckContentUseLevel;
        public PlaceHolder PhCheckContentLevel; 
        public DropDownList DdlCheckContentLevel;
        public DropDownList DdlIsAutoCheckKeywords;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageConfigurationContent), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Configration);

            EBooleanUtils.AddListItems(DdlIsSaveImageInTextEditor, "保存", "不保存");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsSaveImageInTextEditor, SiteInfo.Additional.IsSaveImageInTextEditor.ToString());

            EBooleanUtils.AddListItems(DdlIsAutoPageInTextEditor, "自动分页", "手动分页");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsAutoPageInTextEditor, SiteInfo.Additional.IsAutoPageInTextEditor.ToString());

            PhAutoPage.Visible = SiteInfo.Additional.IsAutoPageInTextEditor;
            TbAutoPageWordNum.Text = SiteInfo.Additional.AutoPageWordNum.ToString();

            EBooleanUtils.AddListItems(DdlIsContentTitleBreakLine, "启用标题换行", "不启用");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsContentTitleBreakLine, SiteInfo.Additional.IsContentTitleBreakLine.ToString());

            //保存时，敏感词自动检测
            EBooleanUtils.AddListItems(DdlIsAutoCheckKeywords, "启用敏感词自动检测", "不启用");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsAutoCheckKeywords, SiteInfo.Additional.IsAutoCheckKeywords.ToString());

            DdlIsCheckContentUseLevel.Items.Add(new ListItem("默认审核机制", false.ToString()));
            DdlIsCheckContentUseLevel.Items.Add(new ListItem("多级审核机制", true.ToString()));

            ControlUtils.SelectSingleItem(DdlIsCheckContentUseLevel, SiteInfo.Additional.IsCheckContentLevel.ToString());
            if (SiteInfo.Additional.IsCheckContentLevel)
            {
                ControlUtils.SelectSingleItem(DdlCheckContentLevel, SiteInfo.Additional.CheckContentLevel.ToString());
                PhCheckContentLevel.Visible = true;
            }
            else
            {
                PhCheckContentLevel.Visible = false;
            }
        }

        public void DdlIsAutoPageInTextEditor_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PhAutoPage.Visible = EBooleanUtils.Equals(DdlIsAutoPageInTextEditor.SelectedValue, EBoolean.True);
        }

        public void DdlIsCheckContentUseLevel_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PhCheckContentLevel.Visible = EBooleanUtils.Equals(DdlIsCheckContentUseLevel.SelectedValue, EBoolean.True);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid) return;

		    SiteInfo.Additional.IsSaveImageInTextEditor = TranslateUtils.ToBool(DdlIsSaveImageInTextEditor.SelectedValue, true);

		    var isReCaculate = false;
		    if (TranslateUtils.ToBool(DdlIsAutoPageInTextEditor.SelectedValue, false))
		    {
		        if (SiteInfo.Additional.IsAutoPageInTextEditor == false)
		        {
		            isReCaculate = true;
		        }
		        else if (SiteInfo.Additional.AutoPageWordNum != TranslateUtils.ToInt(TbAutoPageWordNum.Text, SiteInfo.Additional.AutoPageWordNum))
		        {
		            isReCaculate = true;
		        }
		    }

		    SiteInfo.Additional.IsAutoPageInTextEditor = TranslateUtils.ToBool(DdlIsAutoPageInTextEditor.SelectedValue, false);

		    SiteInfo.Additional.AutoPageWordNum = TranslateUtils.ToInt(TbAutoPageWordNum.Text, SiteInfo.Additional.AutoPageWordNum);

		    SiteInfo.Additional.IsContentTitleBreakLine = TranslateUtils.ToBool(DdlIsContentTitleBreakLine.SelectedValue, true);

		    SiteInfo.Additional.IsAutoCheckKeywords = TranslateUtils.ToBool(DdlIsAutoCheckKeywords.SelectedValue, true);

		    SiteInfo.Additional.IsCheckContentLevel = TranslateUtils.ToBool(DdlIsCheckContentUseLevel.SelectedValue);
		    if (SiteInfo.Additional.IsCheckContentLevel)
		    {
		        SiteInfo.Additional.CheckContentLevel = TranslateUtils.ToInt(DdlCheckContentLevel.SelectedValue);
		    }

            DataProvider.SiteDao.Update(SiteInfo);
            if (isReCaculate)
            {
                DataProvider.ContentDao.SetAutoPageContentToSite(SiteInfo);
            }

            AuthRequest.AddSiteLog(SiteId, "修改内容设置");

            SuccessMessage("内容设置修改成功！");
        }
	}
}
