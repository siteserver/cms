using System;
using System.Text;
using BaiRong.Core.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlEntity;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.Parser
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
        public static void ReplaceStlEntities(StringBuilder parsedBuilder, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var stlEntityList = StlParserUtility.GetStlEntityList(parsedBuilder.ToString());

            foreach (var stlEntity in stlEntityList)
            {
                var startIndex = parsedBuilder.ToString().IndexOf(stlEntity, StringComparison.Ordinal);
                if (startIndex == -1) continue;

                var resultContent = ParseStlEntity(stlEntity, pageInfo, contextInfo);
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
        //        parsedBuilder.Replace(stlEntity, resultContent, startIndex, stlEntity.Length);
        //    }
        //    return parsedBuilder.ToString();
        //}

        internal static string ParseStlEntity(string stlEntity, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;

            var entityType = EStlEntityTypeUtils.GetEntityType(stlEntity);

            if (entityType == EStlEntityType.Stl)
            {
                parsedContent = StlStlEntities.Parse(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == EStlEntityType.StlElement)
            {
                parsedContent = StlElementEntities.Parse(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == EStlEntityType.Content)
            {
                parsedContent = StlContentEntities.Parse(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == EStlEntityType.Channel)
            {
                parsedContent = StlChannelEntities.Parse(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == EStlEntityType.Photo)
            {
                parsedContent = StlPhotoEntities.Parse(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == EStlEntityType.Comment)
            {
                parsedContent = StlCommentEntities.Parse(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == EStlEntityType.Request)
            {
                parsedContent = StlRequestEntities.Parse(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == EStlEntityType.Navigation)
            {
                parsedContent = StlNavigationEntities.Parse(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == EStlEntityType.Sql)
            {
                parsedContent = StlSqlEntities.Parse(stlEntity, pageInfo, contextInfo);
            }
            else if (entityType == EStlEntityType.User)
            {
                parsedContent = StlUserEntities.Parse(stlEntity, pageInfo);
            }

            return parsedContent;
        }

        internal static string ReplaceStlEntitiesForAttributeValue(string attrValue, PageInfo pageInfo, ContextInfo contextInfo)
        {
            if (!StlParserUtility.IsStlEntityInclude(attrValue)) return attrValue;

            var contentBuilder = new StringBuilder(attrValue);
            ReplaceStlEntities(contentBuilder, pageInfo, contextInfo);
            return contentBuilder.ToString();
        }
	}
}
