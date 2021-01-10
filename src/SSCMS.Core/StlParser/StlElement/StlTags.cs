using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Models;
using SSCMS.Parse;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "标签", Description = "通过 stl:tags 标签在模板中显示内容标签")]
    public static class StlTags
	{
        public const string ElementName = "stl:tags";

        [StlAttribute(Title = "标签级别")]
        private const string TagLevel = nameof(TagLevel);

        [StlAttribute(Title = "显示标签数目")]
        private const string TotalNum = nameof(TotalNum);

        [StlAttribute(Title = "是否按引用次数排序")]
        private const string IsOrderByCount = nameof(IsOrderByCount);

        [StlAttribute(Title = "所处上下文")]
        private const string Context = nameof(Context);

        public static async Task<object> ParseAsync(IParseManager parseManager)
		{
		    var tagLevel = 1;
            var totalNum = 0;
            var isOrderByCount = false;

		    foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, TagLevel))
                {
                    tagLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, TotalNum))
                {
                    totalNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsOrderByCount))
                {
                    isOrderByCount = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Context))
                {
                    parseManager.ContextInfo.ContextType = TranslateUtils.ToEnum(value, parseManager.ContextInfo.ContextType);
                }
            }

            return await ParseAsync(parseManager, tagLevel, totalNum, isOrderByCount);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, int tagLevel, int totalNum, bool isOrderByCount)
        {
            var innerHtml = StringUtils.StripTags(parseManager.ContextInfo.OuterHtml, ElementName);

            var tagsBuilder = new StringBuilder();

            if (parseManager.ContextInfo.ContextType == ParseType.Undefined)
            {
                parseManager.ContextInfo.ContextType = parseManager.ContextInfo.ContentId != 0 ? ParseType.Content : ParseType.Channel;
            }
            var contentId = 0;
            if (parseManager.ContextInfo.ContextType == ParseType.Content)
            {
                contentId = parseManager.ContextInfo.ContentId;
            }

            var tags =
                await parseManager.DatabaseManager.ContentTagRepository.GetTagsAsync(parseManager.PageInfo.SiteId);
            var tagInfoList = tags;
            if (contentId > 0)
            {
                tagInfoList = tags.Where(x => ListUtils.Contains(x.ContentIds, contentId)).ToList();
            }
            if (!isOrderByCount)
            {
                tagInfoList = tagInfoList.OrderBy(x => x.TagName).ToList();
            }

            if (totalNum > 0)
            {
                tagInfoList = tagInfoList.Take(totalNum).ToList();
            }
            tagInfoList = parseManager.DatabaseManager.ContentTagRepository.GetTagsByLevel(tagInfoList, totalNum, tagLevel);
            var contentInfo = parseManager.ContextInfo.Content;
            if (parseManager.ContextInfo.ContextType == ParseType.Content && contentInfo != null)
            {
                var tagInfoList2 = new List<ContentTag>();
                var tagNameList = new List<string>(contentInfo.TagNames);
                foreach (var tagName in tagNameList)
                {
                    if (!string.IsNullOrEmpty(tagName))
                    {
                        var isAdd = false;
                        foreach (var tagInfo in tagInfoList)
                        {
                            if (tagInfo.TagName == tagName)
                            {
                                isAdd = true;
                                tagInfoList2.Add(tagInfo);
                                break;
                            }
                        }
                        if (!isAdd)
                        {
                            var tagInfo = new ContentTag
                            {
                                Id = 0,
                                SiteId = parseManager.PageInfo.SiteId,
                                ContentIds = new List<int>{ contentId },
                                TagName = tagName,
                                UseNum = 1
                            };
                            tagInfoList2.Add(tagInfo);
                        }
                    }
                }
                tagInfoList = tagInfoList2;
            }

            foreach (var tagInfo in tagInfoList)
            {
                var tagHtml = innerHtml;
                tagHtml = StringUtils.ReplaceIgnoreCase(tagHtml, "{Tag.Name}", tagInfo.TagName);
                tagHtml = StringUtils.ReplaceIgnoreCase(tagHtml, "{Tag.Count}", tagInfo.UseNum.ToString());
                tagHtml = StringUtils.ReplaceIgnoreCase(tagHtml, "{Tag.Level}", tagInfo.Level.ToString());
                var innerBuilder = new StringBuilder(tagHtml);
                await parseManager.ParseInnerContentAsync(innerBuilder);
                tagsBuilder.Append(innerBuilder);
            }

            return tagsBuilder.ToString();
        }
    }
}
