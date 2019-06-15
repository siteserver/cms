using System;
using System.Text;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.StlEntity;
using SS.CMS.Core.StlParser.Utility;

namespace SS.CMS.Core.StlParser
{
    /// <summary>
    /// Stl实体解析器
    /// </summary>
    public partial class ParseContext
    {
        /// <summary>
        /// 将原始内容中的STL实体替换为实际内容
        /// </summary>
        public void ReplaceStlEntities(StringBuilder parsedBuilder)
        {
            var stlEntityList = StlParserUtility.GetStlEntityList(parsedBuilder.ToString());

            foreach (var stlEntity in stlEntityList)
            {
                var startIndex = parsedBuilder.ToString().IndexOf(stlEntity, StringComparison.Ordinal);
                if (startIndex == -1) continue;

                var resultContent = ParseStlEntity(stlEntity);
                parsedBuilder.Replace(stlEntity, resultContent, startIndex, stlEntity.Length);
            }
        }

        internal string ParseStlEntity(string stlEntity)
        {
            var parsedContent = string.Empty;

            var entityType = EStlEntityTypeUtils.GetEntityType(stlEntity);

            if (entityType == EStlEntityType.Stl)
            {
                parsedContent = StlStlEntities.Parse(stlEntity, this);
            }
            else if (entityType == EStlEntityType.StlElement)
            {
                parsedContent = StlElementEntities.Parse(stlEntity, this);
            }
            else if (entityType == EStlEntityType.Content)
            {
                parsedContent = StlContentEntities.Parse(stlEntity, this);
            }
            else if (entityType == EStlEntityType.Channel)
            {
                parsedContent = StlChannelEntities.Parse(stlEntity, this);
            }
            else if (entityType == EStlEntityType.Request)
            {
                parsedContent = StlRequestEntities.Parse(stlEntity, this);
            }
            else if (entityType == EStlEntityType.Navigation)
            {
                parsedContent = StlNavigationEntities.Parse(stlEntity, this);
            }
            else if (entityType == EStlEntityType.Sql)
            {
                parsedContent = StlSqlEntities.Parse(stlEntity, this);
            }
            else if (entityType == EStlEntityType.User)
            {
                parsedContent = StlUserEntities.Parse(stlEntity, this, UserRepository);
            }

            return parsedContent;
        }

        internal string ReplaceStlEntitiesForAttributeValue(string attrValue)
        {
            if (!StlParserUtility.IsStlEntityInclude(attrValue)) return attrValue;

            var contentBuilder = new StringBuilder(attrValue);
            ReplaceStlEntities(contentBuilder);
            return contentBuilder.ToString();
        }
    }
}
