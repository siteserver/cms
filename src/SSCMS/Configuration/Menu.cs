using System.Collections.Generic;

namespace SSCMS.Configuration
{
    /// <summary>
    /// 插件菜单。
    /// 插件菜单可显示在系统头部、左侧或者内容列表中。
    /// </summary>
    public class Menu
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
        /// 获取或设置资源类型。
        /// </summary>
        public IList<string> Type { get; set; }

        /// <summary>
        /// 获取或设置菜单的显示图片CSS类。
        /// </summary>
        public string IconClass { get; set; }

        /// <summary>
        /// 获取或设置菜单的链接地址。
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// 获取或设置菜单的链接定位窗口。
        /// </summary>
        public string Target { get; set; }

        public IList<string> Permissions { get; set; }

        /// <summary>
        /// 获取或设置菜单的排序。
        /// </summary>
        public int? Order { get; set; }

        /// <summary>
        /// 获取或设置菜单的下级菜单列表。
        /// </summary>
        public IList<Menu> Children { get; set; }
    }
}
