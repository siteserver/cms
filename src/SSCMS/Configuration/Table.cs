using System.Collections.Generic;
using Datory;

namespace SSCMS.Configuration
{
    public class Table
    {
        /// <summary>
        /// 获取或设置表Id。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 获取或设置资源类型。
        /// </summary>
        public TableType Type { get; set; }
        public List<TableColumn> Columns { get; set; }
        public List<InputStyle> Styles { get; set; }
    }
}
