using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "文件下载链接", Description = "通过 stl:file 标签在模板中显示文件下载链接")]
    public static class StlFile
	{
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

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var type = nameof(Content.FileUrl);
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

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

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

            return await ParseAsync(parseManager, type, no, src, isFileName, isFileType, isFileSize, isCount, isLower, isUpper, leftText, rightText, attributes);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, string type, int no, string src, bool isFileName, bool isFileType, bool isFileSize, bool isCount, bool isLower, bool isUpper, string leftText, string rightText, NameValueCollection attributes)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            if (!string.IsNullOrEmpty(contextInfo.InnerHtml))
            {
                var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                await parseManager.ParseInnerContentAsync(innerBuilder);
                contextInfo.InnerHtml = innerBuilder.ToString();
            }

            var contentInfo = await parseManager.GetContentAsync();

            var fileUrl = string.Empty;
            if (!string.IsNullOrEmpty(src))
            {
                fileUrl = src;
            }
            else
            {
                if (contextInfo.ContextType == ParseType.Undefined)
                {
                    contextInfo.ContextType = ParseType.Content;
                }
                if (contextInfo.ContextType == ParseType.Content)
                {
                    if (contextInfo.ContentId != 0)
                    {
                        if (!string.IsNullOrEmpty(contentInfo?.Get<string>(type)))
                        {
                            if (no <= 1)
                            {
                                fileUrl = contentInfo.Get<string>(type);
                            }
                            else
                            {
                                var extendName = ColumnsManager.GetExtendName(type, no - 1);
                                fileUrl = contentInfo.Get<string>(extendName);
                            }
                        }
                    }
                }
                else if (contextInfo.ContextType == ParseType.Each)
                {
                    fileUrl = contextInfo.ItemContainer.EachItem.Value as string;
                }
            }

            string parsedContent;

            if (isFileName)
            {
                parsedContent = PathUtils.RemoveExtension(PageUtils.GetFileNameFromUrl(fileUrl));
                if (isLower)
                {
                    parsedContent = StringUtils.ToLower(parsedContent);
                }
                if (isUpper)
                {
                    parsedContent = StringUtils.ToUpper(parsedContent);
                }
            }
            else if (isFileType)
            {
                var filePath = await parseManager.PathManager.ParseSitePathAsync(pageInfo.Site, fileUrl);
                parsedContent = PathUtils.GetExtension(filePath).Trim('.');
                if (isLower)
                {
                    parsedContent = StringUtils.ToLower(parsedContent);
                }
                if (isUpper)
                {
                    parsedContent = StringUtils.ToUpper(parsedContent);
                }
            }
            else if (isFileSize)
            {
                var filePath = await parseManager.PathManager.ParseSitePathAsync(pageInfo.Site, fileUrl);
                parsedContent = FileUtils.GetFileSizeByFilePath(filePath);
            }
            else if (isCount)
            {
                parsedContent = (contentInfo?.Downloads ?? 0).ToString();
            }
            else
            {
                var inputParser = new InputParserManager(parseManager.PathManager);

                parsedContent = contentInfo != null
                    ? inputParser.GetFileHtmlWithCount(pageInfo.Site, contentInfo.ChannelId,
                        contentInfo.Id, fileUrl, attributes, contextInfo.InnerHtml,
                        contextInfo.IsStlEntity, isLower, isUpper)
                    : inputParser.GetFileHtmlWithoutCount(pageInfo.Site, fileUrl, attributes,
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
