using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSCMS.Abstractions
{
    public partial interface IParseManager
    {
        Task ReplaceStlElementsAsync(StringBuilder parsedBuilder);

        Dictionary<string, Func<IParseManager, Task<object>>> ElementsToParseDic { get; }
    }
}
