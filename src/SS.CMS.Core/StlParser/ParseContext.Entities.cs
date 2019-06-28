using System;
using System.Text;
using System.Threading.Tasks;
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
        public async Task ReplaceStlEntitiesAsync(StringBuilder parsedBuilder)
        {
            var stlEntityList = StlParserUtility.GetStlEntityList(parsedBuilder.ToString());

            foreach (var stlEntity in stlEntityList)
            {
                var startIndex = parsedBuilder.ToString().IndexOf(stlEntity, StringComparison.Ordinal);
                if (startIndex == -1) continue;

                var resultContent = await ParseStlEntityAsync(stlEntity);
                if (resultContent != null)
                {
                    parsedBuilder.Replace(stlEntity, resultContent.ToString(), startIndex, stlEntity.Length);
                }
            }
        }

        internal async Task<string> ParseStlEntityAsync(string stlEntity)
        {
            var parsedContent = string.Empty;

            var entityType = EStlEntityTypeUtils.GetEntityType(stlEntity);

            if (entityType == EStlEntityType.Stl)
            {
                parsedContent = await StlStlEntities.ParseAsync(stlEntity, this);
            }
            else if (entityType == EStlEntityType.StlElement)
            {
                parsedContent = await StlElementEntities.ParseAsync(stlEntity, this);
            }
            else if (entityType == EStlEntityType.Content)
            {
                parsedContent = await StlContentEntities.ParseAsync(stlEntity, this);
            }
            else if (entityType == EStlEntityType.Channel)
            {
                parsedContent = await StlChannelEntities.ParseAsync(stlEntity, this);
            }
            else if (entityType == EStlEntityType.Request)
            {
                parsedContent = StlRequestEntities.Parse(stlEntity, this);
            }
            else if (entityType == EStlEntityType.Navigation)
            {
                parsedContent = await StlNavigationEntities.ParseAsync(stlEntity, this);
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

        internal async Task<string> ReplaceStlEntitiesForAttributeValueAsync(string attrValue)
        {
            if (!StlParserUtility.IsStlEntityInclude(attrValue)) return attrValue;

            var contentBuilder = new StringBuilder(attrValue);
            await ReplaceStlEntitiesAsync(contentBuilder);
            return contentBuilder.ToString();
        }
    }
}
