using System.Collections.Specialized;
using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "文件下载链接", Description = "通过 stl:file 标签在模板中显示文件下载链接")]
    public class StlFile
	{
	    private StlFile() { }

	    public const string ElementName = "stl:file";

	    [StlAttribute(Title = "指定存储附件的字段")]
	    private const string Type = nameof(Type);

        [StlAttribute(Title = "显示字段的顺序")]
        private const string No = nameof(No);

		[StlAttribute(Title = "需要下载的文件地址")]
        private const string Src = nameof(Src);

	    [StlAttribute(Title = "仅显示文件名称")]
	    private const string IsFileName = nameof(IsFileName);

	    [StlAttribute(Title = "仅显示文件类型")]
	    private const string IsFileType = nameof(IsFileType);

        [StlAttribute(Title = "仅显示文件大小")]
        private const string IsFileSize = nameof(IsFileSize);

        [StlAttribute(Title = "仅显示下载次数")]
        private const string IsCount = nameof(IsCount);

	    [StlAttribute(Title = "是否转换为小写")]
	    private const string IsLower = nameof(IsLower);

	    [StlAttribute(Title = "是否转换为大写")]
	    private const string IsUpper = nameof(IsUpper);

        [StlAttribute(Title = "显示在信息前的文字")]
        private const string LeftText = nameof(LeftText);

        [StlAttribute(Title = "显示在信息后的文字")]
        private const string RightText = nameof(RightText);

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var type = BackgroundContentAttribute.FileUrl;
            var no = 0;
            var src = string.Empty;
            var isFileName = false;
            var isFileType = false;
            var isFileSize = false;
            var isCount = false;
            var isLower = false;
            var isUpper = false;
            var leftText = string.Empty;
            var rightText = string.Empty;
            var attributes = new NameValueCollection();

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, No))
                {
                    no = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Src))
                {
                    src = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsFileName))
                {
                    isFileName = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsFileType))
                {
                    isFileType = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsFileSize))
                {
                    isFileSize = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsCount))
                {
                    isCount = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsLower))
                {
                    isLower = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsUpper))
                {
                    isUpper = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, LeftText))
                {
                    leftText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, RightText))
                {
                    rightText = value;
                }
                else
                {
                    attributes[name] = value;
                }
            }

            return ParseImpl(pageInfo, contextInfo, type, no, src, isFileName, isFileType, isFileSize, isCount, isLower, isUpper, leftText, rightText, attributes);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string type, int no, string src, bool isFileName, bool isFileType, bool isFileSize, bool isCount, bool isLower, bool isUpper, string leftText, string rightText, NameValueCollection attributes)
        {
            if (!string.IsNullOrEmpty(contextInfo.InnerHtml))
            {
                var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                contextInfo.InnerHtml = innerBuilder.ToString();
            }

            var fileUrl = string.Empty;
            if (!string.IsNullOrEmpty(src))
            {
                fileUrl = src;
            }
            else
            {
                if (contextInfo.ContextType == EContextType.Undefined)
                {
                    contextInfo.ContextType = EContextType.Content;
                }
                if (contextInfo.ContextType == EContextType.Content)
                {
                    if (contextInfo.ContentId != 0)
                    {
                        var contentInfo = contextInfo.ContentInfo;

                        if (!string.IsNullOrEmpty(contentInfo?.GetString(type)))
                        {
                            if (no <= 1)
                            {
                                fileUrl = contentInfo.GetString(StringUtils.EqualsIgnoreCase(type, BackgroundContentAttribute.FileUrl) ? BackgroundContentAttribute.FileUrl : type);
                            }
                            else
                            {
                                var extendAttributeName = ContentAttribute.GetExtendAttributeName(type);
                                var extendValues = contentInfo.GetString(extendAttributeName);
                                if (!string.IsNullOrEmpty(extendValues))
                                {
                                    var index = 2;
                                    foreach (var extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                    {
                                        if (index == no)
                                        {
                                            fileUrl = extendValue;

                                            break;
                                        }
                                        index++;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (contextInfo.ContextType == EContextType.Each)
                {
                    fileUrl = contextInfo.ItemContainer.EachItem.DataItem as string;
                }
            }

            string parsedContent;

            if (isFileName)
            {
                parsedContent = PathUtils.RemoveExtension(PageUtils.GetFileNameFromUrl(fileUrl));
                if (isLower)
                {
                    parsedContent = parsedContent.ToLower();
                }
                if (isUpper)
                {
                    parsedContent = parsedContent.ToUpper();
                }
            }
            else if (isFileType)
            {
                var filePath = PathUtility.MapPath(pageInfo.SiteInfo, fileUrl);
                parsedContent = PathUtils.GetExtension(filePath).Trim('.');
                if (isLower)
                {
                    parsedContent = parsedContent.ToLower();
                }
                if (isUpper)
                {
                    parsedContent = parsedContent.ToUpper();
                }
            }
            else if (isFileSize)
            {
                var filePath = PathUtility.MapPath(pageInfo.SiteInfo, fileUrl);
                parsedContent = FileUtils.GetFileSizeByFilePath(filePath);
            }
            else if (isCount)
            {
                parsedContent = (contextInfo.ContentInfo?.Downloads ?? 0).ToString();
            }
            else
            {
                parsedContent = contextInfo.ContentInfo != null
                    ? InputParserUtility.GetFileHtmlWithCount(pageInfo.SiteInfo, contextInfo.ContentInfo.ChannelId,
                        contextInfo.ContentInfo.Id, fileUrl, attributes, contextInfo.InnerHtml,
                        contextInfo.IsStlEntity, isLower, isUpper)
                    : InputParserUtility.GetFileHtmlWithoutCount(pageInfo.SiteInfo, fileUrl, attributes,
                        contextInfo.InnerHtml, contextInfo.IsStlEntity, isLower, isUpper);
            }

            if (!string.IsNullOrEmpty(parsedContent))
            {
                parsedContent = leftText + parsedContent + rightText;
            }

            return parsedContent;
        }
	}
}
