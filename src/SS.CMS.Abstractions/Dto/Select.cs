using System;
using Datory;

namespace SS.CMS.Abstractions.Dto
{
    [Serializable]
    public class Select<T>
    {
        public Select(T value, string label)
        {
            Value = value;
            Label = label;
        }

        public Select(Enum e)
        {
            Value = TranslateUtils.Get<T>(e.GetValue());
            Label = e.GetDisplayName();
        }

        public T Value { get; set; }
        public string Label { get; set; }
    }
}
