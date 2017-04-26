using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalUploadImageSingleSystem : BasePageCms
    {
        public HtmlInputFile hifUpload;
        public Literal ltlScript;

        private string _textBoxClientId;

        protected override bool IsSinglePage => true;

        public static string GetOpenWindowStringToTextBox(string textBoxClientId)
        {
            return PageUtils.GetOpenWindowString("上传图片", PageUtils.GetCmsUrl(nameof(ModalUploadImageSingleSystem), new NameValueCollection
            {
                {"TextBoxClientID", textBoxClientId}
            }), 480, 220);
        }

        public static string GetOpenWindowStringToList()
        { 
            return PageUtils.GetOpenWindowString("上传图片", PageUtils.GetCmsUrl(nameof(ModalUploadImageSingleSystem), null), 480, 220);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
             
            _textBoxClientId = Body.GetQueryString("TextBoxClientID");
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (hifUpload.PostedFile != null && "" != hifUpload.PostedFile.FileName)
            {
                var filePath = hifUpload.PostedFile.FileName;
                try
                {
                    var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                    var localDirectoryPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\upload\\images";
                    //Server.MapPath(".");
                    DirectoryUtils.CreateDirectoryIfNotExists(localDirectoryPath);

                    var localFileName = GetUploadFileName(filePath, DateTime.Now, true) ;
                    var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                    if (!PathUtils.IsFileExtenstionAllowed("jpg|gif|png|bmp", fileExtName))
                    {
                        FailMessage("上传失败，上传图片格式不正确！");
                        return;
                    }
                    if (hifUpload.PostedFile.ContentLength > 10 * 1024)
                    {
                        FailMessage("上传失败，上传图片超出规定文件大小！");
                        return;
                    }

                    hifUpload.PostedFile.SaveAs(localFilePath);

                    var isImage = EFileSystemTypeUtils.IsImage(fileExtName);


                    if (string.IsNullOrEmpty(_textBoxClientId))
                    {
                        PageUtils.CloseModalPage(Page);
                    }
                    else
                    {
                        var textBoxUrl = "@/upload/images/" + localFileName;

                        ltlScript.Text += $@"
if (parent.document.getElementById('{_textBoxClientId}') != null)
{{
    parent.document.getElementById('{_textBoxClientId}').value = '{textBoxUrl}';
}}
";

                        ltlScript.Text += PageUtils.HidePopWin;
                    }
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "图片上传失败！");
                }
            }
        }

        public string GetUploadFileName(string filePath, DateTime now, bool isUploadChangeFileName)
        {
            var retval = string.Empty;

            if (isUploadChangeFileName)
            {
                string strDateTime = $"{now.Day}{now.Hour}{now.Minute}{now.Second}{now.Millisecond}";
                retval = $"{strDateTime}{PathUtils.GetExtension(filePath)}";
            }
            else
            {
                retval = PathUtils.GetFileName(filePath);
            }

            retval = StringUtils.ReplaceIgnoreCase(retval, "as", string.Empty);
            retval = StringUtils.ReplaceIgnoreCase(retval, ";", string.Empty);
            return retval;
        }

    }
}
