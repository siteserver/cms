using System.Collections.Specialized;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Configuration;
using SSCMS.Utils;
using System.Collections.Generic;

namespace SSCMS.Services
{
    public partial interface IPathManager
    {
        Task<Content> EncodeContentAsync(Site site, Channel channel, Content content);

        Task<Content> DecodeContentAsync(Site site, Channel channel, int contentId);

        Task<Content> DecodeContentAsync(Site site, Channel channel, Content content);

        Task<string> EncodeTextEditorAsync(Site site, string content);

        Task<string> DecodeTextEditorAsync(Site site, string content, bool isLocal);

        Task PutFilePathsAsync(Site site, Content content, NameValueCollection collection, List<TableStyle> tableStyles);
    }
}
