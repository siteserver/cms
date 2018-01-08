using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

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

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageConfigurationContent), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (IsPostBack) return;

            VerifySitePermissions(AppManager.Permissions.WebSite.Configration);

            EBooleanUtils.AddListItems(DdlIsSaveImageInTextEditor, "保存", "不保存");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsSaveImageInTextEditor, PublishmentSystemInfo.Additional.IsSaveImageInTextEditor.ToString());

            EBooleanUtils.AddListItems(DdlIsAutoPageInTextEditor, "自动分页", "手动分页");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsAutoPageInTextEditor, PublishmentSystemInfo.Additional.IsAutoPageInTextEditor.ToString());

            PhAutoPage.Visible = PublishmentSystemInfo.Additional.IsAutoPageInTextEditor;
            TbAutoPageWordNum.Text = PublishmentSystemInfo.Additional.AutoPageWordNum.ToString();

            EBooleanUtils.AddListItems(DdlIsContentTitleBreakLine, "启用标题换行", "不启用");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsContentTitleBreakLine, PublishmentSystemInfo.Additional.IsContentTitleBreakLine.ToString());

            //保存时，敏感词自动检测
            EBooleanUtils.AddListItems(DdlIsAutoCheckKeywords, "启用敏感词自动检测", "不启用");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsAutoCheckKeywords, PublishmentSystemInfo.Additional.IsAutoCheckKeywords.ToString());

            DdlIsCheckContentUseLevel.Items.Add(new ListItem("默认审核机制", false.ToString()));
            DdlIsCheckContentUseLevel.Items.Add(new ListItem("多级审核机制", true.ToString()));

            ControlUtils.SelectSingleItem(DdlIsCheckContentUseLevel, PublishmentSystemInfo.IsCheckContentUseLevel.ToString());
            if (PublishmentSystemInfo.IsCheckContentUseLevel)
            {
                ControlUtils.SelectSingleItem(DdlCheckContentLevel, PublishmentSystemInfo.CheckContentLevel.ToString());
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

		    PublishmentSystemInfo.Additional.IsSaveImageInTextEditor = TranslateUtils.ToBool(DdlIsSaveImageInTextEditor.SelectedValue, true);

		    var isReCaculate = false;
		    if (TranslateUtils.ToBool(DdlIsAutoPageInTextEditor.SelectedValue, false))
		    {
		        if (PublishmentSystemInfo.Additional.IsAutoPageInTextEditor == false)
		        {
		            isReCaculate = true;
		        }
		        else if (PublishmentSystemInfo.Additional.AutoPageWordNum != TranslateUtils.ToInt(TbAutoPageWordNum.Text, PublishmentSystemInfo.Additional.AutoPageWordNum))
		        {
		            isReCaculate = true;
		        }
		    }

		    PublishmentSystemInfo.Additional.IsAutoPageInTextEditor = TranslateUtils.ToBool(DdlIsAutoPageInTextEditor.SelectedValue, false);

		    PublishmentSystemInfo.Additional.AutoPageWordNum = TranslateUtils.ToInt(TbAutoPageWordNum.Text, PublishmentSystemInfo.Additional.AutoPageWordNum);

		    PublishmentSystemInfo.Additional.IsContentTitleBreakLine = TranslateUtils.ToBool(DdlIsContentTitleBreakLine.SelectedValue, true);

		    PublishmentSystemInfo.Additional.IsAutoCheckKeywords = TranslateUtils.ToBool(DdlIsAutoCheckKeywords.SelectedValue, true);

		    PublishmentSystemInfo.IsCheckContentUseLevel = TranslateUtils.ToBool(DdlIsCheckContentUseLevel.SelectedValue);
		    if (PublishmentSystemInfo.IsCheckContentUseLevel)
		    {
		        PublishmentSystemInfo.CheckContentLevel = TranslateUtils.ToInt(DdlCheckContentLevel.SelectedValue);
		    }
				
		    try
		    {
		        DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
		        if (isReCaculate)
		        {
		            DataProvider.ContentDao.UpdateAutoPageContent(PublishmentSystemInfo.AuxiliaryTableForContent, PublishmentSystemInfo);
		        }

		        Body.AddSiteLog(PublishmentSystemId, "修改内容管理设置");

		        SuccessMessage("内容管理设置修改成功！");
		    }
		    catch (Exception ex)
		    {
		        FailMessage(ex, "内容管理设置修改失败！");
		    }
		}
	}
}
