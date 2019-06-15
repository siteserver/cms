using SS.CMS.Abstractions.Enums;

namespace SS.CMS.Core.Models.Enumerations
{
    public static class CreateTypeUtils
    {
        public static string GetText(CreateType createType)
        {
            if (createType == CreateType.Channel)
            {
                return "栏目页";
            }
            if (createType == CreateType.Content)
            {
                return "内容页";
            }
            if (createType == CreateType.File)
            {
                return "文件页";
            }
            if (createType == CreateType.Special)
            {
                return "专题页";
            }
            if (createType == CreateType.AllContent)
            {
                return "栏目下所有内容页";
            }

            return string.Empty;
        }
    }
}
