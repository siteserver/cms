using System.Text;
using BaiRong.Core;
using SiteServer.CMS.Model;
using System;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Core
{
    public class StringUtility
    {
        private StringUtility()
        {
        }

        /// <summary>
        /// 得到系统图片的路径
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static string GetSystemImageSrc(string imageName, string fileType)
        {
            return $"../pic/System/{imageName}{fileType}";
        }

        /// <summary>
        /// 得到GIF格式的系统图片路径
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public static string GetSystemImageSrc(string imageName)
        {
            return GetSystemImageSrc(imageName, ".gif");
        }

        public static string TextEditorContentEncode(string content, PublishmentSystemInfo publishmentSystemInfo)
        {
            return TextEditorContentEncode(content, publishmentSystemInfo, publishmentSystemInfo.Additional.IsSaveImageInTextEditor);
        }

        public static string TextEditorContentEncode(string content, PublishmentSystemInfo publishmentSystemInfo, bool isSaveImage)
        {
            var publishmentSystemUrl = string.Empty;
            if (publishmentSystemInfo != null)
            {
                publishmentSystemUrl = publishmentSystemInfo.PublishmentSystemUrl;
                if (isSaveImage && !string.IsNullOrEmpty(content))
                {
                    content = PathUtility.SaveImage(publishmentSystemInfo, content);
                }
            }

            var builder = new StringBuilder(content);

            //内容保存之前，把EditorUploadFilePre替换为@符号
            var editorUploadFilePre = publishmentSystemInfo?.Additional.EditorUploadFilePre;
            if (!string.IsNullOrEmpty(editorUploadFilePre))
            {
                builder.Replace("href=\"" + editorUploadFilePre + "/upload", "href=\"@/upload");
                builder.Replace("href='" + editorUploadFilePre + "/upload", "href='@/upload");
                builder.Replace("href=" + editorUploadFilePre + "/upload", "href=@/upload");
                builder.Replace("href=&quot;" + editorUploadFilePre + "/upload", "href=&quot;@/upload");
                builder.Replace("src=\"" + editorUploadFilePre + "/upload", "src=\"@/upload");
                builder.Replace("src='" + editorUploadFilePre + "/upload", "src='@/upload");
                builder.Replace("src=" + editorUploadFilePre + "/upload", "src=@/upload");
                builder.Replace("src=&quot;" + editorUploadFilePre + "/upload", "src=&quot;@/upload");
            }

            if (publishmentSystemUrl == "/")
            {
                publishmentSystemUrl = string.Empty;
            }

            builder.Replace("href=\"" + publishmentSystemUrl, "href=\"@");
            builder.Replace("href='" + publishmentSystemUrl, "href='@");
            builder.Replace("href=" + publishmentSystemUrl, "href=@");
            builder.Replace("href=&quot;" + publishmentSystemUrl, "href=&quot;@");
            builder.Replace("src=\"" + publishmentSystemUrl, "src=\"@");
            builder.Replace("src='" + publishmentSystemUrl, "src='@");
            builder.Replace("src=" + publishmentSystemUrl, "src=@");
            builder.Replace("src=&quot;" + publishmentSystemUrl, "src=&quot;@");

            builder.Replace("@'@", "'@");
            builder.Replace("@\"@", "\"@");

            return builder.ToString();
        }

        /// <summary>
        /// 获取编辑器中内容，解析@符号，添加了远程路径处理 20151103
        /// </summary>
        /// <param name="content"></param>
        /// <param name="publishmentSystemInfo"></param>
        /// <param name="isFromBack">是否是后台请求</param>
        /// <returns></returns>
        public static string TextEditorContentDecode(string content, PublishmentSystemInfo publishmentSystemInfo, bool isFromBack)
        {
            var publishmentSystemUrl = publishmentSystemInfo != null ? publishmentSystemInfo.PublishmentSystemUrl : WebConfigUtils.ApplicationPath;
            return TextEditorContentDecode(content, publishmentSystemUrl, publishmentSystemInfo?.Additional.EditorUploadFilePre, isFromBack);
        }

        /// <summary>
        /// 获取编辑器中内容，解析@符号，添加了远程路径处理 20151103
        /// </summary>
        /// <param name="content"></param>
        /// <param name="publishmentSystemInfo"></param>
        /// <returns></returns>
        public static string TextEditorContentDecode(string content, PublishmentSystemInfo publishmentSystemInfo)
        {
            return TextEditorContentDecode(content, publishmentSystemInfo, false);
        }

        public static string TextEditorContentDecode(string content, string publishmentSystemUrl, string editorUploadFilePre, bool isFromBack)
        {
            var builder = new StringBuilder(content);

            if (publishmentSystemUrl == "/")
            {
                publishmentSystemUrl = string.Empty;
            }

            if (!isFromBack && !string.IsNullOrEmpty(editorUploadFilePre))
            {
                builder.Replace("href=\"@/upload", "href=\"" + editorUploadFilePre + "/upload");
                builder.Replace("href='@/upload", "href='" + editorUploadFilePre + "/upload");
                builder.Replace("href=@/upload", "href=" + editorUploadFilePre + "/upload");
                builder.Replace("href=&quot;@/upload", "href=&quot;" + editorUploadFilePre + "/upload");
                builder.Replace("src=\"@/upload", "src=\"" + editorUploadFilePre + "/upload");
                builder.Replace("src='@/upload", "src='" + editorUploadFilePre + "/upload");
                builder.Replace("src=@/upload", "src=" + editorUploadFilePre + "/upload");
                builder.Replace("src=&quot;@/upload", "src=&quot;" + editorUploadFilePre + "/upload");
            }

            builder.Replace("href=\"@", "href=\"" + publishmentSystemUrl);
            builder.Replace("href='@", "href='" + publishmentSystemUrl);
            builder.Replace("href=@", "href=" + publishmentSystemUrl);
            builder.Replace("href=&quot;@", "href=&quot;" + publishmentSystemUrl);
            builder.Replace("src=\"@", "src=\"" + publishmentSystemUrl);
            builder.Replace("src='@", "src='" + publishmentSystemUrl);
            builder.Replace("src=@", "src=" + publishmentSystemUrl);
            builder.Replace("src=&quot;@", "src=&quot;" + publishmentSystemUrl);

            builder.Replace("&#xa0;", "&nbsp;");

            return builder.ToString();
        }

        public class Template
        {
            public static string GetSearchTemplateContent(ECharset charset)
            {
                return
                    $@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset={ECharsetUtils.GetValue(charset)}"" />
<title>搜索</title>
<style>
*{{font-size:12px}}
</style>
</head>

<body>

<stl:searchInput openwin=""false"" isLoadValues=""true""></stl:searchInput>

<br />

<stl:searchOutput isHighlight=""true""></stl:searchOutput>

</body>
</html>";
            }

            public static string GetCommentsTemplateContent(ECharset charset)
            {
                return
                    $@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset={ECharsetUtils.GetValue(charset)}"" />
<title>评论</title>
<style>
*{{font-size:12px}}
</style>
</head>

<body>

<stl:comments isPage=""true"" pageNum=""20"" isLinkToAll=""false""></stl:comments>

<br />

<stl:commentInput isDynamic=""true""></stl:commentInput>

</body>
</html>";
            }
        }
    }
}
