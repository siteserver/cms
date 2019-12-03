using System.Collections;
using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IContentTagRepository
    {
        Task<SortedList> ReadContentAsync(int siteId);
    }
}