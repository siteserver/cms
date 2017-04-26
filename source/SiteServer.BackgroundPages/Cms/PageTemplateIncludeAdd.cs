using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTemplateIncludeAdd : BasePageCms
    {
        public Literal ltlPageTitle;

        public Literal ltlCreatedFileExtName;

        public TextBox RelatedFileName;
		public DropDownList Charset;
		public TextBox Content;

        private string fileName;
        private string directoryPath;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (Body.IsQueryExists("FileName"))
            {
                fileName = Body.GetQueryString("FileName");
                fileName = PathUtils.RemoveParentPath(fileName);
            }
            directoryPath = PathUtility.MapPath(PublishmentSystemInfo, "@/include");

			if (!IsPostBack)
			{
                var pageTitle = string.IsNullOrEmpty(fileName) ? "添加包含文件" : "编辑包含文件";
                BreadCrumb(AppManager.Cms.LeftMenu.IdTemplate, pageTitle, AppManager.Cms.Permission.WebSite.Template);

                ltlPageTitle.Text = pageTitle;

                ECharsetUtils.AddListItems(Charset);

                if (fileName != null)
				{
                    if (!EFileSystemTypeUtils.IsHtml(PathUtils.GetExtension(fileName)))
                    {
                        PageUtils.RedirectToErrorPage("对不起，此文件无法编辑！");
                    }
                    else
                    {
                        RelatedFileName.Text = PathUtils.RemoveExtension(fileName);
                        ltlCreatedFileExtName.Text = PathUtils.GetExtension(fileName);
                        var fileCharset = FileUtils.GetFileCharset(PathUtils.Combine(directoryPath, fileName));
                        ControlUtils.SelectListItemsIgnoreCase(Charset, ECharsetUtils.GetValue(fileCharset));
                        Content.Text = FileUtils.ReadText(PathUtils.Combine(directoryPath, fileName), fileCharset);
                    }
				}
				else
                {
                    ltlCreatedFileExtName.Text = ".html";
                    ControlUtils.SelectListItemsIgnoreCase(Charset, PublishmentSystemInfo.Additional.Charset);
				}
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (fileName != null)
				{
                    var isChanged = false;
                    if (PathUtils.RemoveExtension(fileName) != PathUtils.RemoveExtension(RelatedFileName.Text))//文件名改变
                    {
                        var fileNames = DirectoryUtils.GetFileNames(directoryPath);
                        foreach (var theFileName in fileNames)
                        {
                            var fileNameWithoutExtension = PathUtils.RemoveExtension(theFileName);
                            if (fileNameWithoutExtension == RelatedFileName.Text.ToLower())
                            {
                                FailMessage("包含文件修改失败，包含文件已存在！");
                                return;
                            }
                        }

                        isChanged = true;
                    }

                    if (PathUtils.GetExtension(fileName) != ltlCreatedFileExtName.Text)//文件后缀改变
                    {
                        isChanged = true;
                    }

                    var previousFileName = string.Empty;
                    if (isChanged)
                    {
                        previousFileName = fileName;
                    }

                    var currentFileName = RelatedFileName.Text + ltlCreatedFileExtName.Text;
                    var charset = ECharsetUtils.GetEnumType(Charset.SelectedValue);
					try
					{
                        FileUtils.WriteText(PathUtils.Combine(directoryPath, currentFileName), charset, Content.Text);
                        if (!string.IsNullOrEmpty(previousFileName))
                        {
                            FileUtils.DeleteFileIfExists(PathUtils.Combine(directoryPath, previousFileName));
                        }
                        Body.AddSiteLog(PublishmentSystemId, "修改包含文件", $"包含文件:{currentFileName}");
						SuccessMessage("包含文件修改成功！");
                        AddWaitAndRedirectScript(PageTemplateInclude.GetRedirectUrl(PublishmentSystemId));
					}
					catch(Exception ex)
					{
						FailMessage(ex, "包含文件修改失败," + ex.Message);
					}
				}
				else
				{
                    var currentFileName = RelatedFileName.Text + ltlCreatedFileExtName.Text;

                    var fileNames = DirectoryUtils.GetFileNames(directoryPath);
                    foreach (var theFileName in fileNames)
                    {
                        if (StringUtils.EqualsIgnoreCase(theFileName, currentFileName))
                        {
                            FailMessage("包含文件添加失败，包含文件文件已存在！");
                            return;
                        }
                    }

                    var charset = ECharsetUtils.GetEnumType(Charset.SelectedValue);
					try
					{
                        FileUtils.WriteText(PathUtils.Combine(directoryPath, currentFileName), charset, Content.Text);
                        Body.AddSiteLog(PublishmentSystemId, "添加包含文件", $"包含文件:{currentFileName}");
						SuccessMessage("包含文件添加成功！");
                        AddWaitAndRedirectScript(PageTemplateInclude.GetRedirectUrl(PublishmentSystemId));
					}
					catch(Exception ex)
					{
                        FailMessage(ex, "包含文件添加失败," + ex.Message);
					}
				}
			}
		}
	}
}
