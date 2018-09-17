using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTemplateAdd : BasePageCms
    {
        public Literal LtlPageTitle;
        public Literal LtlTemplateType;
        public HtmlInputHidden HihTemplateType;
        public TextBox TbTemplateName;
        public DropDownList DdlCreatedFileExtName;
        public TextBox TbRelatedFileName;
        public PlaceHolder PhCreatedFileFullName;
        public TextBox TbCreatedFileFullName;
		public DropDownList DdlCharset;
        public Literal LtlCommands;
        public TextBox TbContent;
        public PlaceHolder PhCodeMirror;
        public Button BtnEditorType;

		private TemplateType _templateType = TemplateType.IndexPageTemplate;
        private bool _isCopy;

        public static string GetRedirectUrl(int siteId, int templateId, TemplateType templateType)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageTemplateAdd), new NameValueCollection
            {
                {"TemplateID", templateId.ToString()},
                {"TemplateType", templateType.Value}
            });
        }

        public static string GetRedirectUrlToCopy(int siteId, int templateId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageTemplateAdd), new NameValueCollection
            {
                {"TemplateID", templateId.ToString()},
                {"IsCopy", true.ToString()}
            });
        }

        public static string GetRedirectUrlToRestore(int siteId, int templateId, int templateLogId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageTemplateAdd), new NameValueCollection
            {
                {"TemplateID", templateId.ToString()},
                {"TemplateLogID", templateLogId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            TemplateInfo templateInfo = null;
            if (AuthRequest.GetQueryInt("TemplateID") > 0)
            {
                var templateId = AuthRequest.GetQueryInt("TemplateID");
                _isCopy = AuthRequest.GetQueryBool("IsCopy");
                templateInfo = TemplateManager.GetTemplateInfo(SiteId, templateId);
                if (templateInfo != null)
                {
                    _templateType = templateInfo.TemplateType;
                }
            }
            else
            {
                _templateType = TemplateTypeUtils.GetEnumType(Request.QueryString["TemplateType"]);
            }

            if (_templateType == TemplateType.IndexPageTemplate || _templateType == TemplateType.FileTemplate)
            {
                PhCreatedFileFullName.Visible = true;
            }
            else
            {
                PhCreatedFileFullName.Visible = false;
            }

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Template);

            LtlTemplateType.Text = TemplateTypeUtils.GetText(_templateType);

            LtlPageTitle.Text = AuthRequest.GetQueryInt("TemplateID") > 0 ? "编辑模板" : "添加模板";

            var isCodeMirror = SiteInfo.Additional.ConfigTemplateIsCodeMirror;
            BtnEditorType.Text = isCodeMirror ? "采用纯文本编辑模式" : "采用代码编辑模式";
            PhCodeMirror.Visible = isCodeMirror;

            EFileSystemTypeUtils.AddWebPageListItems(DdlCreatedFileExtName);

            ECharsetUtils.AddListItems(DdlCharset);

            if (AuthRequest.GetQueryInt("TemplateID") > 0)
            {
                if (templateInfo == null) return;

                TbContent.Text = TemplateManager.GetTemplateContent(SiteInfo, templateInfo);

                if (_isCopy)
                {
                    TbTemplateName.Text = templateInfo.TemplateName + "_复件";
                    TbRelatedFileName.Text = PathUtils.RemoveExtension(templateInfo.RelatedFileName) + "_复件";
                    TbCreatedFileFullName.Text = PathUtils.RemoveExtension(templateInfo.CreatedFileFullName) + "_复件";
                }
                else
                {
                    TbTemplateName.Text = templateInfo.TemplateName;
                    TbRelatedFileName.Text = PathUtils.RemoveExtension(templateInfo.RelatedFileName);
                    TbCreatedFileFullName.Text = PathUtils.RemoveExtension(templateInfo.CreatedFileFullName);

                    LtlCommands.Text += $@"
<button class=""btn"" onclick=""{ModalProgressBar.GetOpenWindowStringWithCreateByTemplate(SiteId, templateInfo.Id)}"">生成页面</button>
<button class=""btn"" onclick=""{ModalTemplateRestore.GetOpenWindowString(SiteId, templateInfo.Id, string.Empty)}"">还原历史版本</button>";

                    if (AuthRequest.GetQueryInt("TemplateLogID") > 0)
                    {
                        var templateLogId = AuthRequest.GetQueryInt("TemplateLogID");
                        if (templateLogId > 0)
                        {
                            TbContent.Text = DataProvider.TemplateLogDao.GetTemplateContent(templateLogId);
                            SuccessMessage("已导入历史版本的模板内容，点击确定保存模板");
                        }
                    }
                }

                ControlUtils.SelectSingleItemIgnoreCase(DdlCharset, ECharsetUtils.GetValue(templateInfo.Charset));

                ControlUtils.SelectSingleItem(DdlCreatedFileExtName, GetTemplateFileExtension(templateInfo));
                HihTemplateType.Value = templateInfo.TemplateType.Value;
            }
            else
            {
                TbRelatedFileName.Text = "T_";
                TbCreatedFileFullName.Text = _templateType == TemplateType.ChannelTemplate ? "index" : "@/";
                ControlUtils.SelectSingleItemIgnoreCase(DdlCharset, SiteInfo.Additional.Charset);
                ControlUtils.SelectSingleItem(DdlCreatedFileExtName, EFileSystemTypeUtils.GetValue(EFileSystemType.Html));
                HihTemplateType.Value = AuthRequest.GetQueryString("TemplateType");
            }
        }

        public void EditorType_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var isCodeMirror = SiteInfo.Additional.ConfigTemplateIsCodeMirror;
            isCodeMirror = !isCodeMirror;
            SiteInfo.Additional.ConfigTemplateIsCodeMirror = isCodeMirror;
            DataProvider.SiteDao.Update(SiteInfo);

            BtnEditorType.Text = isCodeMirror ? "采用纯文本编辑模式" : "采用代码编辑模式";
            PhCodeMirror.Visible = isCodeMirror;
        }

		public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid) return;

		    if (_templateType != TemplateType.ChannelTemplate)
		    {
		        if (!TbCreatedFileFullName.Text.StartsWith("~") && !TbCreatedFileFullName.Text.StartsWith("@"))
		        {
                    TbCreatedFileFullName.Text = PageUtils.Combine("@", TbCreatedFileFullName.Text);
		        }
		    }
		    else
		    {
                TbCreatedFileFullName.Text = TbCreatedFileFullName.Text.TrimStart('~', '@');
                TbCreatedFileFullName.Text = TbCreatedFileFullName.Text.Replace("/", string.Empty);
		    }

		    if (AuthRequest.GetQueryInt("TemplateID") > 0 && _isCopy == false)
		    {
		        var templateId = AuthRequest.GetQueryInt("TemplateID");
		        var templateInfo = TemplateManager.GetTemplateInfo(SiteId, templateId);
		        if (templateInfo.TemplateName != TbTemplateName.Text)
		        {
		            var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(SiteId, templateInfo.TemplateType);
		            if (templateNameList.IndexOf(TbTemplateName.Text) != -1)
		            {
		                FailMessage("模板修改失败，模板名称已存在！");
		                return;
		            }
		        }
		        TemplateInfo previousTemplateInfo = null;
		        var isChanged = false;
		        if (PathUtils.RemoveExtension(templateInfo.RelatedFileName) != PathUtils.RemoveExtension(TbRelatedFileName.Text))//文件名改变
		        {
		            var fileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(SiteId, templateInfo.TemplateType);
		            foreach (var fileName in fileNameList)
		            {
		                var fileNameWithoutExtension = PathUtils.RemoveExtension(fileName);
		                if (fileNameWithoutExtension == TbRelatedFileName.Text.ToLower())
		                {
		                    FailMessage("模板修改失败，模板文件已存在！");
		                    return;
		                }
		            }

		            isChanged = true;
		        }

		        if (GetTemplateFileExtension(templateInfo) != DdlCreatedFileExtName.SelectedValue)//文件后缀改变
		        {
		            isChanged = true;
		        }

		        if (isChanged)
		        {
		            previousTemplateInfo = new TemplateInfo(templateInfo.Id, templateInfo.SiteId, templateInfo.TemplateName, templateInfo.TemplateType, templateInfo.RelatedFileName, templateInfo.CreatedFileFullName, templateInfo.CreatedFileExtName, templateInfo.Charset, templateInfo.IsDefault);
		        }
                    
		        templateInfo.TemplateName = TbTemplateName.Text;
		        templateInfo.RelatedFileName = TbRelatedFileName.Text + DdlCreatedFileExtName.SelectedValue;
		        templateInfo.CreatedFileExtName = DdlCreatedFileExtName.SelectedValue;
		        templateInfo.CreatedFileFullName = TbCreatedFileFullName.Text + DdlCreatedFileExtName.SelectedValue;
		        templateInfo.Charset = ECharsetUtils.GetEnumType(DdlCharset.SelectedValue);

		        DataProvider.TemplateDao.Update(SiteInfo, templateInfo, TbContent.Text, AuthRequest.AdminName);
		        if (previousTemplateInfo != null)
		        {
		            FileUtils.DeleteFileIfExists(TemplateManager.GetTemplateFilePath(SiteInfo, previousTemplateInfo));
		        }
		        CreatePages(templateInfo);

		        AuthRequest.AddSiteLog(SiteId,
		            $"修改{TemplateTypeUtils.GetText(templateInfo.TemplateType)}",
		            $"模板名称:{templateInfo.TemplateName}");

		        SuccessMessage("模板修改成功！");
		    }
		    else
		    {
		        var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(SiteId, TemplateTypeUtils.GetEnumType(HihTemplateType.Value));
		        if (templateNameList.IndexOf(TbTemplateName.Text) != -1)
		        {
		            FailMessage("模板添加失败，模板名称已存在！");
		            return;
		        }
		        var fileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(SiteId, TemplateTypeUtils.GetEnumType(HihTemplateType.Value));
		        if (fileNameList.IndexOf(TbRelatedFileName.Text.ToLower()) != -1)
		        {
		            FailMessage("模板添加失败，模板文件已存在！");
		            return;
		        }

		        var templateInfo = new TemplateInfo
		        {
		            SiteId = SiteId,
		            TemplateName = TbTemplateName.Text,
		            TemplateType = TemplateTypeUtils.GetEnumType(HihTemplateType.Value),
		            RelatedFileName = TbRelatedFileName.Text + DdlCreatedFileExtName.SelectedValue,
		            CreatedFileExtName = DdlCreatedFileExtName.SelectedValue,
		            CreatedFileFullName = TbCreatedFileFullName.Text + DdlCreatedFileExtName.SelectedValue,
		            Charset = ECharsetUtils.GetEnumType(DdlCharset.SelectedValue),
		            IsDefault = false
		        };

		        templateInfo.Id = DataProvider.TemplateDao.Insert(templateInfo, TbContent.Text, AuthRequest.AdminName);
		        CreatePages(templateInfo);
		        AuthRequest.AddSiteLog(SiteId,
		            $"添加{TemplateTypeUtils.GetText(templateInfo.TemplateType)}",
		            $"模板名称:{templateInfo.TemplateName}");
		        SuccessMessage("模板添加成功！");
		        AddWaitAndRedirectScript(PageTemplate.GetRedirectUrl(SiteId, _templateType));
            }
		}

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageTemplate.GetRedirectUrl(SiteId, _templateType));
        }

        private void CreatePages(TemplateInfo templateInfo)
        {
            if (templateInfo.TemplateType == TemplateType.FileTemplate)
            {
                CreateManager.CreateFile(SiteId, templateInfo.Id);
            }
            else if (templateInfo.TemplateType == TemplateType.IndexPageTemplate)
            {
                if (templateInfo.IsDefault)
                {
                    CreateManager.CreateChannel(SiteId, SiteId);
                }
            }
        }

        private static string GetTemplateFileExtension(TemplateInfo templateInfo)
        {
            string extension;
            if (templateInfo.TemplateType == TemplateType.IndexPageTemplate || templateInfo.TemplateType == TemplateType.FileTemplate)
            {
                extension = PathUtils.GetExtension(templateInfo.CreatedFileFullName);
            }
            else
            {
                extension = templateInfo.CreatedFileExtName;
            }
            return extension;
        }
	}
}
