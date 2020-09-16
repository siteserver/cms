using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMS.Services
{
    public partial interface ICacheManager<TCacheValue>
    {
        TCacheValue Get(string key);

        bool Exists(string key);
    }
}
