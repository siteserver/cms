using System;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Parse;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class ParseManager
    {
        /// <summary>
        /// 将原始内容中的STL实体替换为实际内容
        /// </summary>
        public async Task ReplaceStlEntitiesAsync(StringBuilder parsedBuilder)
        {
            var stlEntityList = ParseUtils.GetStlEntities(parsedBuilder.ToString());

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
            var entityType = GetEntityType(stlEntity);

            if (entityType == StlEntityType.Stl)
            {
                stlEntity = StringUtils.ReplaceStartsWithIgnoreCase(stlEntity, "{stl.", "{stl:value type=");
                //parsedContent = await StlStlEntities.ParseAsync(stlEntity, this);
            }
            //else if (entityType == StlEntityType.StlElement)
            //{
            //    parsedContent = await StlElementEntities.ParseAsync(stlEntity, this);
            //}
            else if (entityType == StlEntityType.Content)
            {
                stlEntity = StringUtils.ReplaceStartsWithIgnoreCase(stlEntity, "{content.", "{stl:content type=");
                //parsedContent = await StlContentEntities.ParseAsync(stlEntity, this);
            }
            else if (entityType == StlEntityType.Channel)
            {
                stlEntity = StringUtils.ReplaceStartsWithIgnoreCase(stlEntity, "{channel.", "{stl:channel type=");
                //parsedContent = await StlChannelEntities.ParseAsync(stlEntity, this);
            }
            else if (entityType == StlEntityType.Request)
            {
                stlEntity = StringUtils.ReplaceStartsWithIgnoreCase(stlEntity, "{request.", "{stl:request type=");
                //parsedContent = await StlRequestEntities.ParseAsync(stlEntity, this);
            }
            else if (entityType == StlEntityType.Navigation)
            {
                stlEntity = StringUtils.ReplaceStartsWithIgnoreCase(stlEntity, "{navigation.", "{stl:navigation type=");
                //parsedContent = await StlNavigationEntities.ParseAsync(stlEntity, this);
            }
            else if (entityType == StlEntityType.Sql)
            {
                stlEntity = StringUtils.ReplaceStartsWithIgnoreCase(stlEntity, "{sql.", "{stl:sqlContent type=");
                //parsedContent = StlSqlEntities.Parse(stlEntity, this);
            }
            else if (entityType == StlEntityType.User)
            {
                stlEntity = StringUtils.ReplaceStartsWithIgnoreCase(stlEntity, "{user.", "{stl:user type=");
                //parsedContent = StlUserEntities.Parse(stlEntity, this);
            }

            return await StlElementEntities.ParseAsync(stlEntity, this);
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
                stlEntity = StringUtils.ToLower(stlEntity).Trim();

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
