using System.Collections.Generic;

namespace SSCMS.Services
{
    public partial interface IAuthManager
    {
        bool IsMenuValid(Menu menu, IList<string> permissions);
    }
}
