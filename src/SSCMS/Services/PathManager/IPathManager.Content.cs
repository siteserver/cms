using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface IPathManager
    {
        Task<Content> EncodeContentAsync(Site site, Channel channel, Content content);

        Task<Content> DecodeContentAsync(Site site, Channel channel, int contentId);

        Task<Content> DecodeContentAsync(Site site, Channel channel, Content content);

        Task<string> EncodeTextEditorAsync(Site site, string content);

        Task<string> DecodeTextEditorAsync(Site site, string content, bool isLocal);

        Task PutImagePathsAsync(Site site, Content content, NameValueCollection collection);
    }
}
