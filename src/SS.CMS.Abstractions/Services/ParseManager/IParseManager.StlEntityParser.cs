using System;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Abstractions.Parse;

namespace SS.CMS.Abstractions
{
    public partial interface IParseManager
    {
        Task ReplaceStlEntitiesAsync(StringBuilder parsedBuilder);

        Task<string> ParseStlEntityAsync(string stlEntity);

        Task<string> ReplaceStlEntitiesForAttributeValueAsync(string attrValue);

        StlEntityType GetEntityType(string stlEntity);
    }
}
