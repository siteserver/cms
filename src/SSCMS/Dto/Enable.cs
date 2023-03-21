using System;
using Datory;
using SSCMS.Utils;

namespace SSCMS.Dto
{
    [Serializable]
    public class Enable<T>
    {
        public Enable()
        {

        }

        public Enable(T value, string label)
        {
            Value = value;
            Label = label;
        }

        public Enable(Enum e)
        {
            Value = TranslateUtils.Get<T>(e.GetValue());
            Label = e.GetDisplayName();
        }

        public T Value { get; set; }
        public string Label { get; set; }
        public bool Disabled { get; set; }
    }
}
