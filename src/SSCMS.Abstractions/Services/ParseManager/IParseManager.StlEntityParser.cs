using System.Text;
using System.Threading.Tasks;
using SSCMS.Abstractions.Parse;

namespace SSCMS.Abstractions
{
    public partial interface IParseManager
    {
        Task ReplaceStlEntitiesAsync(StringBuilder parsedBuilder);

        Task<string> ParseStlEntityAsync(string stlEntity);

        Task<string> ReplaceStlEntitiesForAttributeValueAsync(string attrValue);

        StlEntityType GetEntityType(string stlEntity);
    }
}
