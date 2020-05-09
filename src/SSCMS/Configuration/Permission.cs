namespace SSCMS.Configuration
{
    public class Permission
    {
        /// <summary>
        /// 获取或设置权限Id。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 获取或设置权限的显示的文本。
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 获取或设置资源类型。
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 获取或设置权限的排序。
        /// </summary>
        public int? Order { get; set; }
    }
}