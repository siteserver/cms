using System.Threading.Tasks;
using Datory;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "获取素材", Description = "通过 stl:material 标签在模板中获取素材")]
    public static class StlMaterial
    {
        public const string ElementName = "stl:material";

        public const string EditorPlaceHolder1 = @"src=""/sitefiles/assets/images/material-clip.png""";
        public const string EditorPlaceHolder2 = @"src=""@sitefiles/assets/images/material-clip.png""";

        [StlAttribute(Title = "类型")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "素材标题")]
        private const string Name = nameof(Name);

        [StlAttribute(Title = "素材Id")]
        private const string Id = nameof(Id);

        [StlAttribute(Title = "字符开始位置")]
        private const string StartIndex = nameof(StartIndex);

        [StlAttribute(Title = "指定字符长度")]
        private const string Length = nameof(Length);

        [StlAttribute(Title = "显示字符的数目")]
        private const string WordNum = nameof(WordNum);

        [StlAttribute(Title = "文字超出部分显示的文字")]
        private const string Ellipsis = nameof(Ellipsis);

        [StlAttribute(Title = "需要替换的文字，可以是正则表达式")]
        private const string Replace = nameof(Replace);

        [StlAttribute(Title = "替换replace的文字信息")]
        private const string To = nameof(To);

        [StlAttribute(Title = "是否清除标签信息")]
        private const string IsClearTags = nameof(IsClearTags);

        [StlAttribute(Title = "是否将回车替换为HTML换行标签")]
        private const string IsReturnToBr = nameof(IsReturnToBr);

        [StlAttribute(Title = "是否转换为小写")]
        private const string IsLower = nameof(IsLower);

        [StlAttribute(Title = "是否转换为大写")]
        private const string IsUpper = nameof(IsUpper);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var type = string.Empty;
            var name = string.Empty;
            var id = 0;
            var startIndex = 0;
            var length = 0;
            var wordNum = 0;
            var ellipsis = Constants.Ellipsis;
            var replace = string.Empty;
            var to = string.Empty;
            var isClearTags = false;
            var isReturnToBr = false;
            var isLower = false;
            var isUpper = false;

            foreach (var attrName in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[attrName];

                if (StringUtils.EqualsIgnoreCase(attrName, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(attrName, Name))
                {
                    name = value;
                }
                else if (StringUtils.EqualsIgnoreCase(attrName, Id))
                {
                    id = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(attrName, StartIndex))
                {
                    startIndex = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(attrName, Length))
                {
                    length = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(attrName, WordNum))
                {
                    wordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(attrName, Ellipsis))
                {
                    ellipsis = value;
                }
                else if (StringUtils.EqualsIgnoreCase(attrName, Replace))
                {
                    replace = value;
                }
                else if (StringUtils.EqualsIgnoreCase(attrName, To))
                {
                    to = value;
                }
                else if (StringUtils.EqualsIgnoreCase(attrName, IsClearTags))
                {
                    isClearTags = TranslateUtils.ToBool(value, false);
                }
                else if (StringUtils.EqualsIgnoreCase(attrName, IsReturnToBr))
                {
                    isReturnToBr = TranslateUtils.ToBool(value, false);
                }
                else if (StringUtils.EqualsIgnoreCase(attrName, IsLower))
                {
                    isLower = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(attrName, IsUpper))
                {
                    isUpper = TranslateUtils.ToBool(value, true);
                }
            }

            return await ParseAsync(parseManager, type, name, id, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, string type, string name, int id, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper)
        {
            if (string.IsNullOrEmpty(type)) return string.Empty;

            var parsedContent = string.Empty;

            if (StringUtils.EqualsIgnoreCase(type, MaterialType.Article.GetValue()))
            {
                if (id > 0)
                {
                    parsedContent = await parseManager.DatabaseManager.MaterialArticleRepository.GetBodyByIdAsync(id);
                }
                else if (!string.IsNullOrEmpty(name))
                {
                    parsedContent = await parseManager.DatabaseManager.MaterialArticleRepository.GetBodyByTitleAsync(name);
                }
            }
            else if (StringUtils.EqualsIgnoreCase(type, MaterialType.Image.GetValue()))
            {
                if (id > 0)
                {
                    parsedContent = await parseManager.DatabaseManager.MaterialImageRepository.GetUrlByIdAsync(id);
                }
                else if (!string.IsNullOrEmpty(name))
                {
                    parsedContent = await parseManager.DatabaseManager.MaterialImageRepository.GetUrlByTitleAsync(name);
                }
            }
            else if (StringUtils.EqualsIgnoreCase(type, MaterialType.Audio.GetValue()))
            {
                if (id > 0)
                {
                    parsedContent = await parseManager.DatabaseManager.MaterialAudioRepository.GetUrlByIdAsync(id);
                }
                else if (!string.IsNullOrEmpty(name))
                {
                    parsedContent = await parseManager.DatabaseManager.MaterialAudioRepository.GetUrlByTitleAsync(name);
                }
            }
            else if (StringUtils.EqualsIgnoreCase(type, MaterialType.Video.GetValue()))
            {
                if (id > 0)
                {
                    parsedContent = await parseManager.DatabaseManager.MaterialVideoRepository.GetUrlByIdAsync(id);
                }
                else if (!string.IsNullOrEmpty(name))
                {
                    parsedContent = await parseManager.DatabaseManager.MaterialVideoRepository.GetUrlByTitleAsync(name);
                }
            }
            else if (StringUtils.EqualsIgnoreCase(type, MaterialType.File.GetValue()))
            {
                if (id > 0)
                {
                    parsedContent = await parseManager.DatabaseManager.MaterialFileRepository.GetUrlByIdAsync(id);
                }
                else if (!string.IsNullOrEmpty(name))
                {
                    parsedContent = await parseManager.DatabaseManager.MaterialFileRepository.GetUrlByTitleAsync(name);
                }
            }

            parsedContent = InputTypeUtils.ParseString(InputType.Text, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, string.Empty);

            return parsedContent;
        }
    }
}
