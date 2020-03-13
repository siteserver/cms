using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IPathManager
    {
        Task<string> TextEditorContentEncodeAsync(Site site, string content);

        Task<string> TextEditorContentDecodeAsync(Site site, string content, bool isLocal);

        Task PutImagePathsAsync(Site site, Content content, NameValueCollection collection);
    }
}
