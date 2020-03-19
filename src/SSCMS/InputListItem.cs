namespace SSCMS
{
    /// <summary>
    /// 表示列表中的数据项。
    /// </summary>
    public class InputListItem
    {
        /// <summary>
        /// 获取或设置列表中所表示的项的显示的文本。
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 获取或设置列表中所表示的项的值。
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 获取或设置指示是否选定了项的值。
        /// </summary>
        public bool Selected { get; set; }
    }
}
