using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMS.Services
{
    public partial interface ICacheManager<TCacheValue>
    {
        void Remove(string key);
        void Clear();
    }
}
