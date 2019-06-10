using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace SS.CMS.Abstractions
{
    public partial interface IRequest
    {
        IList<string> QueryKeys { get; }

        bool IsQueryExists(string name);

        string GetQueryString(string name);

        int GetQueryInt(string name, int defaultValue = 0);

        decimal GetQueryDecimal(string name, decimal defaultValue = 0);

        bool GetQueryBool(string name, bool defaultValue = false);

        bool TryGetQuery(string name, out StringValues value);
    }
}
