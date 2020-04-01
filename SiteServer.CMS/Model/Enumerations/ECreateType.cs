namespace SiteServer.CMS.Model.Enumerations
{
    public enum ECreateType
	{
        Channel,
        Content,
        File,
        Special,
        AllContent
    }

    public static class ECreateTypeUtils
	{
        public static string GetText(ECreateType createType)
        {
            if (createType == ECreateType.Channel)
            {
                return "栏目页";
            }
            if (createType == ECreateType.Content)
            {
                return "内容页";
            }
            if (createType == ECreateType.File)
            {
                return "文件页";
            }
            if (createType == ECreateType.Special)
            {
                return "专题页";
            }
            if (createType == ECreateType.AllContent)
            {
                return "栏目下所有内容页";
            }

            return string.Empty;
        }
	}
}
