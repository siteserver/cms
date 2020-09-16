namespace SSCMS.Configuration
{
    public class SiteType
    {
        /// <summary>
        /// 获取或设置站点类型Id。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 获取或设置站点类型的显示的文本。
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 获取或设置站点类型的图标。
        /// </summary>
        public string IconClass { get; set; }

        /// <summary>
        /// 获取或设置站点类型的排序。
        /// </summary>
        public int? Order { get; set; }
    }
}