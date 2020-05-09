using System.Collections.Generic;
using SSCMS.Configuration;

namespace SSCMS.Services
{
    public partial interface IAuthManager
    {
        bool IsMenuValid(Menu menu, IList<string> permissions);
    }
}
