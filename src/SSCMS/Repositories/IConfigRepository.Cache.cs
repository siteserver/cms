using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IConfigRepository
    {
        Task<Config> GetAsync();
    }
}
