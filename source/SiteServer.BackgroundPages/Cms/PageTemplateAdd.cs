using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTemplateAdd : BasePageCms
    {
        public Literal ltlPageTitle;

		public TextBox TemplateName;

        public DropDownList CreatedFileExtNameDropDownList;

        public TextBox RelatedFileName;
        public TextBox CreatedFileFullName;
        public Control CreatedFileFullNameRow;
		public Help CreatedFileFullNameHelp;
		public NoTagText CreatedFileFullNameText;

		public DropDownList Charset;
		public string TemplateTypeString;
		public HtmlInputHidden TemplateType;
		public TextBox Content;

        public Literal LtlCommands;
        public PlaceHolder phCodeMirror;
        public Button btnEditorType;

		private ETemplateType _theTemplateType = ETemplateType.IndexPageTemplate;
        private bool _isCopy;

        public static string GetRedirectUrl(int publishmentSystemId, int templateId, ETemplateType templateType)
        {
            return PageUtils.GetCmsUrl(nameof(PageTemplateAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"TemplateID", templateId.ToString()},
                {"TemplateType", ETemplateTypeUtils.GetValue(templateType)}
            });
        }

        public static string GetRedirectUrlToCopy(int publishmentSystemId, int templateId)
        {
            return PageUtils.GetCmsUrl(nameof(PageTemplateAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"TemplateID", templateId.ToString()},
                {"IsCopy", true.ToString()}
            });
        }

        public static string GetRedirectUrlToRestore(int publishmentSystemId, int templateId, int templateLogId)
        {
            return PageUtils.GetCmsUrl(nameof(PageTemplateAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"TemplateID", templateId.ToString()},
                {"TemplateLogID", templateLogId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            TemplateInfo templateInfo = null;
            if (Body.GetQueryInt("TemplateID") > 0)
            {
                var templateId = Body.GetQueryInt("TemplateID");
                _isCopy = Body.GetQueryBool("IsCopy");
                templateInfo = TemplateManager.GetTemplateInfo(PublishmentSystemId, templateId);
                if (templateInfo != null)
                {
                    _theTemplateType = templateInfo.TemplateType;
                }
            }
            else
            {
                _theTemplateType = ETemplateTypeUtils.GetEnumType(Request.QueryString["TemplateType"]);
            }
            TemplateTypeString = ETemplateTypeUtils.GetText(_theTemplateType);

            if (_theTemplateType == ETemplateType.IndexPageTemplate || _theTemplateType == ETemplateType.FileTemplate)
            {
                CreatedFileFullNameRow.Visible = true;
            }
            else
            {
                CreatedFileFullNameRow.Visible = false;
            }

			if (!IsPostBack)
			{
                var pageTitle = Body.GetQueryInt("TemplateID") > 0 ? "编辑模板" : "添加模板";
                BreadCrumb(AppManager.Cms.LeftMenu.IdTemplate, pageTitle, AppManager.Cms.Permission.WebSite.Template);

                ltlPageTitle.Text = pageTitle;

                var isCodeMirror = PublishmentSystemInfo.Additional.ConfigTemplateIsCodeMirror;
                btnEditorType.Text = isCodeMirror ? "采用纯文本编辑模式" : "采用代码编辑模式";
                phCodeMirror.Visible = isCodeMirror;

                EFileSystemTypeUtils.AddWebPageListItems(CreatedFileExtNameDropDownList);

                ECharsetUtils.AddListItems(Charset);

				if (Body.GetQueryInt("TemplateID") > 0)
				{
					if (templateInfo != null)
					{
                        Content.Text = StlCacheManager.FileContent.GetTemplateContent(PublishmentSystemInfo, templateInfo);

                        if (_isCopy)
                        {
                            TemplateName.Text = templateInfo.TemplateName + "_复件";
                            RelatedFileName.Text = PathUtils.RemoveExtension(templateInfo.RelatedFileName) + "_复件";
                            CreatedFileFullName.Text = PathUtils.RemoveExtension(templateInfo.CreatedFileFullName) + "_复件";
                        }
                        else
                        {
                            TemplateName.Text = templateInfo.TemplateName;
                            RelatedFileName.Text = PathUtils.RemoveExtension(templateInfo.RelatedFileName);
                            CreatedFileFullName.Text = PathUtils.RemoveExtension(templateInfo.CreatedFileFullName);

                            LtlCommands.Text +=
                                $@"<a href=""javascript:;"" class=""btn btn-info"" onclick=""{ModalProgressBar.GetOpenWindowStringWithCreateByTemplate(PublishmentSystemId, templateInfo.TemplateId)}"">生成页面</a><a href=""javascript:;"" class=""btn btn-info"" onclick=""{ModalTemplateRestore.GetOpenLayerString(PublishmentSystemId, templateInfo.TemplateId, string.Empty)}"">还原历史版本</a>";

                            if (Body.GetQueryInt("TemplateLogID") > 0)
                            {
                                var templateLogId = Body.GetQueryInt("TemplateLogID");
                                if (templateLogId > 0)
                                {
                                    Content.Text = DataProvider.TemplateLogDao.GetTemplateContent(templateLogId);
                                    SuccessMessage("已导入历史版本的模板内容，点击确定保存模板");
                                }
                            }
                        }

                        ControlUtils.SelectListItemsIgnoreCase(Charset, ECharsetUtils.GetValue(templateInfo.Charset));

                        ControlUtils.SelectListItems(CreatedFileExtNameDropDownList, GetTemplateFileExtension(templateInfo));
						TemplateType.Value = ETemplateTypeUtils.GetValue(templateInfo.TemplateType);
					}
				}
				else
				{
					RelatedFileName.Text = "T_";
                    if (_theTemplateType == ETemplateType.ChannelTemplate)
					{
						CreatedFileFullName.Text = "index";
					}
					else
					{
						CreatedFileFullName.Text = "@/";
					}
                    ControlUtils.SelectListItemsIgnoreCase(Charset, PublishmentSystemInfo.Additional.Charset);
                    ControlUtils.SelectListItems(CreatedFileExtNameDropDownList, EFileSystemTypeUtils.GetValue(EFileSystemType.Html));
					TemplateType.Value = Body.GetQueryString("TemplateType");
				}
			}
		}

        public void EditorType_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                var isCodeMirror = PublishmentSystemInfo.Additional.ConfigTemplateIsCodeMirror;
                isCodeMirror = !isCodeMirror;
                PublishmentSystemInfo.Additional.ConfigTemplateIsCodeMirror = isCodeMirror;
                DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

                btnEditorType.Text = isCodeMirror ? "采用纯文本编辑模式" : "采用代码编辑模式";
                phCodeMirror.Visible = isCodeMirror;
            }
        }

		public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                if (_theTemplateType != ETemplateType.ChannelTemplate)
				{
					if (!CreatedFileFullName.Text.StartsWith("~") && !CreatedFileFullName.Text.StartsWith("@"))
					{
						CreatedFileFullName.Text = PageUtils.Combine("@", CreatedFileFullName.Text);
					}
				}
				else
				{
					CreatedFileFullName.Text = CreatedFileFullName.Text.TrimStart('~', '@');
					CreatedFileFullName.Text = CreatedFileFullName.Text.Replace("/", string.Empty);
				}

				if (Body.GetQueryInt("TemplateID") > 0 && _isCopy == false)
				{
                    var templateId = Body.GetQueryInt("TemplateID");
                    var templateInfo = TemplateManager.GetTemplateInfo(PublishmentSystemId, templateId);
					if (templateInfo.TemplateName != TemplateName.Text)
					{
                        var templateNameArrayList = DataProvider.TemplateDao.GetTemplateNameArrayList(PublishmentSystemId, templateInfo.TemplateType);
						if (templateNameArrayList.IndexOf(TemplateName.Text) != -1)
						{
                            FailMessage("模板修改失败，模板名称已存在！");
							return;
						}
					}
                    TemplateInfo previousTemplateInfo = null;
                    var isChanged = false;
                    if (PathUtils.RemoveExtension(templateInfo.RelatedFileName) != PathUtils.RemoveExtension(RelatedFileName.Text))//文件名改变
                    {
                        var fileNameArrayList = DataProvider.TemplateDao.GetLowerRelatedFileNameArrayList(PublishmentSystemId, templateInfo.TemplateType);
                        foreach (string fileName in fileNameArrayList)
                        {
                            var fileNameWithoutExtension = PathUtils.RemoveExtension(fileName);
                            if (fileNameWithoutExtension == RelatedFileName.Text.ToLower())
                            {
                                FailMessage("模板修改失败，模板文件已存在！");
                                return;
                            }
                        }

                        isChanged = true;
                    }

                    if (GetTemplateFileExtension(templateInfo) != CreatedFileExtNameDropDownList.SelectedValue)//文件后缀改变
                    {
                        isChanged = true;
                    }

                    if (isChanged)
                    {
                        previousTemplateInfo = new TemplateInfo(templateInfo.TemplateId, templateInfo.PublishmentSystemId, templateInfo.TemplateName, templateInfo.TemplateType, templateInfo.RelatedFileName, templateInfo.CreatedFileFullName, templateInfo.CreatedFileExtName, templateInfo.Charset, templateInfo.IsDefault);
                    }
                    
					templateInfo.TemplateName = TemplateName.Text;
                    templateInfo.RelatedFileName = RelatedFileName.Text + CreatedFileExtNameDropDownList.SelectedValue;
                    templateInfo.CreatedFileExtName = CreatedFileExtNameDropDownList.SelectedValue;
                    templateInfo.CreatedFileFullName = CreatedFileFullName.Text + CreatedFileExtNameDropDownList.SelectedValue;
					templateInfo.Charset = ECharsetUtils.GetEnumType(Charset.SelectedValue);
					try
					{
						DataProvider.TemplateDao.Update(PublishmentSystemInfo, templateInfo, Content.Text, Body.AdministratorName);
                        if (previousTemplateInfo != null)
                        {
                            FileUtils.DeleteFileIfExists(TemplateManager.GetTemplateFilePath(PublishmentSystemInfo, previousTemplateInfo));
                        }
                        CreatePages(templateInfo);

                        Body.AddSiteLog(PublishmentSystemId,
                            $"修改{ETemplateTypeUtils.GetText(templateInfo.TemplateType)}",
                            $"模板名称:{templateInfo.TemplateName}");

						SuccessMessage("模板修改成功！");
					}
					catch(Exception ex)
					{
                        FailMessage(ex, "模板修改失败," + ex.Message);
					}
				}
				else
				{
                    var templateNameArrayList = DataProvider.TemplateDao.GetTemplateNameArrayList(PublishmentSystemId, ETemplateTypeUtils.GetEnumType(TemplateType.Value));
					if (templateNameArrayList.IndexOf(TemplateName.Text) != -1)
					{
                        FailMessage("模板添加失败，模板名称已存在！");
						return;
					}
                    var fileNameArrayList = DataProvider.TemplateDao.GetLowerRelatedFileNameArrayList(PublishmentSystemId, ETemplateTypeUtils.GetEnumType(TemplateType.Value));
					if (fileNameArrayList.IndexOf(RelatedFileName.Text.ToLower()) != -1)
					{
                        FailMessage("模板添加失败，模板文件已存在！");
						return;
					}

					var templateInfo = new TemplateInfo();
                    templateInfo.PublishmentSystemId = PublishmentSystemId;
					templateInfo.TemplateName = TemplateName.Text;
					templateInfo.TemplateType = ETemplateTypeUtils.GetEnumType(TemplateType.Value);

                    templateInfo.RelatedFileName = RelatedFileName.Text + CreatedFileExtNameDropDownList.SelectedValue;
                    templateInfo.CreatedFileExtName = CreatedFileExtNameDropDownList.SelectedValue;
                    templateInfo.CreatedFileFullName = CreatedFileFullName.Text + CreatedFileExtNameDropDownList.SelectedValue;
					templateInfo.Charset = ECharsetUtils.GetEnumType(Charset.SelectedValue);
					templateInfo.IsDefault = false;
					try
					{
                        templateInfo.TemplateId = DataProvider.TemplateDao.Insert(templateInfo, Content.Text, Body.AdministratorName);
                        CreatePages(templateInfo);
                        Body.AddSiteLog(PublishmentSystemId,
                            $"添加{ETemplateTypeUtils.GetText(templateInfo.TemplateType)}",
                            $"模板名称:{templateInfo.TemplateName}");
						SuccessMessage("模板添加成功！");
                        AddWaitAndRedirectScript(PageTemplate.GetRedirectUrl(PublishmentSystemId));
					}
					catch(Exception ex)
					{
						FailMessage(ex, "模板添加失败," + ex.Message);
					}
				}
			}
		}

        private void CreatePages(TemplateInfo templateInfo)
        {
            if (templateInfo.TemplateType == ETemplateType.FileTemplate)
            {
                CreateManager.CreateFile(PublishmentSystemId, templateInfo.TemplateId);
            }
            else if (templateInfo.TemplateType == ETemplateType.IndexPageTemplate)
            {
                if (templateInfo.IsDefault)
                {
                    CreateManager.CreateIndex(PublishmentSystemId);
                }
            }
        }

        private static string GetTemplateFileExtension(TemplateInfo templateInfo)
        {
            string extension;
            if (templateInfo.TemplateType == ETemplateType.IndexPageTemplate || templateInfo.TemplateType == ETemplateType.FileTemplate)
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
