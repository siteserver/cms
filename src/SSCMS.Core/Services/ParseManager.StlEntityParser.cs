using System;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.StlEntity;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Parse;

namespace SSCMS.Core.Services
{
    public partial class ParseManager
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
                parsedBuilder.Replace(stlEntity, resultContent, startIndex, stlEntity.Length);
            }
        }

        public async Task<string> ParseStlEntityAsync(string stlEntity)
        {
            var parsedContent = string.Empty;

            var entityType = GetEntityType(stlEntity);

            if (entityType == StlEntityType.Stl)
            {
                parsedContent = await StlStlEntities.ParseAsync(stlEntity, this);
            }
            else if (entityType == StlEntityType.StlElement)
            {
                parsedContent = await StlElementEntities.ParseAsync(stlEntity, this);
            }
            else if (entityType == StlEntityType.Content)
            {
                parsedContent = await StlContentEntities.ParseAsync(stlEntity, this);
            }
            else if (entityType == StlEntityType.Channel)
            {
                parsedContent = await StlChannelEntities.ParseAsync(stlEntity, this);
            }
            else if (entityType == StlEntityType.Request)
            {
                parsedContent = await StlRequestEntities.ParseAsync(stlEntity, this);
            }
            else if (entityType == StlEntityType.Navigation)
            {
                parsedContent = await StlNavigationEntities.ParseAsync(stlEntity, this);
            }
            else if (entityType == StlEntityType.Sql)
            {
                parsedContent = StlSqlEntities.Parse(stlEntity, this);
            }
            else if (entityType == StlEntityType.User)
            {
                parsedContent = StlUserEntities.Parse(stlEntity, this);
            }

            return parsedContent;
        }

        public async Task<string> ReplaceStlEntitiesForAttributeValueAsync(string attrValue)
        {
            if (!StlParserUtility.IsStlEntityInclude(attrValue)) return attrValue;

            var contentBuilder = new StringBuilder(attrValue);
            await ReplaceStlEntitiesAsync(contentBuilder);
            return contentBuilder.ToString();
        }

        public StlEntityType GetEntityType(string stlEntity)
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
