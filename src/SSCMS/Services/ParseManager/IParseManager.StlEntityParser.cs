using System.Text;
using System.Threading.Tasks;
using SSCMS.Parse;

namespace SSCMS
{
    public partial interface IParseManager
    {
        Task ReplaceStlEntitiesAsync(StringBuilder parsedBuilder);

        Task<string> ParseStlEntityAsync(string stlEntity);

        Task<string> ReplaceStlEntitiesForAttributeValueAsync(string attrValue);

        StlEntityType GetEntityType(string stlEntity);
    }
}
