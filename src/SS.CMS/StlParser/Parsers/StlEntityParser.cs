using System;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Abstractions.Parse;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.StlEntity;
using SS.CMS.StlParser.Utility;

namespace SS.CMS.StlParser.Parsers
{
    /// <summary>
    /// Stl实体解析器
    /// </summary>
	public static class StlEntityParser
	{
        public const string RegexStringAll = @"{stl\.[^{}]*}|{stl:[^{}]*}|{content\.[^{}]*}|{channel\.[^{}]*}|{comment\.[^{}]*}|{request\.[^{}]*}|{sql\.[^{}]*}|{user\.[^{}]*}|{navigation\.[^{}]*}|{photo\.[^{}]*}";

        /// <summary>
        /// 将原始内容中的STL实体替换为实际内容
        /// </summary>
        public static async Task ReplaceStlEntitiesAsync(StringBuilder parsedBuilder, ParsePage pageInfo, ParseContext contextInfo)
        {
            var stlEntityList = StlParserUtility.GetStlEntityList(parsedBuilder.ToString());

            foreach (var stlEntity in stlEntityList)
            {
                var startIndex = parsedBuilder.ToString().IndexOf(stlEntity, StringComparison.Ordinal);
                if (startIndex == -1) continue;

                var resultContent = await ParseStlEntityAsync(stlEntity, pageInfo, contextInfo);
                parsedBuilder.Replace(stlEntity, resultContent, startIndex, stlEntity.Length);
            }
        }

        //public static string ReplaceStlUserEntitiesByApiControllers(string content)
        //{
        //    var parsedBuilder = new StringBuilder(content);
        //    var stlEntityList = StlParserUtility.GetStlUserEntityList(parsedBuilder.ToString());

        //    var pageInfo = new PageInfo();

        //    foreach (var stlEntity in stlEntityList)
        //    {
        //        var startIndex = parsedBuilder.ToString().IndexOf(stlEntity, StringComparison.Ordinal);
        //        if (startIndex == -1) continue;
        //        var entityType = EStlEntityTypeUtils.GetEntityType(stlEntity);
        //        if (entityType != EStlEntityType.User) continue;
        //        var resultContent = StlUserEntities.Parse(stlEntity, null);
        //        parsedBuilder.Replace(stlEntity, resultContent, startIndex, stlEntity.DataLength);
        //    }
        //    return parsedBuilder.ToString();
        //}

        internal static async Task<string> ParseStlEntityAsync(string stlEntity, ParsePage pageInfo, ParseContext contextInfo)
        {
            var parsedContent = string.Empty;

            var entityType = GetEntityType(stlEntity);

            if (entityType == StlEntityType.Stl)
            {
                parsedContent = await StlStlEntities.ParseAsync(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == StlEntityType.StlElement)
            {
                parsedContent = await StlElementEntities.ParseAsync(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == StlEntityType.Content)
            {
                parsedContent = await StlContentEntities.ParseAsync(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == StlEntityType.Channel)
            {
                parsedContent = await StlChannelEntities.ParseAsync(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == StlEntityType.Request)
            {
                parsedContent = await StlRequestEntities.ParseAsync(stlEntity, pageInfo);
            }
            else if (entityType == StlEntityType.Navigation)
            {
                parsedContent = await StlNavigationEntities.ParseAsync(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == StlEntityType.Sql)
            {
                parsedContent = StlSqlEntities.Parse(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == StlEntityType.User)
            {
                parsedContent = StlUserEntities.Parse(stlEntity, pageInfo);
            }

            return parsedContent;
        }

        internal static async Task<string> ReplaceStlEntitiesForAttributeValueAsync(string attrValue, ParsePage pageInfo, ParseContext contextInfo)
        {
            if (!StlParserUtility.IsStlEntityInclude(attrValue)) return attrValue;

            var contentBuilder = new StringBuilder(attrValue);
            await ReplaceStlEntitiesAsync(contentBuilder, pageInfo, contextInfo);
            return contentBuilder.ToString();
        }

        public static StlEntityType GetEntityType(string stlEntity)
        {
            var type = StlEntityType.Undefined;
            if (!string.IsNullOrEmpty(stlEntity))
            {
                stlEntity = stlEntity.Trim().ToLower();

                if (stlEntity.StartsWith("{stl."))
                {
                    return StlEntityType.Stl;
                }
                if (stlEntity.StartsWith("{stl:"))
                {
                    return StlEntityType.StlElement;
                }
                if (stlEntity.StartsWith("{content."))
                {
                    return StlEntityType.Content;
                }
                if (stlEntity.StartsWith("{channel."))
                {
                    return StlEntityType.Channel;
                }
                if (stlEntity.StartsWith("{request."))
                {
                    return StlEntityType.Request;
                }
                if (stlEntity.StartsWith("{navigation."))
                {
                    return StlEntityType.Navigation;
                }
                if (stlEntity.StartsWith("{sql."))
                {
                    return StlEntityType.Sql;
                }
                if (stlEntity.StartsWith("{user."))
                {
                    return StlEntityType.User;
                }
            }
            return type;
        }
    }
}
