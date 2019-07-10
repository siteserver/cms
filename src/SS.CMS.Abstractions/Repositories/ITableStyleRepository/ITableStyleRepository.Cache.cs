using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ITableStyleRepository
    {
        Task<List<KeyValuePair<string, TableStyle>>> GetAllTableStylesAsync();
    }
}