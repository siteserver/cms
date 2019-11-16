using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

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

            Template template = null;
            if (AuthRequest.GetQueryInt("TemplateID") > 0)
            {
                var templateId = AuthRequest.GetQueryInt("TemplateID");
                _isCopy = AuthRequest.GetQueryBool("IsCopy");
                template = TemplateManager.GetTemplateAsync(SiteId, templateId).GetAwaiter().GetResult();
                if (template != null)
                {
                    _templateType = template.Type;
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

            var isCodeMirror = Site.ConfigTemplateIsCodeMirror;
            BtnEditorType.Text = isCodeMirror ? "采用纯文本编辑模式" : "采用代码编辑模式";
            PhCodeMirror.Visible = isCodeMirror;

            EFileSystemTypeUtils.AddWebPageListItems(DdlCreatedFileExtName);

            ECharsetUtils.AddListItems(DdlCharset);

            if (AuthRequest.GetQueryInt("TemplateID") > 0)
            {
                if (template == null) return;

                TbContent.Text = TemplateManager.GetTemplateContent(Site, template);

                if (_isCopy)
                {
                    TbTemplateName.Text = template.TemplateName + "_复件";
                    TbRelatedFileName.Text = PathUtils.RemoveExtension(template.RelatedFileName) + "_复件";
                    TbCreatedFileFullName.Text = PathUtils.RemoveExtension(template.CreatedFileFullName) + "_复件";
                }
                else
                {
                    TbTemplateName.Text = template.TemplateName;
                    TbRelatedFileName.Text = PathUtils.RemoveExtension(template.RelatedFileName);
                    TbCreatedFileFullName.Text = PathUtils.RemoveExtension(template.CreatedFileFullName);

                    LtlCommands.Text += $@"
<button class=""btn"" onclick=""{ModalProgressBar.GetOpenWindowStringWithCreateByTemplate(SiteId, template.Id)}"">生成页面</button>
<button class=""btn"" onclick=""{ModalTemplateRestore.GetOpenWindowString(SiteId, template.Id, string.Empty)}"">还原历史版本</button>";

                    if (AuthRequest.GetQueryInt("TemplateLogID") > 0)
                    {
                        var templateLogId = AuthRequest.GetQueryInt("TemplateLogID");
                        if (templateLogId > 0)
                        {
                            TbContent.Text = DataProvider.TemplateLogDao.GetTemplateContentAsync(templateLogId).GetAwaiter().GetResult();
                            SuccessMessage("已导入历史版本的模板内容，点击确定保存模板");
                        }
                    }
                }

                ControlUtils.SelectSingleItemIgnoreCase(DdlCharset, ECharsetUtils.GetValue(template.CharsetType));

                ControlUtils.SelectSingleItem(DdlCreatedFileExtName, GetTemplateFileExtension(template));
                HihTemplateType.Value = template.Type.Value;
            }
            else
            {
                TbRelatedFileName.Text = "T_";
                TbCreatedFileFullName.Text = _templateType == TemplateType.ChannelTemplate ? "index" : "@/";
                ControlUtils.SelectSingleItemIgnoreCase(DdlCharset, Site.Charset);
                ControlUtils.SelectSingleItem(DdlCreatedFileExtName, EFileSystemTypeUtils.GetValue(EFileSystemType.Html));
                HihTemplateType.Value = AuthRequest.GetQueryString("TemplateType");
            }
        }

        public void EditorType_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var isCodeMirror = Site.ConfigTemplateIsCodeMirror;
            isCodeMirror = !isCodeMirror;
            Site.ConfigTemplateIsCodeMirror = isCodeMirror;
            DataProvider.SiteDao.UpdateAsync(Site).GetAwaiter().GetResult();

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
		        var templateInfo = TemplateManager.GetTemplateAsync(SiteId, templateId).GetAwaiter().GetResult();
		        if (templateInfo.TemplateName != TbTemplateName.Text)
		        {
		            var templateNameList = DataProvider.TemplateDao.GetTemplateNameListAsync(SiteId, templateInfo.Type).GetAwaiter().GetResult();
		            if (templateNameList.Contains(TbTemplateName.Text))
		            {
		                FailMessage("模板修改失败，模板名称已存在！");
		                return;
		            }
		        }
		        Template previousTemplate = null;
		        var isChanged = false;
		        if (PathUtils.RemoveExtension(templateInfo.RelatedFileName) != PathUtils.RemoveExtension(TbRelatedFileName.Text))//文件名改变
		        {
		            var fileNameList = DataProvider.TemplateDao.GetRelatedFileNameListAsync(SiteId, templateInfo.Type).GetAwaiter().GetResult();
		            foreach (var fileName in fileNameList)
		            {
		                var fileNameWithoutExtension = PathUtils.RemoveExtension(fileName);
		                if (StringUtils.EqualsIgnoreCase(fileNameWithoutExtension, TbRelatedFileName.Text))
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
		            previousTemplate = new Template
                    {
                        Id = templateInfo.Id,
                        SiteId = templateInfo.SiteId,
                        TemplateName = templateInfo.TemplateName,
                        TemplateType = templateInfo.TemplateType,
                        RelatedFileName = templateInfo.RelatedFileName,
                        CreatedFileFullName = templateInfo.CreatedFileFullName,
                        CreatedFileExtName = templateInfo.CreatedFileExtName,
                        Charset = templateInfo.Charset,
                        Default = templateInfo.Default
                    };
		        }
                    
		        templateInfo.TemplateName = TbTemplateName.Text;
		        templateInfo.RelatedFileName = TbRelatedFileName.Text + DdlCreatedFileExtName.SelectedValue;
		        templateInfo.CreatedFileExtName = DdlCreatedFileExtName.SelectedValue;
		        templateInfo.CreatedFileFullName = TbCreatedFileFullName.Text + DdlCreatedFileExtName.SelectedValue;
		        templateInfo.CharsetType = ECharsetUtils.GetEnumType(DdlCharset.SelectedValue);

		        DataProvider.TemplateDao.UpdateAsync(Site, templateInfo, TbContent.Text, AuthRequest.AdminName).GetAwaiter().GetResult();
		        if (previousTemplate != null)
		        {
		            FileUtils.DeleteFileIfExists(TemplateManager.GetTemplateFilePath(Site, previousTemplate));
		        }
		        CreatePages(templateInfo);

		        AuthRequest.AddSiteLogAsync(SiteId,
		            $"修改{TemplateTypeUtils.GetText(templateInfo.Type)}",
		            $"模板名称:{templateInfo.TemplateName}").GetAwaiter().GetResult();

		        SuccessMessage("模板修改成功！");
		    }
		    else
		    {
		        var templateNameList = DataProvider.TemplateDao.GetTemplateNameListAsync(SiteId, TemplateTypeUtils.GetEnumType(HihTemplateType.Value)).GetAwaiter().GetResult();
		        if (templateNameList.Contains(TbTemplateName.Text))
		        {
		            FailMessage("模板添加失败，模板名称已存在！");
		            return;
		        }
		        var fileNameList = DataProvider.TemplateDao.GetRelatedFileNameListAsync(SiteId, TemplateTypeUtils.GetEnumType(HihTemplateType.Value)).GetAwaiter().GetResult();
		        if (StringUtils.ContainsIgnoreCase(fileNameList, TbRelatedFileName.Text))
		        {
		            FailMessage("模板添加失败，模板文件已存在！");
		            return;
		        }

		        var templateInfo = new Template
		        {
		            SiteId = SiteId,
		            TemplateName = TbTemplateName.Text,
                    Type = TemplateTypeUtils.GetEnumType(HihTemplateType.Value),
		            RelatedFileName = TbRelatedFileName.Text + DdlCreatedFileExtName.SelectedValue,
		            CreatedFileExtName = DdlCreatedFileExtName.SelectedValue,
		            CreatedFileFullName = TbCreatedFileFullName.Text + DdlCreatedFileExtName.SelectedValue,
                    CharsetType = ECharsetUtils.GetEnumType(DdlCharset.SelectedValue),
		            Default = false
		        };

		        templateInfo.Id = DataProvider.TemplateDao.InsertAsync(templateInfo, TbContent.Text, AuthRequest.AdminName).GetAwaiter().GetResult();
		        CreatePages(templateInfo);
		        AuthRequest.AddSiteLogAsync(SiteId,
		            $"添加{TemplateTypeUtils.GetText(templateInfo.Type)}",
		            $"模板名称:{templateInfo.TemplateName}").GetAwaiter().GetResult();
		        SuccessMessage("模板添加成功！");
		        AddWaitAndRedirectScript(PageTemplate.GetRedirectUrl(SiteId, _templateType));
            }
		}

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageTemplate.GetRedirectUrl(SiteId, _templateType));
        }

        private void CreatePages(Template template)
        {
            if (template.Type == TemplateType.FileTemplate)
            {
                CreateManager.CreateFileAsync(SiteId, template.Id).GetAwaiter().GetResult();
            }
            else if (template.Type == TemplateType.IndexPageTemplate)
            {
                if (template.Default)
                {
                    CreateManager.CreateChannelAsync(SiteId, SiteId).GetAwaiter().GetResult();
                }
            }
        }

        private static string GetTemplateFileExtension(Template template)
        {
            string extension;
            if (template.Type == TemplateType.IndexPageTemplate || template.Type == TemplateType.FileTemplate)
            {
                extension = PathUtils.GetExtension(template.CreatedFileFullName);
            }
            else
            {
                extension = template.CreatedFileExtName;
            }
            return extension;
        }
	}
}
