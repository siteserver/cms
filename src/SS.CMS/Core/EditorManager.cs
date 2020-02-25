using SS.CMS.Abstractions;

namespace SS.CMS.Core
{
    public static class EditorManager
    {
        public static string GetCountName(TableStyle style)
        {
            return $"{style.AttributeName}_Count";
        }

        public static string GetExtendName(TableStyle style, int n)
        {
            return $"{style.AttributeName}_{n}";
        }
    }
}
