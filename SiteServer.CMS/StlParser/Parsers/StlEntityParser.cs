using System;
using System.Text;
using System.Threading.Tasks;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlEntity;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.Parsers
{
    /// <summary>
    /// Stl实体解析器
    /// </summary>
	public class StlEntityParser
	{
		private StlEntityParser()
		{
		}

        /// <summary>
        /// 将原始内容中的STL实体替换为实际内容
        /// </summary>
        public static async Task ReplaceStlEntitiesAsync(StringBuilder parsedBuilder, PageInfo pageInfo, ContextInfo contextInfo)
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

        internal static async Task<string> ParseStlEntityAsync(string stlEntity, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;

            var entityType = EStlEntityTypeUtils.GetEntityType(stlEntity);

            if (entityType == EStlEntityType.Stl)
            {
                parsedContent = await StlStlEntities.ParseAsync(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == EStlEntityType.StlElement)
            {
                parsedContent = await StlElementEntities.ParseAsync(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == EStlEntityType.Content)
            {
                parsedContent = await StlContentEntities.ParseAsync(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == EStlEntityType.Channel)
            {
                parsedContent = await StlChannelEntities.ParseAsync(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == EStlEntityType.Request)
            {
                parsedContent = await StlRequestEntities.ParseAsync(stlEntity, pageInfo);
            }
            else if (entityType == EStlEntityType.Navigation)
            {
                parsedContent = await StlNavigationEntities.ParseAsync(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == EStlEntityType.Sql)
            {
                parsedContent = await StlSqlEntities.ParseAsync(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == EStlEntityType.User)
            {
                parsedContent = await StlUserEntities.ParseAsync(stlEntity, pageInfo);
            }

            return parsedContent;
        }

        internal static async Task<string> ReplaceStlEntitiesForAttributeValueAsync(string attrValue, PageInfo pageInfo, ContextInfo contextInfo)
        {
            if (!StlParserUtility.IsStlEntityInclude(attrValue)) return attrValue;

            var contentBuilder = new StringBuilder(attrValue);
            await ReplaceStlEntitiesAsync(contentBuilder, pageInfo, contextInfo);
            return contentBuilder.ToString();
        }
	}
}
