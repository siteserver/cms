using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Text.LitJson;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalTextEditorImportWord : BasePageCms
    {
        public CheckBox CbIsClearFormat;
        public CheckBox CbIsFirstLineIndent;
        public CheckBox CbIsClearFontSize;
        public CheckBox CbIsClearFontFamily;
        public CheckBox CbIsClearImages;

        private string _attributeName;

        public static string GetOpenWindowString(int publishmentSystemId, string attributeName)
        {
            return PageUtils.GetOpenWindowString("导入Word", PageUtils.GetCmsUrl(nameof(ModalTextEditorImportWord), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"AttributeName", attributeName}
            }), 550, 350);
        }

        public string UploadUrl => PageUtils.GetCmsUrl(nameof(ModalTextEditorImportWord), new NameValueCollection
        {
            {"PublishmentSystemID", PublishmentSystemId.ToString()},
            {"upload", true.ToString()}
        });

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("upload"))
            {
                var attributes = Upload();
                var json = JsonMapper.ToJson(attributes);
                Response.Write(json);
                Response.End();
                return;
            }

            _attributeName = Body.GetQueryString("AttributeName");
		}

        private Hashtable Upload()
        {
            var success = false;
            var fileName = string.Empty;
            var message = "Word上传失败";

            if (Request.Files["filedata"] != null)
            {
                var postedFile = Request.Files["filedata"];
                try
                {
                    if (!string.IsNullOrEmpty(postedFile?.FileName))
                    {
                        fileName = postedFile.FileName;
                        var extendName = fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal)).ToLower();
                        if (extendName == ".doc" || extendName == ".docx")
                        {
                            var filePath = WordUtils.GetWordFilePath(fileName);
                            postedFile.SaveAs(filePath);

                            success = true;
                        }
                        else
                        {
                            FailMessage("请选择Word文件上传！");
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }

            var jsonAttributes = new Hashtable();
            if (success)
            {
                jsonAttributes.Add("success", "true");
                jsonAttributes.Add("fileName", fileName);
            }
            else
            {
                jsonAttributes.Add("success", "false");
                jsonAttributes.Add("message", message);
            }

            return jsonAttributes;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var fileNames = Request.Form["fileNames"];
            if (!string.IsNullOrEmpty(fileNames))
            {
                fileNames = fileNames.Trim('|');
                var builder = new StringBuilder();

                foreach (var fileName in fileNames.Split('|'))
                {
                    var filePath = WordUtils.GetWordFilePath(fileName);
                    var wordContent = WordUtils.Parse(PublishmentSystemId, filePath, CbIsClearFormat.Checked, CbIsFirstLineIndent.Checked, CbIsClearFontSize.Checked, CbIsClearFontFamily.Checked, CbIsClearImages.Checked);
                    wordContent = StringUtility.TextEditorContentDecode(wordContent, PublishmentSystemInfo, true);
                    builder.Append(wordContent);
                    FileUtils.DeleteFileIfExists(filePath);
                }
                var script = "parent." + ETextEditorTypeUtils.GetInsertHtmlScript(_attributeName, builder.ToString());
                PageUtils.CloseModalPageWithoutRefresh(Page, script);
            }
            else
            {
                FailMessage("请选择Word文件上传！");
            }
        }
	}
}
