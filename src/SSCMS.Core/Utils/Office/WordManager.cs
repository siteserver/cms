using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using HtmlAgilityPack;
using OpenXmlPowerTools;
using SSCMS.Core.Utils.Office.Word2Html;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Office
{
    public class WordManager
    {
        private string ImageDirectoryPath { get; set; }
        private string ImageDirectoryUrl { get; set; }
        private bool IsFirstLineTitle { get; set; }
        private bool IsClearFormat { get; set; }
        private bool IsFirstLineIndent { get; set; }
        private bool IsClearFontSize { get; set; }
        private bool IsClearFontFamily { get; set; }
        private bool IsClearImages { get; set; }
        private string DocsFilePath { get; set; }
        private string DocsFileTitle { get; set; }
        private IPathManager PathManager { get; set; }
        private Site Site { get; set; }

        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Body { get; set; }

        public WordManager(bool isFirstLineTitle, bool isClearFormat, bool isFirstLineIndent, bool isClearFontSize, bool isClearFontFamily, bool isClearImages, string docsFilePath, string docsFileTitle)
        {
            IsFirstLineTitle = isFirstLineTitle;
            IsClearFormat = isClearFormat;
            IsFirstLineIndent = isFirstLineIndent;
            IsClearFontSize = isClearFontSize;
            IsClearFontFamily = isClearFontFamily;
            IsClearImages = isClearImages;
            DocsFilePath = docsFilePath;
            DocsFileTitle = docsFileTitle;
        }

        public async Task InitAsync(IPathManager pathManager, Site siteInfo)
        {
            if (siteInfo != null)
            {
                ImageDirectoryPath = await pathManager.GetUploadDirectoryPathAsync(siteInfo, UploadType.Image);
                ImageDirectoryUrl = await pathManager.GetSiteUrlByPhysicalPathAsync(siteInfo, ImageDirectoryPath, true);
            }
            else
            {
                var fileName = PathUtils.GetFileName(DocsFilePath);
                ImageDirectoryPath = PathUtils.Combine(pathManager.WebRootPath, PathUtils.GetMaterialVirtualDirectoryPath(UploadType.Image));
                ImageDirectoryUrl = PathUtils.GetMaterialVirtualFilePath(UploadType.Image, fileName);
            }

            PathManager = pathManager;
            Site = siteInfo;
        }

        public async Task ParseAsync(IPathManager pathManager, Site siteInfo)
        {
            await InitAsync(pathManager, siteInfo);

            try
            {
                ConvertToHtml();
                if (string.IsNullOrEmpty(Body))
                {
                    await ConvertToHtmlAsync();
                }
            }
            catch
            {
                await ConvertToHtmlAsync();
            }

            SSCMS.Utils.FileUtils.DeleteFileIfExists(DocsFilePath);

            if (Site != null)
            {
                Body = await pathManager.DecodeTextEditorAsync(Site, Body, true);
            }

            if (string.IsNullOrEmpty(Title))
            {
                Title = DocsFileTitle;
            }

            if (IsFirstLineTitle)
            {
                var contentTitle = RegexUtils.GetInnerContent("p", Body);
                contentTitle = StringUtils.StripTags(contentTitle);
                if (!string.IsNullOrEmpty(contentTitle))
                {
                    contentTitle = contentTitle.Trim();
                    contentTitle = contentTitle.Trim('　', ' ');
                    contentTitle = StringUtils.StripEntities(contentTitle);
                }

                if (!string.IsNullOrEmpty(contentTitle))
                {
                    Title = contentTitle;
                }
            }

            if (IsClearFormat)
            {
                Body = HtmlUtils.ClearFormat(Body);
            }

            if (IsFirstLineIndent)
            {
                Body = HtmlUtils.FirstLineIndent(Body);
            }

            if (IsClearFontSize)
            {
                Body = HtmlUtils.ClearFontSize(Body);
            }

            if (IsClearFontFamily)
            {
                Body = HtmlUtils.ClearFontFamily(Body);
            }
        }

        public async Task ConvertToHtmlAsync()
        {
            FileStream stream = new FileStream(DocsFilePath, FileMode.Open, FileAccess.Read);
            var npoiDoc = new NpoiDoc();
            Body = await npoiDoc.NpoiDocx(stream, UploadImageUrlDelegate);
        }

        private string UploadImageUrlDelegate(byte[] imgByte, string picType)
        {
            var extension = StringUtils.ToLower(picType.Split('/')[1]);
            var imageFileName = StringUtils.GetShortGuid(false) + "." + extension;

            var imageFilePath = PathUtils.Combine(ImageDirectoryPath, imageFileName);
            try
            {
                ImageUtils.Save(imgByte, imageFilePath);

                if (Site.IsImageAutoResize)
                {
                    ImageUtils.ResizeImageIfExceeding(imageFilePath, Site.ImageAutoResizeWidth);
                }

                // AddWaterMarkAsync(Site, imageFilePath);

                var imgSrc = PageUtils.Combine(ImageDirectoryUrl, imageFileName);

                if (string.IsNullOrEmpty(ImageUrl))
                {
                    ImageUrl = imgSrc;
                }

                return imgSrc;
            }
            catch
            {

            }

            return $"data:{picType};base64,{Convert.ToBase64String(imgByte)}";
        }

        private void ConvertToHtml()
        {
            var fi = new FileInfo(DocsFilePath);

            var byteArray = File.ReadAllBytes(fi.FullName);
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(byteArray, 0, byteArray.Length);

                using (var wDoc = WordprocessingDocument.Open(memoryStream, true))
                {
                    var part = wDoc.CoreFilePropertiesPart;
                    if (part != null)
                    {
                        Title = (string)part.GetXDocument().Descendants(DC.title).FirstOrDefault();
                    }

                    var htmlSettings = new HtmlConverterSettings
                    {
                        // AdditionalCss = "body { margin: 1cm auto; max-width: 20cm; padding: 0; }",
                        PageTitle = Title,
                        FabricateCssClasses = true,
                        CssClassPrefix = "pt-",
                        RestrictToSupportedLanguages = false,
                        RestrictToSupportedNumberingFormats = false,
                        ImageHandler = imageInfo =>
                        {
                            if (IsClearImages || string.IsNullOrEmpty(ImageDirectoryPath)) return null;
                            DirectoryUtils.CreateDirectoryIfNotExists(ImageDirectoryPath);

                            var extension = StringUtils.ToLower(imageInfo.ContentType.Split('/')[1]);
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

                            var imageFilePath = PathUtils.Combine(ImageDirectoryPath, imageFileName);
                            try
                            {
                                imageInfo.Bitmap.Save(imageFilePath, imageFormat);
                            }
                            catch (System.Runtime.InteropServices.ExternalException)
                            {
                                return null;
                            }
                            var imageSource = PageUtils.Combine(ImageDirectoryUrl, imageFileName);
                            if (string.IsNullOrEmpty(ImageUrl))
                            {
                                ImageUrl = imageSource;
                            }

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

                    Body = $"{style}{Environment.NewLine}{body}";
                }
            }
        }
    }
}
