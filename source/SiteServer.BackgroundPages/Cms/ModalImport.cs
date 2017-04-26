using System;
using System.Collections.Specialized;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.ImportExport;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalImport : BasePageCms
    {
		public HtmlInputFile myFile;
		public RadioButtonList IsOverride;

        private string _type;

        public const int Width = 560;
        public const int Height = 260;
        public const string TypeInput = "INPUT";
        public const string TypeRelatedField = "RELATED_FIELD";
        public const string TypeTagstyle = "TAGSTYLE";
        public const string TypeGatherrule = "GATHERRULE";

        public static string GetOpenWindowString(int publishmentSystemId, string type)
        {
            var title = string.Empty;
            if (StringUtils.EqualsIgnoreCase(type, TypeGatherrule))
            {
                title = "导入采集规则";
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeInput))
            {
                title = "导入提交表单";
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeRelatedField))
            {
                title = "导入联动字段";
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeTagstyle))
            {
                title = "导入模板标签样式";
            }
            return PageUtils.GetOpenWindowString(title, PageUtils.GetCmsUrl(nameof(ModalImport), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"Type", type}
            }), Width, Height);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            _type = Body.GetQueryString("Type");

            if (!IsPostBack)
			{
			
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (StringUtils.EqualsIgnoreCase(_type, TypeGatherrule))
            {
                if (myFile.PostedFile != null && "" != myFile.PostedFile.FileName)
                {
                    var filePath = myFile.PostedFile.FileName;
                    if (EFileSystemTypeUtils.GetEnumType(Path.GetExtension(filePath)) != EFileSystemType.Xml)
                    {
                        FailMessage("必须上传XML文件");
                        return;
                    }

                    try
                    {
                        var localFilePath = PathUtils.GetTemporaryFilesPath(Path.GetFileName(filePath));

                        myFile.PostedFile.SaveAs(localFilePath);

                        var importObject = new ImportObject(PublishmentSystemId);
                        importObject.ImportGatherRule(localFilePath, TranslateUtils.ToBool(IsOverride.SelectedValue));

                        Body.AddSiteLog(PublishmentSystemId, "导入采集规则");

                        PageUtils.CloseModalPage(Page);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "导入采集规则失败！");
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(_type, TypeInput))
            {
                if (myFile.PostedFile != null && "" != myFile.PostedFile.FileName)
                {
                    var filePath = myFile.PostedFile.FileName;
                    if (EFileSystemTypeUtils.GetEnumType(Path.GetExtension(filePath)) != EFileSystemType.Zip)
                    {
                        FailMessage("必须上传ZIP文件");
                        return;
                    }

                    try
                    {
                        var localFilePath = PathUtils.GetTemporaryFilesPath(Path.GetFileName(filePath));

                        myFile.PostedFile.SaveAs(localFilePath);

                        var importObject = new ImportObject(PublishmentSystemId);
                        importObject.ImportInputByZipFile(localFilePath, TranslateUtils.ToBool(IsOverride.SelectedValue));

                        Body.AddSiteLog(PublishmentSystemId, "导入提交表单");

                        PageUtils.CloseModalPage(Page);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "导入提交表单失败！");
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(_type, TypeRelatedField))
            {
                if (myFile.PostedFile != null && "" != myFile.PostedFile.FileName)
                {
                    var filePath = myFile.PostedFile.FileName;
                    if (EFileSystemTypeUtils.GetEnumType(Path.GetExtension(filePath)) != EFileSystemType.Zip)
                    {
                        FailMessage("必须上传ZIP文件");
                        return;
                    }

                    try
                    {
                        var localFilePath = PathUtils.GetTemporaryFilesPath(Path.GetFileName(filePath));

                        myFile.PostedFile.SaveAs(localFilePath);

                        var importObject = new ImportObject(PublishmentSystemId);
                        importObject.ImportRelatedFieldByZipFile(localFilePath, TranslateUtils.ToBool(IsOverride.SelectedValue));

                        Body.AddSiteLog(PublishmentSystemId, "导入联动字段");

                        PageUtils.CloseModalPage(Page);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "导入联动字段失败！");
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(_type, TypeTagstyle))
            {
                if (myFile.PostedFile != null && "" != myFile.PostedFile.FileName)
                {
                    var filePath = myFile.PostedFile.FileName;
                    if (EFileSystemTypeUtils.GetEnumType(Path.GetExtension(filePath)) != EFileSystemType.Xml)
                    {
                        FailMessage("必须上传XML文件");
                        return;
                    }

                    try
                    {
                        var localFilePath = PathUtils.GetTemporaryFilesPath(Path.GetFileName(filePath));

                        myFile.PostedFile.SaveAs(localFilePath);

                        var importObject = new ImportObject(PublishmentSystemId);
                        importObject.ImportTagStyle(localFilePath, TranslateUtils.ToBool(IsOverride.SelectedValue));

                        Body.AddSiteLog(PublishmentSystemId, "导入模板标签样式");

                        PageUtils.CloseModalPage(Page);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "导入模板标签样式失败！");
                    }
                }
            }
		}
	}
}
