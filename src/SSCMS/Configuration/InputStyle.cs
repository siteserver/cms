using System.Collections.Generic;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Configuration
{
    /// <summary>
    /// 表单字段的输入样式。
    /// </summary>
    public class InputStyle
    {
        public InputStyle()
        {

        }

        public InputStyle(TableStyle style)
        {
            Id = style.Id;
            AttributeName = style.AttributeName;
            DisplayName = style.DisplayName;
            HelpText = style.HelpText;
            InputType = style.InputType;
            Rules = style.Rules;
            Items = style.Items;
            Taxis = style.Taxis;
            DefaultValue = style.DefaultValue;
        }

        /// <summary>
        /// 样式的输入类型。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 字段名称。
        /// </summary>
        public string AttributeName { get; set; }
        public string DisplayName { get; set; }
        /// <summary>
        /// 表单输入的提示信息。
        /// </summary>
        public string HelpText { get; set; }
        /// <summary>
        /// 样式的输入类型。
        /// </summary>
        public InputType InputType { get; set; }
        public List<InputStyleRule> Rules { get; set; }
        public List<InputStyleItem> Items { get; set; }
        public int Taxis { get; set; }
        public bool IsSystem { get; set; }
        /// <summary>
        /// 表单输入的默认值。
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 样式的显示宽度。
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// 样式的显示高度。
        /// </summary>
        public string Height { get; set; }
    }
}