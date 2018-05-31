using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "文件下载链接", Description = "通过 stl:file 标签在模板中显示文件下载链接")]
    public class StlFile
	{
	    private StlFile() { }

	    public const string ElementName = "stl:file";

        private static readonly Attr No = new Attr("no", "显示字段的顺序");
		private static readonly Attr Src = new Attr("src", "需要下载的文件地址");
        private static readonly Attr IsFileSize = new Attr("isFileSize", "显示文件大小");
        private static readonly Attr IsCount = new Attr("isCount", "是否记录文件下载次数");
        private static readonly Attr Type = new Attr("type", "指定存储附件的字段");
        private static readonly Attr LeftText = new Attr("leftText", "显示在信息前的文字");
        private static readonly Attr RightText = new Attr("rightText", "显示在信息后的文字");

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var no = 0;
            var src = string.Empty;
            var isFilesize = false;
            var isCount = true;
            var type = BackgroundContentAttribute.FileUrl;
            var leftText = string.Empty;
            var rightText = string.Empty;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, No.Name))
                {
                    no = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Src.Name))
                {
                    src = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsFileSize.Name))
                {
                    isFilesize = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsCount.Name))
                {
                    isCount = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Type.Name))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, LeftText.Name))
                {
                    leftText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, RightText.Name))
                {
                    rightText = value;
                }
            }

            return ParseImpl(pageInfo, contextInfo, type, no, src, isFilesize, isCount, leftText, rightText);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string type, int no, string src, bool isFilesize, bool isCount, string leftText, string rightText)
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

            var parsedContent = InputParserUtility.GetFileHtmlWithoutCount(pageInfo.SiteInfo, fileUrl, contextInfo.Attributes, contextInfo.InnerHtml, contextInfo.IsStlEntity);

            if (isFilesize)
            {
                var filePath = PathUtility.MapPath(pageInfo.SiteInfo, fileUrl);
                parsedContent += " (" + FileUtils.GetFileSizeByFilePath(filePath) + ")";
            }
            else
            {
                if (isCount && contextInfo.ContentInfo != null)
                {
                    parsedContent = InputParserUtility.GetFileHtmlWithCount(pageInfo.SiteInfo, contextInfo.ContentInfo.ChannelId, contextInfo.ContentInfo.Id, fileUrl, contextInfo.Attributes, contextInfo.InnerHtml, contextInfo.IsStlEntity);
                }
                else
                {
                    parsedContent = InputParserUtility.GetFileHtmlWithoutCount(pageInfo.SiteInfo, fileUrl, contextInfo.Attributes, contextInfo.InnerHtml, contextInfo.IsStlEntity);
                }                
            }

            if (!string.IsNullOrEmpty(parsedContent))
            {
                parsedContent = leftText + parsedContent + rightText;
            }

            return parsedContent;
        }
	}
}
