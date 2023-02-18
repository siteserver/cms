using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Services
{
    public partial interface IFormManager
    {   
        Task SendNotifyAsync(Form form, List<TableStyle> styles, FormData data);
    }
}
