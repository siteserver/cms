using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using HtmlAgilityPack;
using OpenXmlPowerTools;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;
using FileUtils = SSCMS.Utils.FileUtils;

namespace SSCMS.Core.Utils.Office
{
    public static class WordManager
    {
        public class ConverterSettings
        {
            public bool IsFirstLineTitle { get; set; }
            public bool IsClearFormat { get; set; }
            public bool IsFirstLineIndent { get; set; }
            public bool IsClearFontSize { get; set; }
            public bool IsClearFontFamily { get; set; }
            public bool IsClearImages { get; set; }
            public bool IsSaveHtml { get; set; }
            public string HtmlDirectoryPath { get; set; }
            public string ImageDirectoryPath { get; set; }
            public string ImageDirectoryUrl { get; set; }
        }

        public static async Task<(string title, string body)> GetWordAsync(IPathManager pathManager, Site siteInfo, bool isFirstLineTitle, bool isClearFormat, bool isFirstLineIndent, bool isClearFontSize, bool isClearFontFamily, bool isClearImages, string docsFilePath)
        {
            string imageDirectoryPath;
            string imageDirectoryUrl;
            if (siteInfo != null)
            {
                imageDirectoryPath = await pathManager.GetUploadDirectoryPathAsync(siteInfo, UploadType.Image);
                imageDirectoryUrl = await pathManager.GetSiteUrlByPhysicalPathAsync(siteInfo, imageDirectoryPath, true);
            }
            else
            {
                var fileName = PathUtils.GetFileName(docsFilePath);
                imageDirectoryPath = PathUtils.Combine(pathManager.WebRootPath, PathUtils.GetLibraryVirtualDirectoryPath(UploadType.Image));
                imageDirectoryUrl = PathUtils.GetLibraryVirtualFilePath(UploadType.Image, fileName);
            }

            var settings = new ConverterSettings
            {
                IsFirstLineTitle = isFirstLineTitle,
                IsClearFormat = isClearFormat,
                IsFirstLineIndent = isFirstLineIndent,
                IsClearFontSize = isClearFontSize,
                IsClearFontFamily = isClearFontFamily,
                IsClearImages = isClearImages,
                ImageDirectoryPath = imageDirectoryPath,
                ImageDirectoryUrl = imageDirectoryUrl,
                IsSaveHtml = false
            };

            var (title, body) = ConvertToHtml(docsFilePath, settings);

            FileUtils.DeleteFileIfExists(docsFilePath);

            if (siteInfo != null)
            {
                body = await pathManager.DecodeTextEditorAsync(siteInfo, body, true);
            }

            return (title, body);
        }

        public static (string title, string body) ConvertToHtml(string docxFilePath, ConverterSettings settings)
        {
            string title;
            string content;
            var fi = new FileInfo(docxFilePath);

            var byteArray = File.ReadAllBytes(fi.FullName);
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(byteArray, 0, byteArray.Length);
                using (var wDoc = WordprocessingDocument.Open(memoryStream, true))
                {
                    title = fi.FullName;
                    var part = wDoc.CoreFilePropertiesPart;
                    if (part != null)
                    {
                        title = (string)part.GetXDocument().Descendants(DC.title).FirstOrDefault() ?? fi.FullName;
                    }

                    title = PathUtils.GetFileNameWithoutExtension(title);

                    // TODO: Determine max-width from size of content area.
                    var htmlSettings = new HtmlConverterSettings
                    {
                        // AdditionalCss = "body { margin: 1cm auto; max-width: 20cm; padding: 0; }",
                        PageTitle = title,
                        FabricateCssClasses = true,
                        CssClassPrefix = "pt-",
                        RestrictToSupportedLanguages = false,
                        RestrictToSupportedNumberingFormats = false,
                        ImageHandler = imageInfo =>
                        {
                            if (settings.IsClearImages || string.IsNullOrEmpty(settings.ImageDirectoryPath)) return null;
                            DirectoryUtils.CreateDirectoryIfNotExists(settings.ImageDirectoryPath);

                            var extension = imageInfo.ContentType.Split('/')[1].ToLower();
                            ImageFormat imageFormat = null;
                            if (extension == "png")
                                imageFormat = ImageFormat.Png;
                            else if (extension == "gif")
                                imageFormat = ImageFormat.Gif;
                            else if (extension == "bmp")
                                imageFormat = ImageFormat.Bmp;
                            else if (extension == "jpeg")
                                imageFormat = ImageFormat.Jpeg;
                            else if (extension == "tiff")
                            {
                                // Convert tiff to gif.
                                extension = "gif";
                                imageFormat = ImageFormat.Gif;
                            }
                            else if (extension == "x-wmf")
                            {
                                extension = "wmf";
                                imageFormat = ImageFormat.Wmf;
                            }

                            // If the image format isn't one that we expect, ignore it,
                            // and don't return markup for the link.
                            if (imageFormat == null)
                                return null;

                            var imageFileName = StringUtils.GetShortGuid(false) + "." + extension;

                            var imageFilePath = PathUtils.Combine(settings.ImageDirectoryPath, imageFileName);
                            try
                            {
                                imageInfo.Bitmap.Save(imageFilePath, imageFormat);
                            }
                            catch (System.Runtime.InteropServices.ExternalException)
                            {
                                return null;
                            }
                            var imageSource = PageUtils.Combine(settings.ImageDirectoryUrl, imageFileName);

                            var img = new XElement(Xhtml.img,
                                new XAttribute(NoNamespace.src, imageSource),
                                imageInfo.ImgStyleAttribute,
                                imageInfo.AltText != null ?
                                    new XAttribute(NoNamespace.alt, imageInfo.AltText) : null);
                            return img;
                        }
                    };
                    var htmlElement = HtmlConverter.ConvertToHtml(wDoc, htmlSettings);

                    // Produce HTML document with <!DOCTYPE html > declaration to tell the browser
                    // we are using HTML5.
                    var html = new XDocument(
                        new XDocumentType("html", null, null, null),
                        htmlElement);

                    // Note: the xhtml returned by ConvertToHtmlTransform contains objects of type
                    // XEntity.  PtOpenXmlUtil.cs define the XEntity class.  See
                    // http://blogs.msdn.com/ericwhite/archive/2010/01/21/writing-entity-references-using-linq-to-xml.aspx
                    // for detailed explanation.
                    //
                    // If you further transform the XML tree returned by ConvertToHtmlTransform, you
                    // must do it correctly, or entities will not be serialized properly.

                    var htmlString = html.ToString(SaveOptions.DisableFormatting);
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(htmlString);
                    var style = htmlDoc.DocumentNode.SelectSingleNode("//style").OuterHtml;
                    var body = htmlDoc.DocumentNode.SelectSingleNode("//body").InnerHtml;

                    // var style = HtmlToWmlConverter.CleanUpCss((string)htmlElement.Descendants().FirstOrDefault(d => d.Name.LocalName.ToLower() == "style"));

                    content = $"{style}{Environment.NewLine}{body}";

                    if (settings.IsSaveHtml && !string.IsNullOrEmpty(settings.HtmlDirectoryPath) && DirectoryUtils.IsDirectoryExists(settings.HtmlDirectoryPath))
                    {
                        var htmlFilePath = PathUtils.Combine(settings.HtmlDirectoryPath, PathUtils.GetFileNameWithoutExtension(docxFilePath) + ".html");
                        File.WriteAllText(htmlFilePath, htmlString, Encoding.UTF8);
                    }
                }
            }

            if (settings.IsFirstLineTitle)
            {
                var contentTitle = RegexUtils.GetInnerContent("p", content);
                contentTitle = StringUtils.StripTags(contentTitle);
                if (!string.IsNullOrEmpty(contentTitle))
                {
                    contentTitle = contentTitle.Trim();
                    contentTitle = contentTitle.Trim('　', ' ');
                    contentTitle = StringUtils.StripEntities(contentTitle);
                }

                if (!string.IsNullOrEmpty(contentTitle))
                {
                    title = contentTitle;
                }
            }

            if (settings.IsClearFormat)
            {
                content = HtmlClearUtils.ClearFormat(content);
            }

            if (settings.IsFirstLineIndent)
            {
                content = HtmlClearUtils.FirstLineIndent(content);
            }

            if (settings.IsClearFontSize)
            {
                content = HtmlClearUtils.ClearFontSize(content);
            }

            if (settings.IsClearFontFamily)
            {
                content = HtmlClearUtils.ClearFontFamily(content);
            }

            if (string.IsNullOrEmpty(title))
            {
                title = PathUtils.GetFileNameWithoutExtension(docxFilePath);
            }

            return (title, content);
        }

        public static void ConvertToDocx(string file, string destinationDir)
        {
            var sourceHtmlFi = new FileInfo(file);

            var destCssFi = new FileInfo(Path.Combine(destinationDir, sourceHtmlFi.Name.Replace(".html", "-2.css")));
            var destDocxFi = new FileInfo(Path.Combine(destinationDir, sourceHtmlFi.Name.Replace(".html", "-3-ConvertedByHtmlToWml.docx")));
            var annotatedHtmlFi = new FileInfo(Path.Combine(destinationDir, sourceHtmlFi.Name.Replace(".html", "-4-Annotated.txt")));

            var html = ReadAsXElement(sourceHtmlFi);

            var usedAuthorCss = HtmlToWmlConverter.CleanUpCss((string)html.Descendants().FirstOrDefault(d => d.Name.LocalName.ToLower() == "style"));
            File.WriteAllText(destCssFi.FullName, usedAuthorCss);

            var settings = HtmlToWmlConverter.GetDefaultSettings();
            // image references in HTML files contain the path to the subdir that contains the images, so base URI is the name of the directory
            // that contains the HTML files
            settings.BaseUriForImages = sourceHtmlFi.DirectoryName;

            var doc = HtmlToWmlConverter.ConvertHtmlToWml(DefaultCss, usedAuthorCss, UserCss, html, settings, null, annotatedHtmlFi.FullName);
            doc.SaveAs(destDocxFi.FullName);
        }

        private static XElement ReadAsXElement(FileInfo sourceHtmlFi)
        {
            var htmlString = File.ReadAllText(sourceHtmlFi.FullName);
            XElement html;
            try
            {
                html = XElement.Parse(htmlString);
            }
            catch (XmlException)
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.Load(sourceHtmlFi.FullName, Encoding.Default);
                htmlDoc.OptionOutputAsXml = true;
                htmlDoc.Save(sourceHtmlFi.FullName, Encoding.Default);
                var sb = new StringBuilder(File.ReadAllText(sourceHtmlFi.FullName, Encoding.Default));
                sb.Replace("&amp;", "&");
                sb.Replace("&nbsp;", "\xA0");
                sb.Replace("&quot;", "\"");
                sb.Replace("&lt;", "~lt;");
                sb.Replace("&gt;", "~gt;");
                sb.Replace("&#", "~#");
                sb.Replace("&", "&amp;");
                sb.Replace("~lt;", "&lt;");
                sb.Replace("~gt;", "&gt;");
                sb.Replace("~#", "&#");
                File.WriteAllText(sourceHtmlFi.FullName, sb.ToString(), Encoding.Default);
                html = XElement.Parse(sb.ToString());
            }
            // HtmlToWmlConverter expects the HTML elements to be in no namespace, so convert all elements to no namespace.
            html = (XElement)ConvertToNoNamespace(html);
            return html;
        }

        private static object ConvertToNoNamespace(XNode node)
        {
            var element = node as XElement;
            if (element != null)
            {
                return new XElement(element.Name.LocalName,
                    element.Attributes().Where(a => !a.IsNamespaceDeclaration),
                    element.Nodes().Select(n => ConvertToNoNamespace(n)));
            }
            return node;
        }

        private const string DefaultCss =
            @"html, address,
blockquote,
body, dd, div,
dl, dt, fieldset, form,
frame, frameset,
h1, h2, h3, h4,
h5, h6, noframes,
ol, p, ul, center,
dir, hr, menu, pre { display: block; unicode-bidi: embed }
li { display: list-item }
head { display: none }
table { display: table }
tr { display: table-row }
thead { display: table-header-group }
tbody { display: table-row-group }
tfoot { display: table-footer-group }
col { display: table-column }
colgroup { display: table-column-group }
td, th { display: table-cell }
caption { display: table-caption }
th { font-weight: bolder; text-align: center }
caption { text-align: center }
body { margin: auto; }
h1 { font-size: 2em; margin: auto; }
h2 { font-size: 1.5em; margin: auto; }
h3 { font-size: 1.17em; margin: auto; }
h4, p,
blockquote, ul,
fieldset, form,
ol, dl, dir,
menu { margin: auto }
a { color: blue; }
h5 { font-size: .83em; margin: auto }
h6 { font-size: .75em; margin: auto }
h1, h2, h3, h4,
h5, h6, b,
strong { font-weight: bolder }
blockquote { margin-left: 40px; margin-right: 40px }
i, cite, em,
var, address { font-style: italic }
pre, tt, code,
kbd, samp { font-family: monospace }
pre { white-space: pre }
button, textarea,
input, select { display: inline-block }
big { font-size: 1.17em }
small, sub, sup { font-size: .83em }
sub { vertical-align: sub }
sup { vertical-align: super }
table { border-spacing: 2px; }
thead, tbody,
tfoot { vertical-align: middle }
td, th, tr { vertical-align: inherit }
s, strike, del { text-decoration: line-through }
hr { border: 1px inset }
ol, ul, dir,
menu, dd { margin-left: 40px }
ol { list-style-type: decimal }
ol ul, ul ol,
ul ul, ol ol { margin-top: 0; margin-bottom: 0 }
u, ins { text-decoration: underline }
br:before { content: ""\A""; white-space: pre-line }
center { text-align: center }
:link, :visited { text-decoration: underline }
:focus { outline: thin dotted invert }
/* Begin bidirectionality settings (do not change) */
BDO[DIR=""ltr""] { direction: ltr; unicode-bidi: bidi-override }
BDO[DIR=""rtl""] { direction: rtl; unicode-bidi: bidi-override }
*[DIR=""ltr""] { direction: ltr; unicode-bidi: embed }
*[DIR=""rtl""] { direction: rtl; unicode-bidi: embed }
";

        private const string UserCss = @"";
    }
}
