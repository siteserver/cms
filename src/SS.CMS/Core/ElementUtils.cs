using System.Collections.Generic;
using System.Linq;
using SS.CMS.Abstractions.Dto;

namespace SS.CMS.Core
{
    public static class ElementUtils
    {
        public static IEnumerable<Select<T>> GetSelects<T>(IEnumerable<KeyValuePair<T, string>> pairs)
        {
            return pairs.Select(x => new Select<T>(x.Key, x.Value));
        }

        public static IEnumerable<CheckBox<T>> GetCheckBoxes<T>(IEnumerable<KeyValuePair<T, string>> pairs)
        {
            return pairs.Select(x => new CheckBox<T>(x.Key, x.Value));
        }
    }
}
