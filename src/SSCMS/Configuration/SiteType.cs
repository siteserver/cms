namespace SSCMS.Configuration
{
    public class SiteType
    {
        /// <summary>
        /// 获取或设置菜单Id。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 获取或设置菜单的显示的文本。
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 获取或设置类型的排序。
        /// </summary>
        public int? Order { get; set; }
    }
}