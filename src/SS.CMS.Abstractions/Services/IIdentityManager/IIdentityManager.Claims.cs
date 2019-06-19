using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Services
{
    public partial interface IIdentityManager
    {
        Task SignInAsync(AdministratorInfo administratorInfo, bool isPersistent = false);

        Task SignOutAsync();
    }
}
