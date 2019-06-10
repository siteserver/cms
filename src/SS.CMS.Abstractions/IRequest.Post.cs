using System.Collections.Generic;

namespace SS.CMS.Abstractions
{
    public partial interface IRequest
    {
        IList<string> PostKeys { get; }

        bool IsPostExists(string name);

        string GetPostString(string name);

        int GetPostInt(string name, int defaultValue = 0);

        decimal GetPostDecimal(string name, decimal defaultValue = 0);

        bool GetPostBool(string name, bool defaultValue = false);

        bool TryGetPost<T>(string name, out T value);

        bool TryGetPost<T>(out T value);
    }
}
