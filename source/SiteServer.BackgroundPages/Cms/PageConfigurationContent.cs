using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationContent : BasePageCms
    {
        public DropDownList DdlIsGroupContent;
        public DropDownList DdlIsRelatedByTags;
        public DropDownList DdlIsTranslate;
        public DropDownList DdlIsSaveImageInTextEditor;
        public DropDownList DdlIsAutoPageInTextEditor;
        public TextBox TbAutoPageWordNum;
        public DropDownList DdlIsAutoSaveContent;
        public TextBox TbAutoSaveContentInterval;
        public RadioButtonList RblIsContentTitleBreakLine;
        public RadioButtonList RblIsCheckContentUseLevel;
        public PlaceHolder PhCheckContentLevel; 
        public DropDownList DdlCheckContentLevel;
        public RadioButtonList RblIsAutoCheckKeywords;
        public TextBox TbEditorUploadFilePre;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (IsPostBack) return;

            BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, "内容管理设置", AppManager.Cms.Permission.WebSite.Configration);

            EBooleanUtils.AddListItems(DdlIsGroupContent, "使用内容组", "不使用内容组");
            ControlUtils.SelectListItemsIgnoreCase(DdlIsGroupContent, PublishmentSystemInfo.Additional.IsGroupContent.ToString());

            EBooleanUtils.AddListItems(DdlIsRelatedByTags, "使用标签", "不使用标签");
            ControlUtils.SelectListItemsIgnoreCase(DdlIsRelatedByTags, PublishmentSystemInfo.Additional.IsRelatedByTags.ToString());

            EBooleanUtils.AddListItems(DdlIsTranslate, "使用内容转移", "不使用内容转移");
            ControlUtils.SelectListItemsIgnoreCase(DdlIsTranslate, PublishmentSystemInfo.Additional.IsTranslate.ToString());

            EBooleanUtils.AddListItems(DdlIsSaveImageInTextEditor, "保存", "不保存");
            ControlUtils.SelectListItemsIgnoreCase(DdlIsSaveImageInTextEditor, PublishmentSystemInfo.Additional.IsSaveImageInTextEditor.ToString());

            EBooleanUtils.AddListItems(DdlIsAutoPageInTextEditor, "自动分页", "手动分页");
            ControlUtils.SelectListItemsIgnoreCase(DdlIsAutoPageInTextEditor, PublishmentSystemInfo.Additional.IsAutoPageInTextEditor.ToString());

            TbAutoPageWordNum.Text = PublishmentSystemInfo.Additional.AutoPageWordNum.ToString();

            EBooleanUtils.AddListItems(DdlIsAutoSaveContent, "开启自动保存功能", "关闭自动保存功能");
            ControlUtils.SelectListItemsIgnoreCase(DdlIsAutoSaveContent, PublishmentSystemInfo.Additional.IsAutoSaveContent.ToString());

            TbAutoSaveContentInterval.Text = PublishmentSystemInfo.Additional.AutoSaveContentInterval.ToString();

            EBooleanUtils.AddListItems(RblIsContentTitleBreakLine, "启用标题换行", "不启用");
            ControlUtils.SelectListItemsIgnoreCase(RblIsContentTitleBreakLine, PublishmentSystemInfo.Additional.IsContentTitleBreakLine.ToString());

            //保存时，敏感词自动检测
            EBooleanUtils.AddListItems(RblIsAutoCheckKeywords, "启用敏感词自动检测", "不启用");
            ControlUtils.SelectListItemsIgnoreCase(RblIsAutoCheckKeywords, PublishmentSystemInfo.Additional.IsAutoCheckKeywords.ToString());

            //编辑器上传文件URL前缀
            TbEditorUploadFilePre.Text = PublishmentSystemInfo.Additional.EditorUploadFilePre;

            RblIsCheckContentUseLevel.Items.Add(new ListItem("默认审核机制", false.ToString()));
            RblIsCheckContentUseLevel.Items.Add(new ListItem("多级审核机制", true.ToString()));

            ControlUtils.SelectListItems(RblIsCheckContentUseLevel, PublishmentSystemInfo.IsCheckContentUseLevel.ToString());
            if (PublishmentSystemInfo.IsCheckContentUseLevel)
            {
                ControlUtils.SelectListItems(DdlCheckContentLevel, PublishmentSystemInfo.CheckContentLevel.ToString());
                PhCheckContentLevel.Visible = true;
            }
            else
            {
                PhCheckContentLevel.Visible = false;
            }
        }

        public void RblIsCheckContentUseLevel_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PhCheckContentLevel.Visible = EBooleanUtils.Equals(RblIsCheckContentUseLevel.SelectedValue, EBoolean.True);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                PublishmentSystemInfo.Additional.IsGroupContent = TranslateUtils.ToBool(DdlIsGroupContent.SelectedValue);
                PublishmentSystemInfo.Additional.IsRelatedByTags = TranslateUtils.ToBool(DdlIsRelatedByTags.SelectedValue);
                PublishmentSystemInfo.Additional.IsTranslate = TranslateUtils.ToBool(DdlIsTranslate.SelectedValue);

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

                PublishmentSystemInfo.Additional.IsAutoSaveContent = TranslateUtils.ToBool(DdlIsAutoSaveContent.SelectedValue, false);

                PublishmentSystemInfo.Additional.AutoSaveContentInterval = TranslateUtils.ToInt(TbAutoSaveContentInterval.Text, PublishmentSystemInfo.Additional.AutoSaveContentInterval);

                PublishmentSystemInfo.Additional.IsContentTitleBreakLine = TranslateUtils.ToBool(RblIsContentTitleBreakLine.SelectedValue, true);

                //敏感词自动检测
                PublishmentSystemInfo.Additional.IsAutoCheckKeywords = TranslateUtils.ToBool(RblIsAutoCheckKeywords.SelectedValue, true);

                //编辑器上传文件URL前缀
                PublishmentSystemInfo.Additional.EditorUploadFilePre = TbEditorUploadFilePre.Text;

                PublishmentSystemInfo.IsCheckContentUseLevel = TranslateUtils.ToBool(RblIsCheckContentUseLevel.SelectedValue);
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
}
